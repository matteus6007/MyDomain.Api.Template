using AutoFixture.Xunit2;

using Microsoft.Extensions.Options;

using Moq;

using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;
using MyDomain.Infrastructure.Persistence.Options;

using Shouldly;

namespace MyDomain.Tests.Integration.Repositories;

public class MyAggregateRepositoryTests
{
    private static readonly DateTime CreatedOn = new(2013, 1, 1);
    private static readonly DateTime UpdatedOn = new(2013, 1, 2);
    private readonly DatabaseHelper<Guid, MyAggregate> _databaseHelper;
    private readonly MyAggregateRepository _sut;

    public MyAggregateRepositoryTests()
    {
        _databaseHelper = new DatabaseHelper<Guid, MyAggregate>("MyAggregates", "Id");

        var options = new Mock<IOptionsSnapshot<DatabaseOptions>>();
        options.Setup(_ => _.Value).Returns(_databaseHelper.Options);

        MyAggregateIdTypeHandler.AddTypeHandlers();

        _sut = new MyAggregateRepository(options.Object);
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WhenRecordExists_ThenShouldReturnRecord(
        MyAggregate aggregate)
    {
        // Arrange
        await GivenRecordExists(aggregate);

        // Act
        var result = await _sut.GetByIdAsync(aggregate.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Version.ShouldBe(aggregate.Version);
        result.Name.ShouldBe(aggregate.Name);
        result.CreatedOn.ShouldBe(aggregate.CreatedOn, TimeSpan.FromSeconds(1));
        result.UpdatedOn.ShouldBe(aggregate.UpdatedOn, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [AutoData]
    public async Task AddAsync_WhenRecordDoesNotExist_ThenRecordShouldBeSavedSuccessfully(
        string name,
        string description)
    {
        // Arrange
        var aggregate = MyAggregate.Create(name, description, CreatedOn);

        // Act
        await _sut.AddAsync(aggregate);

        // Assert
        var result = await ThenRecordExists(aggregate.Id);

        result.ShouldNotBeNull();
        result.Version.ShouldBe(1);
        result.Name.ShouldBe(name);
        result.Description.ShouldBe(description);
        result.CreatedOn.ShouldBe(aggregate.CreatedOn, TimeSpan.FromSeconds(1));
        result.UpdatedOn.ShouldBe(aggregate.CreatedOn, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WhenRecordExists_ThenRecordShouldBeSavedSuccessfully(
        MyAggregate aggregate,
        string updatedName,
        string updatedDescription) 
    {
        // Arrange
        var expectedVersion = aggregate.Version + 1;

        await GivenRecordExists(aggregate);

        aggregate.Update(updatedName, updatedDescription, UpdatedOn);

        // Act
        await _sut.UpdateAsync(aggregate);

        // Assert
        var result = await ThenRecordExists(aggregate.Id);

        result.ShouldNotBeNull();
        result.Version.ShouldBe(expectedVersion);
        result.Name.ShouldBe(updatedName);
        result.Description.ShouldBe(updatedDescription);
        result.CreatedOn.ShouldBe(aggregate.CreatedOn, TimeSpan.FromSeconds(1));
        result.UpdatedOn.ShouldBe(aggregate.UpdatedOn, TimeSpan.FromSeconds(1));
    }

    private async Task GivenRecordExists(MyAggregate record)
    {
        await _databaseHelper.AddRecordAsync(record);
    }

    private async Task<MyAggregate> ThenRecordExists(MyAggregateId id)
    {
        var record = await _databaseHelper.GetRecordAsync<MyAggregate>(id.Value);

        return record;
    }      
}
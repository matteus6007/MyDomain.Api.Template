using AutoFixture.Xunit2;

using ErrorOr;

using Microsoft.Extensions.Options;

using Moq;

using MyDomain.Domain;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Persistence.Repositories;
using MyDomain.Tests.Integration.Infrastructure;

using Shouldly;

namespace MyDomain.Tests.Integration.Repositories;

public class MyAggregateRepositoryTests : IDisposable
{
    private static readonly DateTime CreatedOn = new(2013, 1, 1);
    private static readonly DateTime UpdatedOn = new(2013, 1, 2);
    private readonly DatabaseHelper<Guid, MyAggregateState> _databaseHelper;
    private readonly MyAggregateRepository _sut;

    public MyAggregateRepositoryTests()
    {
        _databaseHelper = new DatabaseHelper<Guid, MyAggregateState>("MyAggregates", "Id");

        var options = new Mock<IOptionsSnapshot<DatabaseOptions>>();
        options.Setup(_ => _.Value).Returns(_databaseHelper.Options);

        MyAggregateIdTypeHandler.AddTypeHandlers();

        _sut = new MyAggregateRepository(options.Object);
    }

    public void Dispose() => _databaseHelper.CleanTableAsync().GetAwaiter().GetResult();

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
        result.State.Name.ShouldBe(aggregate.State.Name);
        result.State.Description.ShouldBe(aggregate.State.Description);
        result.State.CreatedOn.ShouldBe(aggregate.State.CreatedOn, TimeSpan.FromSeconds(1));
        result.State.UpdatedOn.ShouldBe(aggregate.State.UpdatedOn, TimeSpan.FromSeconds(1));
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
        ErrorOr<Created> result = await _sut.AddAsync(aggregate);

        _databaseHelper.TrackId(aggregate.Id.Value);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Created);

        var record = await ThenRecordExists(aggregate.Id);

        record.ShouldNotBeNull();
        record.Version.ShouldBe(1);
        record.Name.ShouldBe(name);
        record.Description.ShouldBe(description);
        record.CreatedOn.ShouldBe(aggregate.State.CreatedOn, TimeSpan.FromSeconds(1));
        record.UpdatedOn.ShouldBe(aggregate.State.CreatedOn, TimeSpan.FromSeconds(1));
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
        ErrorOr<Updated> result = await _sut.UpdateAsync(aggregate);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Updated);

        var record = await ThenRecordExists(aggregate.Id);

        record.ShouldNotBeNull();
        record.Version.ShouldBe(expectedVersion);
        record.Name.ShouldBe(updatedName);
        record.Description.ShouldBe(updatedDescription);
        record.CreatedOn.ShouldBe(aggregate.State.CreatedOn, TimeSpan.FromSeconds(1));
        record.UpdatedOn.ShouldBe(aggregate.State.UpdatedOn, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WhenRecordDoesNotExist_ThenShouldThrowException(
        MyAggregate aggregate,
        string updatedName,
        string updatedDescription)
    {
        // Arrange
        aggregate.Update(updatedName, updatedDescription, UpdatedOn);

        // Act
        ErrorOr<Updated> result = await _sut.UpdateAsync(aggregate);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }

    [Theory]
    [AutoData]
    public async Task UpdateAsync_WhenRecordVersionDoesNotMatch_ThenShouldThrowException(
        string updatedName,
        string updatedDescription)
    {
        // Arrange
        var id = MyAggregateId.CreateUnique();

        var existingAggregateState = new MyAggregateState
        {
            Id = id,
            Version = 2,
            Name = "Test",
            Description = "Test",
            CreatedOn = CreatedOn,
            UpdatedOn = UpdatedOn
        };

        var existingAggregate = new MyAggregate(existingAggregateState);

        await GivenRecordExists(existingAggregate);

        // reset version
        var aggregateState = new MyAggregateState
        {
            Id = id,
            Version = existingAggregateState.Version - 1,
            Name = existingAggregateState.Name,
            Description = existingAggregateState.Description,
            CreatedOn = existingAggregateState.CreatedOn,
            UpdatedOn = existingAggregateState.UpdatedOn
        };

        var aggregate = new MyAggregate(aggregateState);
        aggregate.Update(updatedName, updatedDescription, UpdatedOn);

        // Act
        ErrorOr<Updated> result = await _sut.UpdateAsync(aggregate);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Conflict);
        result.FirstError.Description.ShouldContain("version is out of date");
    }

    private async Task GivenRecordExists(MyAggregate record)
    {
        await _databaseHelper.AddRecordAsync(record.Id.Value, record.State);
    }

    private async Task<MyAggregateState> ThenRecordExists(MyAggregateId id)
    {
        var record = await _databaseHelper.GetRecordAsync<MyAggregateState>(id.Value);

        return record;
    }      
}
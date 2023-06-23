using AutoFixture.Xunit2;

using Microsoft.Extensions.Options;

using Moq;

using MyDomain.Domain.MyAggregate;
using MyDomain.Infrastructure.Persistence;
using MyDomain.Infrastructure.Persistence.Options;

using Shouldly;

namespace MyDomain.Tests.Integration.Repositories;

public class MyAggregateRepositoryTests
{
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

    private async Task GivenRecordExists(MyAggregate record)
    {
        await _databaseHelper.AddRecordAsync(record);
    }           
}
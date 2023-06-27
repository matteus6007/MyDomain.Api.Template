using AutoFixture.Xunit2;

using Shouldly;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Infrastructure.Persistence.Repositories;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Tests.Integration.Repositories;

public class InMemoryMyAggregateRepositoryTests
{
    private readonly IMyAggregateRepository _sut;

    public InMemoryMyAggregateRepositoryTests()
    {
        _sut = new InMemoryMyAggregateRepository();
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WhenRecordExists_ThenShouldReturnRecord(
        string name,
        string description)
    {
        // Arrange
        var existingRecord = await GivenRecordExists(name, description);

        // Act
        var result = await _sut.GetByIdAsync(existingRecord.Id);

        // Assert
        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoData]
    public async Task GetByIdAsync_WhenRecordDoesNotExist_ThenShouldReturnNull(
        MyAggregateId id)
    {
        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.ShouldBeNull();
    }

    [Theory]
    [AutoData]
    public async Task Update_WhenRecordExists_ThenShouldBeUpdated(
        MyAggregate existingAggregate,
        string updatedName,
        string updatedDescription)
    {
        // Arrange
        var expectedVersion = existingAggregate.Version + 1;

        await GivenRecordExists(existingAggregate);

        // Act
        existingAggregate.Update(updatedName, updatedDescription, DateTime.UtcNow);

        // Assert
        var result = await _sut.GetByIdAsync(existingAggregate.Id);

        result.ShouldNotBeNull();
        result.Version.ShouldBe(expectedVersion);
        result.Name.ShouldBe(updatedName);
        result.Description.ShouldBe(updatedDescription);
    }

    private async Task<MyAggregate> GivenRecordExists(string name, string description)
    {
        var aggregate = MyAggregate.Create(name, description, DateTime.UtcNow);

        await GivenRecordExists(aggregate);

        return aggregate;
    }

    private async Task GivenRecordExists(MyAggregate aggregate)
    {
        await _sut.AddAsync(aggregate);
    }
}
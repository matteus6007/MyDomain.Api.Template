using AutoFixture.Xunit2;

using ErrorOr;

using Moq;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.Events;
using MyDomain.Domain.MyAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;

using Shouldly;

namespace MyDomain.Tests;

public class AggregatePersistenceServiceTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<IWriteRepository<MyAggregate, MyAggregateId>> _repositoryMock = new();
    private readonly IAggregatePersistenceService<MyAggregate, MyAggregateId> _sut;

    public AggregatePersistenceServiceTests()
    {
        _sut = new AggregatePersistenceService<MyAggregate, MyAggregateId>(
            _eventPublisherMock.Object,
            _repositoryMock.Object);
    }

    [Theory]
    [AutoData]
    public async Task PersistAsync_WhenRecordIsNew_ThenResultShouldBeSuccess(
        string name,
        string description)
    {
        // Arrange
        GivenRecordIsCreatedSuccessfully();

        var aggregate = MyAggregate.Create(name, description, DateTime.UtcNow);

        // Act
        var result = await _sut.PersistAsync(aggregate);

        // Assert
        ThenResultShouldBeSuccess(result);
        TheRecordShouldBeCreated(aggregate);
        ThenCreatedEventShouldBePublished();
    }

    [Theory]
    [AutoData]
    public async Task PersistAsync_WhenRecordAlreadyExists_AndRecordIsUpdated_ThenResultShouldBeSuccess(
        MyAggregate aggregate,
        string updatedName,
        string updatedDescription)
    {
        // Arrange
        GivenRecordIsUpdatedSuccessfully();

        aggregate.Update(updatedName, updatedDescription, DateTime.UtcNow);

        // Act
        var result = await _sut.PersistAsync(aggregate);

        // Assert
        ThenResultShouldBeSuccess(result);
        ThenRecordShouldBeUpdated(aggregate);
        ThenUpdatedEventShouldBePublished();
    }

    private void GivenRecordIsCreatedSuccessfully()
    {
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MyAggregate>()))
            .ReturnsAsync(Result.Created);
    }

    private void GivenRecordIsUpdatedSuccessfully()
    {
        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<MyAggregate>()))
            .ReturnsAsync(Result.Updated);
    }

    private static void ThenResultShouldBeSuccess(ErrorOr<Success> result)
    {
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
    }

    private void TheRecordShouldBeCreated(MyAggregate aggregate)
    {
        _repositoryMock
            .Verify(x => x.AddAsync(aggregate));
    }

    private void ThenCreatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Verify(x => x.PublishAsync(It.Is<IDomainEvent>(y => y.GetType() == typeof(MyAggregateCreated))));
    }

    private void ThenRecordShouldBeUpdated(MyAggregate aggregate)
    {
        _repositoryMock
            .Verify(x => x.UpdateAsync(aggregate));
    }

    private void ThenUpdatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Verify(x => x.PublishAsync(It.Is<IDomainEvent>(y => y.GetType() == typeof(MyAggregateUpdated))));
    }
}
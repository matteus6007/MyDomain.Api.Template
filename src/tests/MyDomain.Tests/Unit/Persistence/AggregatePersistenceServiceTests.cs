using AutoFixture.Xunit2;

using ErrorOr;

using Moq;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.Events;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;

using Shouldly;

namespace MyDomain.Tests;

public class AggregatePersistenceServiceTests
{
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<IWriteRepository<MyDomainAggregate, MyDomainId>> _repositoryMock = new();
    private readonly IAggregatePersistenceService<MyDomainAggregate, MyDomainId> _sut;

    public AggregatePersistenceServiceTests()
    {
        _sut = new AggregatePersistenceService<MyDomainAggregate, MyDomainId>(
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

        var aggregate = MyDomainAggregate.Create(name, description, DateTime.UtcNow);

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
        Domain.MyDomainAggregate.MyDomainAggregate aggregate,
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
            .Setup(x => x.AddAsync(It.IsAny<MyDomainAggregate>()))
            .ReturnsAsync(Result.Created);
    }

    private void GivenRecordIsUpdatedSuccessfully()
    {
        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<MyDomainAggregate>()))
            .ReturnsAsync(Result.Updated);
    }

    private static void ThenResultShouldBeSuccess(ErrorOr<Success> result)
    {
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
    }

    private void TheRecordShouldBeCreated(MyDomainAggregate aggregate)
    {
        _repositoryMock
            .Verify(x => x.AddAsync(aggregate));
    }

    private void ThenCreatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Verify(x => x.PublishAsync(It.Is<IDomainEvent>(y => y.GetType() == typeof(MyDomainCreated))));
    }

    private void ThenRecordShouldBeUpdated(MyDomainAggregate aggregate)
    {
        _repositoryMock
            .Verify(x => x.UpdateAsync(aggregate));
    }

    private void ThenUpdatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Verify(x => x.PublishAsync(It.Is<IDomainEvent>(y => y.GetType() == typeof(MyDomainUpdated))));
    }
}
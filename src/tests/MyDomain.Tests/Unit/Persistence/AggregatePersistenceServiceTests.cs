using AutoFixture.Xunit2;

using ErrorOr;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.Events;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;
using MyDomain.Infrastructure.Persistence;

using NSubstitute;

using Shouldly;

namespace MyDomain.Tests;

public class AggregatePersistenceServiceTests
{
    private readonly IEventPublisher _eventPublisherMock = Substitute.For<IEventPublisher>();
    private readonly IWriteRepository<MyDomainAggregate, MyDomainId> _repositoryMock = Substitute.For<IWriteRepository<MyDomainAggregate, MyDomainId>>();
    private readonly IAggregatePersistenceService<MyDomainAggregate, MyDomainId> _sut;

    public AggregatePersistenceServiceTests()
    {
        _sut = new AggregatePersistenceService<MyDomainAggregate, MyDomainId>(
            _eventPublisherMock,
            _repositoryMock);
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
            .AddAsync(Arg.Any<MyDomainAggregate>())
            .Returns(Result.Created);
    }

    private void GivenRecordIsUpdatedSuccessfully()
    {
        _repositoryMock
            .UpdateAsync(Arg.Any<MyDomainAggregate>())
            .Returns(Result.Updated);
    }

    private static void ThenResultShouldBeSuccess(ErrorOr<Success> result)
    {
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(Result.Success);
    }

    private void TheRecordShouldBeCreated(MyDomainAggregate aggregate)
    {
        _repositoryMock
            .Received()
            .AddAsync(aggregate);
    }

    private void ThenCreatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Received()
            .PublishAsync(Arg.Any<MyDomainCreated>());
    }

    private void ThenRecordShouldBeUpdated(MyDomainAggregate aggregate)
    {
        _repositoryMock
            .Received()
            .UpdateAsync(aggregate);
    }

    private void ThenUpdatedEventShouldBePublished()
    {
        _eventPublisherMock
            .Received()
            .PublishAsync(Arg.Any<MyDomainUpdated>());
    }
}
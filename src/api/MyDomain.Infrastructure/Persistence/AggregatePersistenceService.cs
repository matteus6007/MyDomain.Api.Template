using ErrorOr;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Common.Interfaces;

namespace MyDomain.Infrastructure.Persistence;

public class AggregatePersistenceService<TAggregate, TId> : IAggregatePersistenceService<TAggregate, TId>
    where TAggregate : IAggregateRoot<TId>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IWriteRepository<TAggregate, TId> _repository;

    public AggregatePersistenceService(
        IEventPublisher eventPublisher,
        IWriteRepository<TAggregate, TId> repository)
    {
        _eventPublisher = eventPublisher;
        _repository = repository;
    }

    public async Task<ErrorOr<Success>> PersistAsync(TAggregate aggregate)
    {
        var saveResult = await SaveAsync(aggregate);

        if (saveResult.IsError)
        {
            return saveResult.FirstError;
        }

        if (!aggregate.DomainEvents.Any())
        {
            return Result.Success;
        }

        await PublishEventsAsync(aggregate);

        return Result.Success;
    }

    private async Task<ErrorOr<Success>> SaveAsync(TAggregate aggregate)
    {
        if (aggregate.IsNew)
        {
            var result = await _repository.AddAsync(aggregate);

            return result.IsError ? result.FirstError : Result.Success;
        }
        else
        {
            var result = await _repository.UpdateAsync(aggregate);

            return result.IsError ? result.FirstError : Result.Success;
        }
    }

    private async Task PublishEventsAsync(TAggregate aggregate)
    {
        await Task.WhenAll(aggregate.DomainEvents.Select(_eventPublisher.PublishAsync));
    }
}
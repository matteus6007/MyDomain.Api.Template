using ErrorOr;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.Common.Interfaces;

namespace MyDomain.Infrastructure.Persistence;

public class AggregatePersistenceService<TAggregate, TId> : IAggregatePersistenceService<TAggregate, TId>
    where TAggregate : IAggregateRoot<TId>
{
    private readonly IWriteRepository<TAggregate, TId> _repository;

    public AggregatePersistenceService(IWriteRepository<TAggregate, TId> repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<Success>> PersistAsync(TAggregate aggregate)
    {
        return await SaveAsync(aggregate);
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
}

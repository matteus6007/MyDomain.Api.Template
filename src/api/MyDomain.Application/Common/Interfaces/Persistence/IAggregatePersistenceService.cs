using ErrorOr;

using MyDomain.Domain.Common.Interfaces;

namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IAggregatePersistenceService<in TAggregate, TId>
    where TAggregate : IAggregateRoot<TId>
{
    Task<ErrorOr<Success>> PersistAsync(TAggregate aggregate);
}
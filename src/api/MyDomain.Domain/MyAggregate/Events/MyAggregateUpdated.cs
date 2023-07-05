using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Domain.MyAggregate.Events;

public record MyAggregateUpdated(MyAggregateId Id, string Name, string Description, DateTime UpdatedOn) : IDomainEvent;

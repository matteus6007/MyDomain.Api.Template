using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Domain.MyDomainAggregate.Events;

public record MyDomainUpdated(MyDomainId Id, string Name, string Description, DateTime UpdatedOn) : IDomainEvent;
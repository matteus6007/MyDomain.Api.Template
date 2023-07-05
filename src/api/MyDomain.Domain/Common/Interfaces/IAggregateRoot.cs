namespace MyDomain.Domain.Common.Interfaces;

public interface IAggregateRoot<TId>
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    TId Id { get; }
    bool IsNew { get; }
    int Version { get; }
    int PreviousVersion { get; }
}

using MyDomain.Domain.Common.Interfaces;

namespace MyDomain.Domain.Common.Models;

public abstract class AggregateRoot<TState, TId> : Entity<TId>, IAggregateRoot<TId>
    where TId : notnull
    where TState : IAggregateState<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public bool IsNew { get; protected set; }

    public int Version => State.Version;

    public int PreviousVersion { get; protected set; }

    public TState State { get; protected set; }

    protected AggregateRoot(TState state)
     : base(state.Id)
    {
        State = state;
        PreviousVersion = state.Version;
        IsNew = state.Version == 0;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        State.Version++;

        _domainEvents.Add(domainEvent);
    }
}
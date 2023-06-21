namespace MyDomain.Domain.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    public long Version { get; private set; }

    protected AggregateRoot(TId id) : base(id)
    {
        Version = 1;
    }
}
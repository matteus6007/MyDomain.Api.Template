using Newtonsoft.Json;

namespace MyDomain.Domain.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    public int Version { get; protected set; }

    [JsonIgnore]
    public int PreviousVersion { get; protected set; }

    protected AggregateRoot(TId id, int version)
     : base(id)
    {
        Version = version;
        PreviousVersion = version;
    }
}
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Domain;

public class MyAggregateState : IAggregateState<MyAggregateId>
{
    public required MyAggregateId Id { get; set; }
    public int Version { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required DateTime UpdatedOn { get; set; }
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
}

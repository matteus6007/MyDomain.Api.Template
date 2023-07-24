using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Domain.MyDomainAggregate;

public class MyDomainState : IAggregateState<MyDomainId>
{
    public required MyDomainId Id { get; set; }
    public int Version { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required DateTime UpdatedOn { get; set; }
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";
}
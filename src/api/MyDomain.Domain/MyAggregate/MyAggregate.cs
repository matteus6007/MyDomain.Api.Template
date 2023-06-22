using MyDomain.Domain.Common.Models;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Domain.MyAggregate;

public sealed class MyAggregate : AggregateRoot<MyAggregateId>
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime Created { get; private set; }

    public DateTime Updated { get; private set; }

    private MyAggregate(
        MyAggregateId id,
        string name,
        string description,
        DateTime created,
        DateTime updated)
         : base(id)
    {
        Name = name;
        Description = description;
        Created = created;
        Updated = updated;
    }

    public static MyAggregate Create(
        string name,
        string description)
    {
        return new(
            MyAggregateId.CreateUnique(),
            name,
            description,
            DateTime.UtcNow,
            DateTime.UtcNow); // TODO: create DateTime wrapper
    }

    public void Update(
        string name,
        string description)
    {
        Name = name;
        Description = description;
        Updated = DateTime.UtcNow;
    }
}
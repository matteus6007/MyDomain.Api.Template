using MyDomain.Domain.Common.Models;
using MyDomain.Domain.MyAggregate.Events;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Domain.MyAggregate;

public sealed class MyAggregate : AggregateRoot<MyAggregateState, MyAggregateId>
{
    public MyAggregate(MyAggregateState state)
     : base(state)
    {
    }

    public static MyAggregate Create(
        string name,
        string description,
        DateTime createdOn)
    {
        var state = new MyAggregateState
        {
            Id = MyAggregateId.CreateUnique(),
            Name = name,
            Description = description,
            CreatedOn = createdOn,
            UpdatedOn = createdOn
        };

        var aggregate = new MyAggregate(state);

        var @event = new MyAggregateCreated(
            aggregate.Id,
            name,
            description,
            createdOn);

        aggregate.AddDomainEvent(@event);

        return aggregate;
    }

    public void Update(
        string name,
        string description,
        DateTime updatedOn)
    {
        State.Name = name;
        State.Description = description;
        State.UpdatedOn = updatedOn;

        var @event = new MyAggregateUpdated(
            Id,
            name,
            description,
            updatedOn);

        AddDomainEvent(@event);
    }
}
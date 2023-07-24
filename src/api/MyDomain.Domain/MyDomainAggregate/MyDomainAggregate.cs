using MyDomain.Domain.Common.Models;
using MyDomain.Domain.MyDomainAggregate.Events;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Domain.MyDomainAggregate;

public sealed class MyDomainAggregate : AggregateRoot<MyDomainState, MyDomainId>
{
    public MyDomainAggregate(MyDomainState state)
     : base(state)
    {
    }

    public static MyDomainAggregate Create(
        string name,
        string description,
        DateTime createdOn)
    {
        var state = new MyDomainState
        {
            Id = MyDomainId.CreateUnique(),
            Name = name,
            Description = description,
            CreatedOn = createdOn,
            UpdatedOn = createdOn
        };

        var aggregate = new MyDomainAggregate(state);

        var @event = new MyDomainCreated(
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

        var @event = new MyDomainUpdated(
            Id,
            name,
            description,
            updatedOn);

        AddDomainEvent(@event);
    }
}
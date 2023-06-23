using MyDomain.Domain.Common.Models;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Domain.MyAggregate;

public sealed class MyAggregate : AggregateRoot<MyAggregateId>
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public DateTime UpdatedOn { get; private set; }

    public MyAggregate(
        MyAggregateId id,
        int version,
        string name,
        string description,
        DateTime createdOn,
        DateTime updatedOn)
         : base(id, version)
    {
        Name = name;
        Description = description;
        CreatedOn = createdOn;
        UpdatedOn = updatedOn;
    }

    public static MyAggregate Create(
        string name,
        string description,
        DateTime createdOn)
    {
        return new(
            MyAggregateId.CreateUnique(),
            1,
            name,
            description,
            createdOn,
            createdOn);
    }

    public void Update(
        string name,
        string description,
        DateTime updatedOn)
    {
        Version++;
        Name = name;
        Description = description;
        UpdatedOn = updatedOn;
    }
}
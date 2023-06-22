using MyDomain.Domain.Common.Models;

namespace MyDomain.Domain.MyAggregate.ValueObjects;

public sealed class MyAggregateId : ValueObject
{
    public Guid Value { get; }

    public MyAggregateId(Guid value)
    {
        Value = value;
    }

    public static MyAggregateId CreateUnique() => new(Guid.NewGuid());

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
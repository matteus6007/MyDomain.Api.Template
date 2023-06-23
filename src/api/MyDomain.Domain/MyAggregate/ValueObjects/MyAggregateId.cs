using MyDomain.Domain.Common.Models;

namespace MyDomain.Domain.MyAggregate.ValueObjects;

public sealed class MyAggregateId : ValueObject
{
    public Guid Value { get; }

    private MyAggregateId(Guid value)
    {
        Value = value;
    }

    public static MyAggregateId CreateUnique() => new(Guid.NewGuid());

    public static MyAggregateId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
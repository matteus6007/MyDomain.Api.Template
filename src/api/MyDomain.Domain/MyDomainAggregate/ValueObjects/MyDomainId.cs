using MyDomain.Domain.Common.Models;

namespace MyDomain.Domain.MyDomainAggregate.ValueObjects;

public sealed class MyDomainId : ValueObject
{
    public Guid Value { get; }

    private MyDomainId(Guid value)
    {
        Value = value;
    }

    public static MyDomainId CreateUnique() => new(Guid.NewGuid());

    public static MyDomainId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
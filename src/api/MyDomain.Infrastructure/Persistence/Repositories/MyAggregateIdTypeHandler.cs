using System.Data;

using Dapper;

using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence.Repositories;

public class MyAggregateIdTypeHandler : SqlMapper.TypeHandler<MyDomainId>
{
    private static bool _initialised;

    public static void AddTypeHandlers()
    {
        if (_initialised)
        {
            return;
        }

        SqlMapper.AddTypeHandler(typeof(MyDomainId), new MyAggregateIdTypeHandler());

        _initialised = true;
    }

    public override void SetValue(IDbDataParameter parameter, MyDomainId value)
    {
        parameter.Value = value?.ToString();
    }

    public override MyDomainId Parse(object value)
    {
        var myAggregateId = value?.ToString();

        return MyDomainId.Create(new Guid(myAggregateId));
    }
}
using System.Data;

using Dapper;

using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence.Repositories;

public class MyAggregateIdTypeHandler : SqlMapper.TypeHandler<MyAggregateId>
{
    private static bool _initialised;

    public static void AddTypeHandlers()
    {
        if (_initialised)
        {
            return;
        }

        SqlMapper.AddTypeHandler(typeof(MyAggregateId), new MyAggregateIdTypeHandler());

        _initialised = true;
    }

    public override void SetValue(IDbDataParameter parameter, MyAggregateId value)
    {
        parameter.Value = value?.ToString();
    }

    public override MyAggregateId Parse(object value)
    {
        var myAggregateId = value?.ToString();

        return MyAggregateId.Create(new Guid(myAggregateId));
    }
}
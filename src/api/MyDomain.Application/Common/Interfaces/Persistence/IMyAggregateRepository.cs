using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IMyAggregateRepository : IRepository<MyAggregate, MyAggregateId>
{
    
}
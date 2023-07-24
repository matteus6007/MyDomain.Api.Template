using ErrorOr;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence.Repositories;

public class InMemoryMyAggregateRepository :
    IReadRepository<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId>,
    IWriteRepository<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId>
{
    private readonly List<Domain.MyDomainAggregate.MyDomainAggregate> _items = new();

    public async Task<Domain.MyDomainAggregate.MyDomainAggregate?> GetByIdAsync(MyDomainId id)
    {
        await Task.CompletedTask;

        var aggregate = _items.FirstOrDefault(x => x.Id == id);

        return aggregate;
    }

    public async Task<ErrorOr<Created>> AddAsync(Domain.MyDomainAggregate.MyDomainAggregate data)
    {
        await Task.CompletedTask;

        _items.Add(data);

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Domain.MyDomainAggregate.MyDomainAggregate data)
    {
        await Task.CompletedTask;

        var item = _items.FirstOrDefault(x => x.Id == data.Id);

        if (item is null)
        {
            return Error.NotFound();
        }

        item = data;

        return Result.Updated;
    }
}
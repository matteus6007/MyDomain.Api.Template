using ErrorOr;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence.Repositories;

public class InMemoryMyAggregateRepository :
    IReadRepository<MyAggregate, MyAggregateId>,
    IWriteRepository<MyAggregate, MyAggregateId>
{
    private readonly List<MyAggregate> _items = new();

    public async Task<MyAggregate?> GetByIdAsync(MyAggregateId id)
    {
        await Task.CompletedTask;

        var aggregate = _items.FirstOrDefault(x => x.Id == id);

        return aggregate;
    }

    public async Task<ErrorOr<Created>> AddAsync(MyAggregate data)
    {
        await Task.CompletedTask;

        _items.Add(data);

        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(MyAggregate data)
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
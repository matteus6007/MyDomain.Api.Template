using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence;

public class InMemoryMyAggregateRepository : IMyAggregateRepository
{
    private readonly List<MyAggregate> _items = new();

    public async Task<MyAggregate?> GetByIdAsync(MyAggregateId id)
    {
        await Task.CompletedTask;

        return _items.FirstOrDefault(x => x.Id == id);
    }    

    public async Task AddAsync(MyAggregate data)
    {
        await Task.CompletedTask;

        _items.Add(data);
    }

    public async Task UpdateAsync(MyAggregate data)
    {
        await Task.CompletedTask;

        var item = _items.FirstOrDefault(x => x.Id == data.Id);
        item = data;
    }
}

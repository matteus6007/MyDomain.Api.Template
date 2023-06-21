using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Infrastructure.Persistence;

public class InMemoryMyAggregateRepository : IMyAggregateRepository
{
    private static readonly List<MyAggregate> Items = new();

    public async Task AddAsync(MyAggregate data)
    {
        await Task.CompletedTask;

        Items.Add(data);
    }

    public async Task<MyAggregate?> GetByIdAsync(MyAggregateId id)
    {
        await Task.CompletedTask;

        return Items.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(MyAggregate data)
    {
        await Task.CompletedTask;

        var item = Items.FirstOrDefault(x => x.Id == data.Id);
        item = data;
    }
}

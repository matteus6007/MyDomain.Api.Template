namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IWriteRepository<TData, in TId>
{
    Task AddAsync(TData data);

    Task UpdateAsync(TData data);
}
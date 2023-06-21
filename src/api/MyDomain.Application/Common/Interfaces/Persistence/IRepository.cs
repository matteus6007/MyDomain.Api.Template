namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IRepository<TData, in TId>
{
    Task<TData?> GetByIdAsync(TId id);

    Task AddAsync(TData data);

    Task UpdateAsync(TData data);
}
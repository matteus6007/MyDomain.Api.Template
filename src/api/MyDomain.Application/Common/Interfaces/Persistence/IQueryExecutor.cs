namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IQueryExecutor<TData, in TId>
{
    Task<TData?> GetByIdAsync(TId id);
}
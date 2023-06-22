namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IReadRepository<TData, in TId>
{
    Task<TData?> GetByIdAsync(TId id);
}
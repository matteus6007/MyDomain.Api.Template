namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IRepository<TData, in TId> : IReadRepository<TData, TId>, IWriteRepository<TData, TId>
{
}
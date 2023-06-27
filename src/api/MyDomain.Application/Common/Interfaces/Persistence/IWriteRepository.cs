using ErrorOr;

namespace MyDomain.Application.Common.Interfaces.Persistence;

public interface IWriteRepository<TData, in TId>
{
    Task<ErrorOr<Created>> AddAsync(TData data);

    Task<ErrorOr<Updated>> UpdateAsync(TData data);
}
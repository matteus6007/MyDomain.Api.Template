using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.Models;

namespace MyDomain.Application.Services.Queries;

public class GetMyDomainByIdQueryHandler : IRequestHandler<GetMyDomainByIdQuery, ErrorOr<MyDomainResult>>
{
    private readonly IQueryExecutor<MyDomainReadModel, Guid> _queryExecutor;

    public GetMyDomainByIdQueryHandler(IQueryExecutor<MyDomainReadModel, Guid> queryExecutor)
    {
        _queryExecutor = queryExecutor;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(GetMyDomainByIdQuery request, CancellationToken cancellationToken)
    {
        var model = await _queryExecutor.GetByIdAsync(request.Id);

        if (model is null)
        {
            return Error.NotFound();
        }

        var result = new MyDomainResult(
            model.Id,
            model.Name,
            model.Description,
            model.CreatedOn,
            model.UpdatedOn);

        return result;
    }
}
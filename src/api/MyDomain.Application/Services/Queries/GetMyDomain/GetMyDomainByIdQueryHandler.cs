using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Application.Services.Queries;

public class GetMyDomainByIdQueryHandler : IRequestHandler<GetMyDomainByIdQuery, ErrorOr<MyDomainResult>>
{
    private readonly IMyAggregateRepository _repository;

    public GetMyDomainByIdQueryHandler(IMyAggregateRepository repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(GetMyDomainByIdQuery request, CancellationToken cancellationToken)
    {
        var id = MyAggregateId.Create(request.Id);

        var aggregate = await _repository.GetByIdAsync(id);

        if (aggregate is null)
        {
            return Error.NotFound();
        }

        var result = new MyDomainResult(
            aggregate.Id.Value,
            aggregate.Name,
            aggregate.Description,
            aggregate.CreatedOn,
            aggregate.UpdatedOn);

        return result;        
    }
}

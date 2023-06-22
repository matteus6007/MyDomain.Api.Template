using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Application.Services.Commands.UpdateMyDomain;

public class UpdateMyDomainCommandHandler : IRequestHandler<UpdateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IMyAggregateRepository _repository;
    private readonly IDateTimeProvider _dateTime;

    public UpdateMyDomainCommandHandler(
        IMyAggregateRepository repository,
        IDateTimeProvider dateTime)
    {
        _repository = repository;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(UpdateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var id = new MyAggregateId(request.Id);

        var aggregate = await _repository.GetByIdAsync(id);

        if (aggregate is null)
        {
            return Error.NotFound();
        }

        aggregate.Update(request.Name, request.Description, _dateTime.UtcNow);

        await _repository.UpdateAsync(aggregate);

        var result = new MyDomainResult(
            aggregate.Id.Value,
            aggregate.Name,
            aggregate.Description,
            aggregate.Created,
            aggregate.Updated);

        return result;        
    }
}

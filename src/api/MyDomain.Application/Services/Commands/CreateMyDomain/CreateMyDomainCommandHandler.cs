using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Application.Services.Commands.CreateMyDomain;

public class CreateMyDomainCommandHandler : IRequestHandler<CreateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IAggregatePersistenceService<MyAggregate, MyAggregateId> _persistenceService;
    private readonly IDateTimeProvider _dateTime;

    public CreateMyDomainCommandHandler(
        IAggregatePersistenceService<MyAggregate, MyAggregateId> persistenceService,
        IDateTimeProvider dateTime)
    {
        _persistenceService = persistenceService;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(CreateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var aggregate = MyAggregate.Create(request.Name, request.Description, _dateTime.UtcNow);

        ErrorOr<Success> response = await _persistenceService.PersistAsync(aggregate);

        if (response.IsError)
        {
            return response.FirstError;
        }

        var result = new MyDomainResult(
            aggregate.Id.Value,
            aggregate.State.Name,
            aggregate.State.Description,
            aggregate.State.CreatedOn,
            aggregate.State.UpdatedOn);

        return result;
    }
}
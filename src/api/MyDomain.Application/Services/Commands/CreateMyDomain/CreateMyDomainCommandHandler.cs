using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Application.Services.Commands.CreateMyDomain;

public class CreateMyDomainCommandHandler : IRequestHandler<CreateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IAggregatePersistenceService<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> _persistenceService;
    private readonly IDateTimeProvider _dateTime;

    public CreateMyDomainCommandHandler(
        IAggregatePersistenceService<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> persistenceService,
        IDateTimeProvider dateTime)
    {
        _persistenceService = persistenceService;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(CreateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var aggregate = Domain.MyDomainAggregate.MyDomainAggregate.Create(request.Name, request.Description, _dateTime.UtcNow);

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
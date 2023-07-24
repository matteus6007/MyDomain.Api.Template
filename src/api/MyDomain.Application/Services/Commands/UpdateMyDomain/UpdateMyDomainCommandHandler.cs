using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyDomainAggregate;
using MyDomain.Domain.MyDomainAggregate.ValueObjects;

namespace MyDomain.Application.Services.Commands.UpdateMyDomain;

public class UpdateMyDomainCommandHandler : IRequestHandler<UpdateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IReadRepository<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> _readRepository;
    private readonly IAggregatePersistenceService<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> _persistenceService;
    private readonly IDateTimeProvider _dateTime;

    public UpdateMyDomainCommandHandler(
        IReadRepository<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> readRepository,
        IAggregatePersistenceService<Domain.MyDomainAggregate.MyDomainAggregate, MyDomainId> persistenceService,
        IDateTimeProvider dateTime)
    {
        _readRepository = readRepository;
        _persistenceService = persistenceService;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(UpdateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var id = MyDomainId.Create(request.Id);

        var aggregate = await _readRepository.GetByIdAsync(id);

        if (aggregate is null)
        {
            return Error.NotFound();
        }

        aggregate.Update(request.Name, request.Description, _dateTime.UtcNow);

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
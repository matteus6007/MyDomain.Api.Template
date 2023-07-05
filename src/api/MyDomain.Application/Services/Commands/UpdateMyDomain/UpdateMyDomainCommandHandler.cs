using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyAggregate;
using MyDomain.Domain.MyAggregate.ValueObjects;

namespace MyDomain.Application.Services.Commands.UpdateMyDomain;

public class UpdateMyDomainCommandHandler : IRequestHandler<UpdateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IReadRepository<MyAggregate, MyAggregateId> _readRepository;
    private readonly IAggregatePersistenceService<MyAggregate, MyAggregateId> _persistenceService;
    private readonly IDateTimeProvider _dateTime;

    public UpdateMyDomainCommandHandler(
        IReadRepository<MyAggregate, MyAggregateId> readRepository,
        IAggregatePersistenceService<MyAggregate, MyAggregateId> persistenceService,
        IDateTimeProvider dateTime)
    {
        _readRepository = readRepository;
        _persistenceService = persistenceService;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(UpdateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var id = MyAggregateId.Create(request.Id);

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

using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Interfaces;
using MyDomain.Application.Common.Interfaces.Persistence;
using MyDomain.Application.Common.Models;
using MyDomain.Domain.MyAggregate;

namespace MyDomain.Application.Services.Commands.CreateMyDomain;

public class CreateMyDomainCommandHandler : IRequestHandler<CreateMyDomainCommand, ErrorOr<MyDomainResult>>
{
    private readonly IMyAggregateRepository _repository;
    private readonly IDateTimeProvider _dateTime;

    public CreateMyDomainCommandHandler(
        IMyAggregateRepository repository,
        IDateTimeProvider dateTime)
    {
        _repository = repository;
        _dateTime = dateTime;
    }

    public async Task<ErrorOr<MyDomainResult>> Handle(CreateMyDomainCommand request, CancellationToken cancellationToken)
    {
        var aggregate = MyAggregate.Create(request.Name, request.Description, _dateTime.UtcNow);

        await _repository.AddAsync(aggregate);

        var result = new MyDomainResult(
            aggregate.Id.Value,
            aggregate.Name,
            aggregate.Description,
            aggregate.Created,
            aggregate.Updated);

        return result;
    }
}
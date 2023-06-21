using ErrorOr;

using MediatR;

using MyDomain.Application.Services.Common;

namespace MyDomain.Application.Services.Commands.UpdateMyDomain;

public record UpdateMyDomainCommand(
    Guid Id,
    string Name,
    string Description) : IRequest<ErrorOr<MyDomainResult>>;
using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Models;

namespace MyDomain.Application.Services.Commands.CreateMyDomain;

public record CreateMyDomainCommand(
    string Name,
    string Description) : IRequest<ErrorOr<MyDomainResult>>;
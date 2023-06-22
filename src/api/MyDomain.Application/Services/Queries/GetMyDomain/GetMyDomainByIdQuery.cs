using ErrorOr;

using MediatR;

using MyDomain.Application.Common.Models;

namespace MyDomain.Application.Services.Queries;

public record GetMyDomainByIdQuery(Guid Id) : IRequest<ErrorOr<MyDomainResult>>;
using ErrorOr;

using MediatR;

using MyDomain.Application.Services.Common;

namespace MyDomain.Application.Services.Queries;

public record GetMyDomainByIdQuery(Guid Id) : IRequest<ErrorOr<MyDomainResult>>;
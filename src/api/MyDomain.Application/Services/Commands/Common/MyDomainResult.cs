namespace MyDomain.Application.Services.Commands.Common;

public record MyDomainResult(
    Guid Id,
    string Name,
    string Description,
    DateTime Created,
    DateTime Updated);
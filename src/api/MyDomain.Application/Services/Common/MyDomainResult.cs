namespace MyDomain.Application.Services.Common;

public record MyDomainResult(
    Guid Id,
    string Name,
    string Description,
    DateTime Created,
    DateTime Updated);
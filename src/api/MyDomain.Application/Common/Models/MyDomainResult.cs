namespace MyDomain.Application.Common.Models;

public record MyDomainResult(
    Guid Id,
    string Name,
    string Description,
    DateTime Created,
    DateTime Updated);
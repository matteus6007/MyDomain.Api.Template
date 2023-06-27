namespace MyDomain.Application.Common.Models;

public record MyDomainResult(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedOn,
    DateTime UpdatedOn);
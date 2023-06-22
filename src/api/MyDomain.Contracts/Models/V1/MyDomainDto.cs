namespace MyDomain.Contracts.Models.V1;

public record MyDomainDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedOn,
    DateTime UpdatedOn);
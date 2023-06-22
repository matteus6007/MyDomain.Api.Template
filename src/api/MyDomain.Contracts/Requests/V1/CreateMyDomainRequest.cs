namespace MyDomain.Contracts.Requests.V1;

public record CreateMyDomainRequest(
    string Name,
    string? Description);
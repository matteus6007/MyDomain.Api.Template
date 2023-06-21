namespace MyDomain.Contracts.Requests.V1;

public record UpdateMyDomainRequest(
    string Name,
    string Description);
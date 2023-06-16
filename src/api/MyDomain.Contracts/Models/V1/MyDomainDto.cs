namespace MyDomain.Contracts.Models.V1;

public record MyDomainDto(Guid id, string name, string description, DateTime created, DateTime? updated);
namespace MyDomain.Domain.Common.Interfaces;

public interface IAggregateState<TId>
{
    TId Id { get; set; }
    int Version { get; set; }
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }
}
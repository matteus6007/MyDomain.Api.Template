namespace MyDomain.Domain.Common.Models.Messaging;

public class MessageEnvelope
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
    public Metadata Metadata { get; set; } = new();
    public object Message { get; protected set; }

    public MessageEnvelope(object message)
    {
        Message = message;
    }
}
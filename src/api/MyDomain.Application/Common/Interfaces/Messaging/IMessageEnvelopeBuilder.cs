using ErrorOr;

using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.Common.Models.Messaging;

namespace MyDomain.Application.Common.Interfaces.Messaging;

public interface IMessageEnvelopeBuilder
{
    Task<ErrorOr<MessageEnvelope>> CreateMessageEnvelopeAsync(IDomainEvent @event);
}

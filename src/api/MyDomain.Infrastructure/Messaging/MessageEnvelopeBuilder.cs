using ErrorOr;

using MyDomain.Application;
using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.Common.Models.Messaging;

namespace MyDomain.Infrastructure.Messaging;

public class MessageEnvelopeBuilder : IMessageEnvelopeBuilder
{
    public async Task<ErrorOr<MessageEnvelope>> CreateMessageEnvelopeAsync(IDomainEvent @event)
    {
        await Task.CompletedTask;

        var envelope = new MessageEnvelope(@event);

        return envelope;
    }
}
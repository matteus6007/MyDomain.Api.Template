using ErrorOr;

using MyDomain.Domain.Common.Interfaces;

namespace MyDomain.Application.Common.Interfaces.Messaging;

public interface IEventPublisher
{
    Task<ErrorOr<Success>> PublishAsync(IDomainEvent @event);
}

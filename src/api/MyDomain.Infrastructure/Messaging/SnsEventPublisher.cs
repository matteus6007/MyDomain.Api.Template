using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

using ErrorOr;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MyDomain.Application;
using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Infrastructure.Messaging.Options;

using Newtonsoft.Json;

using Polly;

namespace MyDomain.Infrastructure.Messaging;

public class SnsEventPublisher : IEventPublisher
{
    private readonly SnsOptions _options;
    private readonly IMessageEnvelopeBuilder _messageEnvelopeBuilder;
    private readonly IAmazonSimpleNotificationService _client;
    private readonly ILogger<SnsEventPublisher> _logger;

    public SnsEventPublisher(
        SnsOptions options,
        IMessageEnvelopeBuilder messageEnvelopeBuilder,
        IAmazonSimpleNotificationService client,
        ILogger<SnsEventPublisher> logger)
    {
        _options = options;
        _messageEnvelopeBuilder = messageEnvelopeBuilder;
        _client = client;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> PublishAsync(IDomainEvent @event)
    {
        if (string.IsNullOrEmpty(_options.TopicName))
        {
            return Error.Validation(description: "Topic Name has not been set");
        }

        var topicArn = await GetTopicArnAsync(_options.TopicName);

        if (string.IsNullOrEmpty(topicArn))
        {
            return Error.NotFound(description: $"Topic {_options.TopicName} cannot be found");
        }

        var envelope = await _messageEnvelopeBuilder.CreateMessageEnvelopeAsync(@event);

        if (envelope.IsError)
        {
            return envelope.FirstError;
        }

        var request = new PublishRequest
        {
            TopicArn = topicArn,
            MessageAttributes = envelope.Value.Metadata.MapToMessageAttributes(),
            Message = JsonConvert.SerializeObject(envelope.Value)
        };

        var type = @event.GetType();

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                _options.RetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) => _logger.LogError(exception, "Failed to publish event {Type} to topic {TopicArn}. Attempt: {RetryCount}", type, topicArn, retryCount));

        await retryPolicy.ExecuteAsync(() => _client.PublishAsync(request, CancellationToken.None));

        return Result.Success;
    }

    private async Task<string?> GetTopicArnAsync(string topicName)
    {
        var topic = await _client.FindTopicAsync(topicName);

        return topic?.TopicArn;
    }
}
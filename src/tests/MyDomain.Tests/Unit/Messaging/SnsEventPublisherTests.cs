using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

using ErrorOr;

using Microsoft.Extensions.Logging;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.Common.Models.Messaging;
using MyDomain.Infrastructure.Messaging;
using MyDomain.Infrastructure.Messaging.Options;

using NSubstitute;

using Shouldly;

namespace MyDomain.Tests.Unit.Messaging;

public class SnsEventPublisherTests
{
    private readonly IAmazonSimpleNotificationService _clientMock = Substitute.For<IAmazonSimpleNotificationService>();
    private readonly IMessageEnvelopeBuilder _messageEnvelopeBuilderMock = Substitute.For<IMessageEnvelopeBuilder>();
    private readonly ILogger<SnsEventPublisher> _loggerMock = Substitute.For<ILogger<SnsEventPublisher>>();

    [Fact]
    public async Task PublishAsync_WhenTopicNameIsEmpty_ThenShouldReturnError()
    {
        // Arrange
        var @event = new TestEvent();

        var options = new SnsOptions
        {
            ServiceUrlOverride = "http://localhost:4566",
            RetryCount = 1
        };

        var sut = SystemUnderTest(options);

        // Act
        var result = await sut.PublishAsync(@event);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
    }

    [Fact]
    public async Task PublishAsync_WhenTopicNotFound_ThenShouldReturnError()
    {
        // Arrange
        var @event = new TestEvent();

        GivenTopicDoesNotExist();

        var options = new SnsOptions
        {
            ServiceUrlOverride = "http://localhost:4566",
            TopicName = "test",
            RetryCount = 1
        };

        var sut = SystemUnderTest(options);

        // Act
        var result = await sut.PublishAsync(@event);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task PublishAsync_WhenPublishFails_ThenShouldBeRetried()
    {
        // Arrange
        var @event = new TestEvent();

        GivenMessageEnvelopeIsCreated(@event);
        GivenTopicExists();
        GivenPublishFailsButSucceedsOnSecondAttempt();

        var options = new SnsOptions
        {
            ServiceUrlOverride = "http://localhost:4566",
            TopicName = "test",
            RetryCount = 1
        };

        var sut = SystemUnderTest(options);

        // Act
        var result = await sut.PublishAsync(@event);

        // Assert
        result.IsError.ShouldBeFalse();

        ThenTopicShouldBeFound(options);
        ThenEventShouldBePublished();
    }

    private SnsEventPublisher SystemUnderTest(SnsOptions options)
    {
        return new SnsEventPublisher(
            options,
            _messageEnvelopeBuilderMock,
            _clientMock,
            _loggerMock);
    }

    private void GivenTopicDoesNotExist()
    {
        _clientMock                
            .FindTopicAsync(Arg.Any<string>())
            .Returns((Topic?)null);
    }

    private void GivenTopicExists()
    {
        var expectedTopic = new Topic
        {
            TopicArn = "arn:aws:sns:eu-west-1:000000000000:test"
        };

        _clientMock
            .FindTopicAsync(Arg.Any<string>())
            .Returns(expectedTopic);
    }

    private void GivenMessageEnvelopeIsCreated(TestEvent @event)
    {
        _messageEnvelopeBuilderMock
           .CreateMessageEnvelopeAsync(Arg.Any<IDomainEvent>())
           .Returns(new MessageEnvelope(@event));
    }

    private void GivenPublishFailsButSucceedsOnSecondAttempt()
    {
        _ = _clientMock
            .PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => { throw new Exception("Something went wrong"); },
                _ => new PublishResponse());
    }

    private void ThenTopicShouldBeFound(SnsOptions options)
    {
        _clientMock
            .Received(1)
            .FindTopicAsync(options.TopicName);
    }

    private void ThenEventShouldBePublished()
    {
        _clientMock
            .Received(2)
            .PublishAsync(Arg.Any<PublishRequest>(), Arg.Any<CancellationToken>());
    }
}
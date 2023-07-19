using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

using ErrorOr;

using Microsoft.Extensions.Logging;

using Moq;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Domain.Common.Interfaces;
using MyDomain.Domain.Common.Models.Messaging;
using MyDomain.Infrastructure.Messaging;
using MyDomain.Infrastructure.Messaging.Options;

using Shouldly;

namespace MyDomain.Tests.Unit.Messaging;

public class SnsEventPublisherTests
{
    private readonly Mock<IAmazonSimpleNotificationService> _clientMock = new();
    private readonly Mock<IMessageEnvelopeBuilder> _messageEnvelopeFactoryMock = new();
    private readonly Mock<ILogger<SnsEventPublisher>> _loggerMock = new();

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
            _messageEnvelopeFactoryMock.Object,
            _clientMock.Object,
            _loggerMock.Object);
    }

    private void GivenTopicDoesNotExist()
    {
        _clientMock
            .Setup(x => x.FindTopicAsync(It.IsAny<string>()))
            .ReturnsAsync((Topic?)null);
    }

    private void GivenTopicExists()
    {
        var expectedTopic = new Topic
        {
            TopicArn = "arn:aws:sns:eu-west-1:000000000000:test"
        };

        _clientMock
            .Setup(x => x.FindTopicAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedTopic);
    }

    private void GivenMessageEnvelopeIsCreated(TestEvent @event)
    {
        _messageEnvelopeFactoryMock
           .Setup(x => x.CreateMessageEnvelopeAsync(It.IsAny<IDomainEvent>()))
           .ReturnsAsync(new MessageEnvelope(@event));
    }

    private void GivenPublishFailsButSucceedsOnSecondAttempt()
    {
        _clientMock
            .SetupSequence(x => x.PublishAsync(It.IsAny<PublishRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong"))
            .ReturnsAsync(new PublishResponse());
    }

    private void ThenTopicShouldBeFound(SnsOptions options)
    {
        _clientMock
            .Verify(x => x.FindTopicAsync(options.TopicName), Times.Once);
    }

    private void ThenEventShouldBePublished()
    {
        _clientMock
            .Verify(x => x.PublishAsync(It.IsAny<PublishRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
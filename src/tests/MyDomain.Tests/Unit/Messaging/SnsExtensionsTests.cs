using MyDomain.Domain.Common.Models.Messaging;
using MyDomain.Infrastructure.Messaging;

using Shouldly;

namespace MyDomain.Tests.Unit.Messaging;

public class SnsExtensionsTests
{
    [Theory]
    [InlineData("Key", "Value", "key", "value")]
    [InlineData("KEY", "VALUE", "key", "value")]
    public void WhenMappingMessageAttributes_ThenShouldReturnMappedAttributes(
        string key,
        string value,
        string expectedKey,
        string expectedValue)
    {
        // Arrange
        var metadata = new Metadata();
        metadata.AddAttribute(key, value);

        // Act
        var result = metadata.MapToMessageAttributes();

        // Assert
        result.ContainsKey(expectedKey).ShouldBeTrue();
        result[expectedKey].DataType.ShouldBe("String");
        result[expectedKey].StringValue.ShouldBe(expectedValue);
    }
}
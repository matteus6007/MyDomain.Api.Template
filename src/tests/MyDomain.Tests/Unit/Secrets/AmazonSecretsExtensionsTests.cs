using AutoFixture.Xunit2;

using MyDomain.Infrastructure.Secrets;

namespace MyDomain.Tests.Unit.Secrets
{
    public class AmazonSecretsExtensionsTests
    {
        private const string SecretName = "Test-Secret";

        [Theory]
        [InlineData("{\"settings\":{\"values\":{\"string\":\"some string\",\"true\":true,\"false\":false,\"number\":0}}}", "settings:values")]
        public void WhenParsingSecret_AndSecretIsComplexJson_ThenShouldReturnDictionary(string secret, string expectedKeyPrefix)
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, secret);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "string")));
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "true")));
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "false")));
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "number")));
        }

        [Theory]
        [AutoData]
        public void WhenParsingSecret_AndSecretIsAString_TheShouldReturnDictionary(string secret)
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, secret);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(SecretName));
        }

        [Theory]
        [InlineData("[\"Item 1\", \"Item 2\"]")]
        public void WhenParsingSecret_AndSecretIsAnArray_ThenShouldReturnDictionary(string secret)
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, secret);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Theory]
        [InlineData("{\"settings\":null}")]
        public void WhenParsingSecret_AndSecretIsComplexJson_AndPropertyIsNull_ThenShouldThrowFormatException(string secret)
        {
            Assert.Throws<FormatException>(() => AmazonSecretsExtensions.ParseSecret(SecretName, secret));
        }

        [Theory]
        [InlineData("{\"settings\"}")]
        public void WhenParsingSecret_AndSecretIsComplexJson_AndJsonIsNotValid_ThenShouldReturnDictionary(string secret)
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, secret);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(SecretName));
        }

        [Fact]
        public void WhenParsingSecret_AndSecretIsNull_ThenShouldReturnEmptyDictionary()
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
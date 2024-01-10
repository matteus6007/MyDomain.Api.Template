using AutoFixture.Xunit2;

using MyDomain.Infrastructure.Secrets;

namespace MyDomain.Tests.Unit.Secrets
{
    public class AmazonSecretsExtensionsTests
    {
        private const string SecretName = "Test-Secret";

        [Theory]
        [InlineData("{\"mydomain\":{\"database\":{\"password\":\"password123\",\"user\":\"root\"}}}", "mydomain:database")]
        public void WhenParsingSecret_AndSecretIsComplexJson_ThenShouldReturnDictionary(string secret, string expectedKeyPrefix)
        {
            // Act
            var result = AmazonSecretsExtensions.ParseSecret(SecretName, secret);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "password")));
            Assert.True(result.ContainsKey(string.Join(":", expectedKeyPrefix, "user")));
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

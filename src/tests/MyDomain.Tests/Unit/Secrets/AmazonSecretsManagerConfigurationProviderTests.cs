using Amazon.SecretsManager.Extensions.Caching;
using Amazon.SecretsManager.Model;

using AutoFixture.Xunit2;

using Microsoft.Extensions.Configuration;

using MyDomain.Infrastructure.Persistence.Options;
using MyDomain.Infrastructure.Secrets;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyDomain.Tests.Unit.Secrets
{
    public class AmazonSecretsManagerConfigurationProviderTests
    {
        private readonly ISecretsManagerCache _cache = Substitute.For<ISecretsManagerCache>();

        [Theory]
        [InlineAutoData("{\"MyDomain\":{\"Database\":{\"Password\":\"password123\",\"User\":\"root\"}}}")]
        public void GivenSecretExists_WhenIGetSection_AndSectionIsFound_ThenSectionShouldBeReturned(string secret, string secretKey)
        {
            // Arrange
            GivenSecretExists(secret, secretKey);

            IConfigurationRoot configuration = GivenSecretsManagerIsConfiguredForSecret(secretKey);

            // Act
            DatabaseOptions? result = WhenIGetSection(configuration);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("root", result.User);
            Assert.Equal("password123", result.Password);
        }

        [Theory]
        [InlineAutoData("{\"MyDomain\":{}}")]
        public void GivenSecretExists_WhenIGetSection_AndSectionIsNotFound_ThenShouldReturnNull(string secret, string secretKey)
        {
            // Arrange
            GivenSecretExists(secret, secretKey);

            IConfigurationRoot configuration = GivenSecretsManagerIsConfiguredForSecret(secretKey);

            // Act
            DatabaseOptions? result = WhenIGetSection(configuration);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [AutoData]
        public void GivenSecretDoesNotExist_WhenIGetSection_ThenShouldThrowException(string secretKey)
        {
            // Arrange
            GivenSecretDoesNotExist(secretKey);

            Assert.Throws<ResourceNotFoundException>(() => GivenSecretsManagerIsConfiguredForSecret(secretKey));
        }

        private void GivenSecretExists(string secret, string secretKey)
        {
            _cache
                .GetSecretString(secretKey)
                .Returns(secret);
        }

        private void GivenSecretDoesNotExist(string secretKey)
        {
            _cache
                .GetSecretString(secretKey)
                .Throws(new ResourceNotFoundException($"Secret {secretKey} not found"));
        }

        private IConfigurationRoot GivenSecretsManagerIsConfiguredForSecret(string secretKey)
        {
            var configurationSource = new AmazonSecretsManagerConfigurationSource(_cache, secretKey);

            var builder = new ConfigurationBuilder();
            builder.Add(configurationSource);

            return builder.Build();
        }

        private static DatabaseOptions? WhenIGetSection(IConfigurationRoot configuration)
        {
            return configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();
        }
    }
}
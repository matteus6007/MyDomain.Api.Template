using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;

using Microsoft.Extensions.Configuration;

using MyDomain.Infrastructure.Extensions;
using MyDomain.Infrastructure.Secrets.Options;

namespace MyDomain.Infrastructure.Secrets;

public static class DependencyInjection
{
    public static void AddAmazonSecretsManager(
        this IConfigurationBuilder configurationBuilder,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(AmazonSecretsManagerOptions.SectionName).Get<AmazonSecretsManagerOptions>();

        IAmazonSecretsManager client;

        if (!string.IsNullOrWhiteSpace(options?.ServiceUrlOverride))
        {
            var clientConfig = new AmazonSecretsManagerConfig
            {
                ServiceURL = options.ServiceUrlOverride,
                UseHttp = true
            };

            client = new AmazonSecretsManagerClient(AwsExtensions.GetTestCredentials(), clientConfig);
        }
        else
        {
            client = new AmazonSecretsManagerClient();
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var configurationSource = new AmazonSecretsManagerConfigurationSource(new SecretsManagerCache(client), options.SecretsKey);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        configurationBuilder.Add(configurationSource);
    }
}
using Amazon.SecretsManager.Extensions.Caching;

using Microsoft.Extensions.Configuration;

namespace MyDomain.Infrastructure.Secrets;

public class AmazonSecretsManagerConfigurationSource(ISecretsManagerCache cache, string secretName) : IConfigurationSource
{
    private readonly ISecretsManagerCache _cache = cache;
    private readonly string _secretName = secretName;

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider(_cache, _secretName);
    }
}
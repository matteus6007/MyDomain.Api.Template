using Amazon.SecretsManager.Extensions.Caching;

using Microsoft.Extensions.Configuration;

namespace MyDomain.Infrastructure.Secrets;

public class AmazonSecretsManagerConfigurationProvider(ISecretsManagerCache cache, string secretName) : ConfigurationProvider
{
    private readonly ISecretsManagerCache _cache = cache;
    private readonly string _secretName = secretName;

    public override void Load()
    {
        var secret = GetSecretAsync(default).GetAwaiter().GetResult();

        Data = AmazonSecretsExtensions.ParseSecret(_secretName, secret);
    }

    private async Task<string> GetSecretAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetSecretString(_secretName, cancellationToken).ConfigureAwait(false);
    }
}

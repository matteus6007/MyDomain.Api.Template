namespace MyDomain.Infrastructure.Secrets.Options;

public class AmazonSecretsManagerOptions
{
    public const string SectionName = "MyDomain:Secrets";

    public string ServiceUrlOverride { get; set; } = "";
    public required string SecretsKey { get; set; }
}
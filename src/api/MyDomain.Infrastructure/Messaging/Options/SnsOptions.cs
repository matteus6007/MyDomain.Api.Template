namespace MyDomain.Infrastructure.Messaging.Options;

public class SnsOptions
{
    public const string SectionName = "MyDomain:Sns";

    public string ServiceUrlOverride { get; set; } = "";
    public string TopicName { get; set; } = "";
    public int RetryCount { get; set; } = 0;
}

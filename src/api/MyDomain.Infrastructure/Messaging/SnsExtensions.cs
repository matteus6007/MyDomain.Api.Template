using Amazon.SimpleNotificationService.Model;

using MyDomain.Domain.Common.Models.Messaging;

namespace MyDomain.Infrastructure.Messaging;

public static class SnsExtensions
{
    public static Dictionary<string, MessageAttributeValue> MapToMessageAttributes(this Metadata metadata)
    {
        var attributes = new Dictionary<string, MessageAttributeValue>();

        foreach (var (key, value) in metadata.Attributes)
        {
            attributes.Add(key.ToLowerInvariant(), new MessageAttributeValue { DataType = "String", StringValue = value.ToLowerInvariant() });
        }

        return attributes;
    }
}

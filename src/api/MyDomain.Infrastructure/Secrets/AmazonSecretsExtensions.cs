using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace MyDomain.Infrastructure.Secrets;

public static class AmazonSecretsExtensions
{
    public static Dictionary<string, string?> ParseSecret(string secretName, string secret)
    {
        var configuration = new Dictionary<string, string?>();

        if (string.IsNullOrWhiteSpace(secret))
        {
            return configuration;
        }

        if (TryParseJson(secret, out var jElement))
        {
            var values = ExtractValues(jElement!, string.Empty);

            foreach (var (key, value) in values)
            {
                configuration.TryAdd(key, value);
            }
        }
        else
        {
            configuration.TryAdd(secretName, secret);
        }

        return configuration;
    }

    private static bool TryParseJson(string data, out JsonElement? jsonElement)
    {
        jsonElement = null;

        data = data.TrimStart();
        var firstChar = data.FirstOrDefault();

        if (firstChar != '[' && firstChar != '{')
        {
            return false;
        }

        try
        {
            using var jsonDocument = JsonDocument.Parse(data);
            //  https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-use-dom-utf8jsonreader-utf8jsonwriter?pivots=dotnet-6-0#jsondocument-is-idisposable
            //  Its recommended to return the clone of the root element as the json document will be disposed
            jsonElement = jsonDocument.RootElement.Clone();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static IEnumerable<(string key, string value)> ExtractValues(JsonElement? jsonElement, string prefix)
    {
        if (jsonElement == null)
        {
            yield break;
        }
        var element = jsonElement.Value;
        switch (element.ValueKind)
        {
            case JsonValueKind.Array:
                {
                    var currentIndex = 0;
                    foreach (var el in element.EnumerateArray())
                    {
                        var secretKey = string.IsNullOrEmpty(prefix) ? currentIndex.ToString() : $"{prefix}{ConfigurationPath.KeyDelimiter}{currentIndex}";
                        foreach (var (key, value) in ExtractValues(el, secretKey))
                        {
                            yield return (key, value);
                        }
                        currentIndex++;
                    }
                    break;
                }
            case JsonValueKind.Number:
                {
                    var value = element.GetRawText();
                    yield return (prefix, value);
                    break;
                }
            case JsonValueKind.String:
                {
                    var value = element.GetString() ?? "";
                    yield return (prefix, value);
                    break;
                }
            case JsonValueKind.True:
                {
                    var value = element.GetBoolean();
                    yield return (prefix, value.ToString());
                    break;
                }
            case JsonValueKind.False:
                {
                    var value = element.GetBoolean();
                    yield return (prefix, value.ToString());
                    break;
                }
            case JsonValueKind.Object:
                {
                    foreach (var property in element.EnumerateObject())
                    {
                        var secretKey = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}{ConfigurationPath.KeyDelimiter}{property.Name}";
                        foreach (var (key, value) in ExtractValues(property.Value, secretKey))
                        {
                            yield return (key, value);
                        }
                    }
                    break;
                }
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
            default:
                {
                    throw new FormatException("unsupported json token");
                }
        }
    }
}
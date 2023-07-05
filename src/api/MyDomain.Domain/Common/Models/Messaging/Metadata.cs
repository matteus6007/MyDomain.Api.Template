namespace MyDomain.Domain.Common.Models.Messaging;

public class Metadata
{
    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => _attributes;

    public void AddAttribute(string key, string value)
    {
        _attributes.Remove(key);
        _attributes.Add(key, value);
    }
}

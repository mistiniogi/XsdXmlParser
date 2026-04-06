using System.Text.Json;
using System.Text.Json.Serialization;

namespace XsdXmlParser.Core.Serialization.Converters;

/// <summary>
/// Serializes string-based reference identifiers as JSON string values.
/// </summary>
public sealed class RefIdJsonConverter : JsonConverter<string>
{
    /// <inheritdoc/>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
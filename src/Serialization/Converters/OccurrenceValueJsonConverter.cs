using System.Text.Json;
using System.Text.Json.Serialization;

namespace XsdXmlParser.Core.Serialization.Converters;

/// <summary>
/// Serializes occurrence markers such as numeric values or the unbounded sentinel.
/// </summary>
public sealed class OccurrenceValueJsonConverter : JsonConverter<string>
{
    /// <inheritdoc/>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Number
            ? reader.GetInt32().ToString(System.Globalization.CultureInfo.InvariantCulture)
            : reader.GetString();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (int.TryParse(value, out var numericValue))
        {
            writer.WriteNumberValue(numericValue);
            return;
        }

        writer.WriteStringValue(value);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Serialization.Converters;

/// <summary>
/// Serializes normalized constraint-set payloads while preserving the compact occurrence and rule-shape contract used by graph JSON output.
/// </summary>
public sealed class ConstraintSetJsonConverter : JsonConverter<ConstraintSetModel>
{
    /// <inheritdoc/>
    public override ConstraintSetModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<ConstraintSetModel>(ref reader, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ConstraintSetModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(ConstraintSetModel.RuleId), value.RuleId);
        writer.WriteString(nameof(ConstraintSetModel.OwnerRefId), value.OwnerRefId);

        if (value.MinOccurs.HasValue)
        {
            writer.WriteNumber(nameof(ConstraintSetModel.MinOccurs), value.MinOccurs.Value);
        }

        if (!string.IsNullOrWhiteSpace(value.MaxOccurs))
        {
            writer.WriteString(nameof(ConstraintSetModel.MaxOccurs), value.MaxOccurs);
        }

        if (!string.IsNullOrWhiteSpace(value.Pattern))
        {
            writer.WriteString(nameof(ConstraintSetModel.Pattern), value.Pattern);
        }

        if (!string.IsNullOrWhiteSpace(value.BaseTypeRefId))
        {
            writer.WriteString(nameof(ConstraintSetModel.BaseTypeRefId), value.BaseTypeRefId);
        }

        writer.WritePropertyName(nameof(ConstraintSetModel.ValueBounds));
        JsonSerializer.Serialize(writer, value.ValueBounds, options);

        writer.WritePropertyName(nameof(ConstraintSetModel.EnumerationValues));
        JsonSerializer.Serialize(writer, value.EnumerationValues, options);

        writer.WritePropertyName(nameof(ConstraintSetModel.SerializerShape));
        JsonSerializer.Serialize(writer, value.SerializerShape, options);
        writer.WriteEndObject();
    }
}
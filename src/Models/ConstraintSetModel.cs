using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents normalized validation and generation-oriented rule metadata.
/// </summary>
public sealed class ConstraintSetModel
{
    /// <summary>
    /// Gets or sets the rule identifier.
    /// </summary>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning entry reference identifier.
    /// </summary>
    public string OwnerRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional minimum occurrence.
    /// </summary>
    public int? MinOccurs { get; set; }

    /// <summary>
    /// Gets or sets the optional maximum occurrence marker.
    /// </summary>
    public string? MaxOccurs { get; set; }

    /// <summary>
    /// Gets or sets the optional pattern restriction.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the optional base type reference identifier.
    /// </summary>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the optional value bounds.
    /// </summary>
    public Dictionary<string, string> ValueBounds { get; } = new();

    /// <summary>
    /// Gets the optional enumeration values.
    /// </summary>
    public List<string> EnumerationValues { get; } = new();

    /// <summary>
    /// Gets the serializer shape hints.
    /// </summary>
    public Dictionary<string, string> SerializerShape { get; } = new();
}
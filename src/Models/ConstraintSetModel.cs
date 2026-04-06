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
    /// <value>The stable identifier for the normalized constraint set.</value>
    public string RuleId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning entry reference identifier.
    /// </summary>
    /// <value>The canonical reference identifier of the graph entry that owns the rule set.</value>
    public string OwnerRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional minimum occurrence.
    /// </summary>
    /// <value>The minimum occurrence constraint when one is present.</value>
    public int? MinOccurs { get; set; }

    /// <summary>
    /// Gets or sets the optional maximum occurrence marker.
    /// </summary>
    /// <value>The maximum occurrence marker, which may contain values such as a numeric limit or <c>unbounded</c>.</value>
    public string? MaxOccurs { get; set; }

    /// <summary>
    /// Gets or sets the optional pattern restriction.
    /// </summary>
    /// <value>The pattern restriction expressed for the owning entry, when one is available.</value>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the optional base type reference identifier.
    /// </summary>
    /// <value>The canonical base type reference identifier used by the rule set, when applicable.</value>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the optional value bounds.
    /// </summary>
    /// <value>The normalized value-bound metadata keyed by bound name.</value>
    public Dictionary<string, string> ValueBounds { get; } = new();

    /// <summary>
    /// Gets the optional enumeration values.
    /// </summary>
    /// <value>The explicit enumeration values attached to the owning rule set.</value>
    public List<string> EnumerationValues { get; } = new();

    /// <summary>
    /// Gets the serializer shape hints.
    /// </summary>
    /// <value>The serializer shape hints that help downstream JSON consumers interpret constrained values.</value>
    public Dictionary<string, string> SerializerShape { get; } = new();
}
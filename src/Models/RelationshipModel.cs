namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a normalized relationship between graph entries.
/// </summary>
public sealed class RelationshipModel
{
    /// <summary>
    /// Gets or sets the relationship identifier.
    /// </summary>
    /// <value>The stable identifier for the normalized relationship record.</value>
    public string RelationshipId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source reference identifier.
    /// </summary>
    /// <value>The canonical reference identifier from which the relationship originates.</value>
    public string FromRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target reference identifier.
    /// </summary>
    /// <value>The canonical reference identifier to which the relationship points.</value>
    public string ToRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship kind.
    /// </summary>
    /// <value>The normalized relationship kind that explains how the two entries are connected.</value>
    public ERelationshipKind RelationshipKind { get; set; }

    /// <summary>
    /// Gets or sets the optional order index.
    /// </summary>
    /// <value>The order index used when relationship order is significant.</value>
    public int? OrderIndex { get; set; }

    /// <summary>
    /// Gets or sets the parsing pass in which the relationship becomes final.
    /// </summary>
    /// <value>The parsing pass label that indicates when the relationship becomes stable.</value>
    public string PassAssigned { get; set; } = string.Empty;
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a normalized relationship between graph entries.
/// </summary>
public sealed class RelationshipModel
{
    /// <summary>
    /// Gets or sets the relationship identifier.
    /// </summary>
    public string RelationshipId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source reference identifier.
    /// </summary>
    public string FromRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target reference identifier.
    /// </summary>
    public string ToRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the relationship kind.
    /// </summary>
    public ERelationshipKind RelationshipKind { get; set; }

    /// <summary>
    /// Gets or sets the optional order index.
    /// </summary>
    public int? OrderIndex { get; set; }

    /// <summary>
    /// Gets or sets the parsing pass in which the relationship becomes final.
    /// </summary>
    public string PassAssigned { get; set; } = string.Empty;
}
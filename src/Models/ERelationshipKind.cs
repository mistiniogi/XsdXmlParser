namespace XsdXmlParser.Core.Models;

/// <summary>
/// Identifies the normalized relationship type recorded between canonical graph entries.
/// </summary>
public enum ERelationshipKind
{
    /// <summary>
    /// Indicates that one source imports another source.
    /// </summary>
    Import,

    /// <summary>
    /// Indicates that one source includes another source.
    /// </summary>
    Include,

    /// <summary>
    /// Indicates that a parent entry contains a child entry.
    /// </summary>
    Contains,

    /// <summary>
    /// Indicates that an entry references another canonical entry.
    /// </summary>
    References,

    /// <summary>
    /// Indicates that an entry derives from another canonical entry.
    /// </summary>
    DerivesFrom,

    /// <summary>
    /// Indicates that an attribute belongs to an owning entry.
    /// </summary>
    AttributeOf,
}
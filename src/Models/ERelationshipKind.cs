namespace XsdXmlParser.Core.Models;

/// <summary>
/// Identifies the normalized relationship type between graph entries.
/// </summary>
public enum ERelationshipKind
{
    /// <summary>
    /// The source imports another source.
    /// </summary>
    Import,

    /// <summary>
    /// The source includes another source.
    /// </summary>
    Include,

    /// <summary>
    /// A parent contains a child entry.
    /// </summary>
    Contains,

    /// <summary>
    /// An entry references another entry.
    /// </summary>
    References,

    /// <summary>
    /// An entry derives from another entry.
    /// </summary>
    DerivesFrom,

    /// <summary>
    /// An attribute belongs to an owning entry.
    /// </summary>
    AttributeOf,
}
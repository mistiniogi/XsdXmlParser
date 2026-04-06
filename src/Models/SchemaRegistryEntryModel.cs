namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents lifecycle metadata for a canonical registry entry.
/// </summary>
public sealed class SchemaRegistryEntryModel
{
    /// <summary>
    /// Gets or sets the canonical reference identifier.
    /// </summary>
    /// <value>The canonical reference identifier for the entry being tracked.</value>
    public string RefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entry kind name.
    /// </summary>
    /// <value>The logical entry kind, such as type, element, or attribute.</value>
    public string EntryKind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    /// <value>The source identifier that owns the tracked entry.</value>
    public string OwningSourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current discovery state.
    /// </summary>
    /// <value>The current discovery lifecycle state for the entry.</value>
    public string DiscoveryState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional conflict hash used for duplicate analysis.
    /// </summary>
    /// <value>The duplicate-analysis hash used to detect conflicting definitions, when applicable.</value>
    public string? ConflictHash { get; set; }
}
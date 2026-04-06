namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents lifecycle metadata for a canonical registry entry.
/// </summary>
public sealed class SchemaRegistryEntryModel
{
    /// <summary>
    /// Gets or sets the canonical reference identifier.
    /// </summary>
    public string RefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entry kind name.
    /// </summary>
    public string EntryKind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    public string OwningSourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current discovery state.
    /// </summary>
    public string DiscoveryState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional conflict hash used for duplicate analysis.
    /// </summary>
    public string? ConflictHash { get; set; }
}
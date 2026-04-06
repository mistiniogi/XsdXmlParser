namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents the shared fields for exported registry entries.
/// </summary>
public class RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the canonical reference identifier.
    /// </summary>
    public string RefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the qualified name when available.
    /// </summary>
    public string? QualifiedName { get; set; }

    /// <summary>
    /// Gets or sets the canonical schema path for diagnostics.
    /// </summary>
    public string SchemaPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent reference identifier for local definitions.
    /// </summary>
    public string? ParentRefId { get; set; }

    /// <summary>
    /// Gets or sets the optional documentation summary.
    /// </summary>
    public string? Documentation { get; set; }
}
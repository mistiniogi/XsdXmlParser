namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents the shared fields for exported registry entries.
/// </summary>
public class RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the canonical reference identifier.
    /// </summary>
    /// <value>The canonical reference identifier used to address the entry within the normalized metadata graph.</value>
    public string RefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    /// <value>The identifier of the source that originally contributed the entry.</value>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the qualified name when available.
    /// </summary>
    /// <value>The qualified schema name associated with the entry, when one exists.</value>
    public string? QualifiedName { get; set; }

    /// <summary>
    /// Gets or sets the canonical schema path for diagnostics.
    /// </summary>
    /// <value>The canonical schema path used for diagnostics and duplicate analysis.</value>
    public string SchemaPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent reference identifier for local definitions.
    /// </summary>
    /// <value>The parent reference identifier when the entry represents a local or inline definition.</value>
    public string? ParentRefId { get; set; }

    /// <summary>
    /// Gets or sets the optional documentation summary.
    /// </summary>
    /// <value>The summary documentation extracted from the source schema or WSDL artifact.</value>
    public string? Documentation { get; set; }
}
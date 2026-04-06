namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents one normalized parser source after request loading and source identity assignment.
/// </summary>
public sealed class SourceDescriptorModel
{
    /// <summary>
    /// Gets or sets the caller-declared document kind.
    /// </summary>
    /// <value>The document kind that controls downstream discovery and parsing behavior.</value>
    public ESchemaDocumentKind DocumentKind { get; set; }

    /// <summary>
    /// Gets or sets the stable logical source identifier.
    /// </summary>
    /// <value>The canonical source identifier used by the registry and metadata graph.</value>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized source origin kind.
    /// </summary>
    /// <value>The origin kind that indicates whether the source came from a file, string, or other supported input form.</value>
    public ESourceKind SourceKind { get; set; }

    /// <summary>
    /// Gets or sets the display name used in diagnostics.
    /// </summary>
    /// <value>The name surfaced in diagnostics and review output.</value>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized virtual path.
    /// </summary>
    /// <value>The normalized virtual path used for import, include, and diagnostic correlation.</value>
    public string VirtualPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the logical relative path used for import/include resolution.
    /// </summary>
    /// <value>The logical relative path used to resolve references from this source.</value>
    public string? RelativePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the source is the designated main source.
    /// </summary>
    /// <value><see langword="true"/> when the source is the caller-designated entry point for the parse request.</value>
    public bool IsMainSource { get; set; }

    /// <summary>
    /// Gets or sets the optional content fingerprint for duplicate analysis.
    /// </summary>
    /// <value>The content fingerprint used to detect duplicate or conflicting source content.</value>
    public string? ContentFingerprint { get; set; }

    /// <summary>
    /// Gets or sets the logical source name supplied by the caller.
    /// </summary>
    /// <value>The caller-supplied logical name preserved for diagnostics and display.</value>
    public string LogicalName { get; set; } = string.Empty;
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents one normalized parser source.
/// </summary>
public sealed class SourceDescriptorModel
{
    /// <summary>
    /// Gets or sets the caller-declared document kind.
    /// </summary>
    public ESchemaDocumentKind DocumentKind { get; set; }

    /// <summary>
    /// Gets or sets the stable logical source identifier.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized source origin kind.
    /// </summary>
    public ESourceKind SourceKind { get; set; }

    /// <summary>
    /// Gets or sets the display name used in diagnostics.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the normalized virtual path.
    /// </summary>
    public string VirtualPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the logical relative path used for import/include resolution.
    /// </summary>
    public string? RelativePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the source is the designated main source.
    /// </summary>
    public bool IsMainSource { get; set; }

    /// <summary>
    /// Gets or sets the optional content fingerprint for duplicate analysis.
    /// </summary>
    public string? ContentFingerprint { get; set; }

    /// <summary>
    /// Gets or sets the logical source name supplied by the caller.
    /// </summary>
    public string LogicalName { get; set; } = string.Empty;
}
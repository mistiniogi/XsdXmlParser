namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a source as resolved through the virtual file system.
/// </summary>
public sealed class VirtualFileModel
{
    /// <summary>
    /// Gets or sets the normalized virtual path.
    /// </summary>
    public string VirtualPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the source is memory-backed.
    /// </summary>
    public bool IsMemoryBacked { get; set; }

    /// <summary>
    /// Gets or sets the optional content length metadata.
    /// </summary>
    public long? ContentLength { get; set; }

    /// <summary>
    /// Gets or sets the source provenance used by import/include diagnostics.
    /// </summary>
    public string? Provenance { get; set; }

    /// <summary>
    /// Gets or sets the main-source context captured at resolution time.
    /// </summary>
    public string? MainSourceId { get; set; }
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a source as resolved through the virtual file system.
/// </summary>
public sealed class VirtualFileModel
{
    /// <summary>
    /// Gets or sets the normalized virtual path.
    /// </summary>
    /// <value>The normalized virtual path used for downstream import, include, and diagnostic resolution.</value>
    public string VirtualPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the owning source identifier.
    /// </summary>
    /// <value>The source identifier associated with the resolved virtual file.</value>
    public string SourceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the source is memory-backed.
    /// </summary>
    /// <value><see langword="true"/> when the resolved source originated from in-memory content rather than a physical file.</value>
    public bool IsMemoryBacked { get; set; }

    /// <summary>
    /// Gets or sets the optional content length metadata.
    /// </summary>
    /// <value>The content length, when the underlying source can report it.</value>
    public long? ContentLength { get; set; }

    /// <summary>
    /// Gets or sets the source provenance used by import/include diagnostics.
    /// </summary>
    /// <value>The provenance text used to explain where the resolved source originated.</value>
    public string? Provenance { get; set; }

    /// <summary>
    /// Gets or sets the main-source context captured at resolution time.
    /// </summary>
    /// <value>The primary entry-point source identifier associated with the current resolution flow.</value>
    public string? MainSourceId { get; set; }
}
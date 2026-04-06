namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents one caller-supplied source entry in a batch parse request.
/// </summary>
public sealed class BatchSourceRequestModel
{
    /// <summary>
    /// Gets or sets the caller-declared document kind.
    /// </summary>
    public ESchemaDocumentKind DocumentKind { get; set; }

    /// <summary>
    /// Gets or sets the logical name used for source identity and matching.
    /// </summary>
    public string LogicalName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the logical path used for relative import/include resolution.
    /// </summary>
    public string LogicalPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stream content for the source.
    /// </summary>
    public Stream Content { get; set; } = Stream.Null;

    /// <summary>
    /// Gets or sets a value indicating whether the source is the designated main source.
    /// </summary>
    public bool IsMain { get; set; }
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents one actionable parser or resolution diagnostic.
/// </summary>
public sealed class ParseDiagnosticModel
{
    /// <summary>
    /// Gets or sets the diagnostic identifier.
    /// </summary>
    public string DiagnosticId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the associated source identifier.
    /// </summary>
    public string? SourceId { get; set; }

    /// <summary>
    /// Gets or sets the associated virtual path.
    /// </summary>
    public string? VirtualPath { get; set; }

    /// <summary>
    /// Gets or sets the processing stage.
    /// </summary>
    public string Stage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stable diagnostic code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the actionable diagnostic message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional extended diagnostic details.
    /// </summary>
    public string? Details { get; set; }
}
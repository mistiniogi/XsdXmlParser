namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents one actionable parser, discovery, or resolution diagnostic.
/// </summary>
public sealed class ParseDiagnosticModel
{
    /// <summary>
    /// Gets or sets the diagnostic identifier.
    /// </summary>
    /// <value>The stable identifier for the diagnostic entry.</value>
    public string DiagnosticId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the associated source identifier.
    /// </summary>
    /// <value>The associated source identifier, when the diagnostic can be tied to a specific source.</value>
    public string? SourceId { get; set; }

    /// <summary>
    /// Gets or sets the associated virtual path.
    /// </summary>
    /// <value>The virtual path associated with the diagnostic, when available.</value>
    public string? VirtualPath { get; set; }

    /// <summary>
    /// Gets or sets the processing stage.
    /// </summary>
    /// <value>The high-level processing stage that emitted the diagnostic.</value>
    public string Stage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stable diagnostic code.
    /// </summary>
    /// <value>The stable code used to group similar diagnostic conditions.</value>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the actionable diagnostic message.
    /// </summary>
    /// <value>The actionable message intended for callers and maintainers.</value>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets optional extended diagnostic details.
    /// </summary>
    /// <value>The extended diagnostic details, such as exception information or contextual notes.</value>
    public string? Details { get; set; }
}
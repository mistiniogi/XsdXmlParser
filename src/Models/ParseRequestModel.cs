namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents the shared fields for one consumer-supplied parse request.
/// </summary>
public abstract class ParseRequestModel
{
    /// <summary>
    /// Gets or sets the caller-declared document kind.
    /// </summary>
    public ESchemaDocumentKind DocumentKind { get; set; }

    /// <summary>
    /// Gets or sets the optional display name used in diagnostics.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the optional logical path used for resolution.
    /// </summary>
    public string? LogicalPath { get; set; }
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents the shared fields for one consumer-supplied parse request.
/// </summary>
public abstract class ParseRequestModel
{
    /// <summary>
    /// Gets or sets the caller-declared document kind for the primary request-model workflow.
    /// </summary>
    public ESchemaDocumentKind DocumentKind { get; set; }

    /// <summary>
    /// Gets or sets the optional display name used in diagnostics.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the logical path used for resolution.
    /// </summary>
    /// <value>
    /// The file-backed request may omit this value to fall back to the physical file path, while
    /// string-backed requests must provide it so relative references can resolve consistently.
    /// </value>
    public string? LogicalPath { get; set; }
}
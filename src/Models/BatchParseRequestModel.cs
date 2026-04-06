using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a coordinated multi-source parse request.
/// </summary>
public sealed class BatchParseRequestModel
{
    /// <summary>
    /// Gets or sets the logical sources that participate in the request.
    /// </summary>
    public IReadOnlyList<BatchSourceRequestModel> Sources { get; set; } = Array.Empty<BatchSourceRequestModel>();
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a string-backed parse request.
/// </summary>
public sealed class StringParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the XML content to parse.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
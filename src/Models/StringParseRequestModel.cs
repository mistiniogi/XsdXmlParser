namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a parse request whose primary source is provided directly as XML content.
/// </summary>
public sealed class StringParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the XML content to parse.
    /// </summary>
    /// <value>The XML content that should be parsed as the primary source.</value>
    public string Content { get; set; } = string.Empty;
}
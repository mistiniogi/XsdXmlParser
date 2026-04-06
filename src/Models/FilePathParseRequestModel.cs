namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a parse request whose primary source is loaded from a physical file path.
/// </summary>
public sealed class FilePathParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the physical file path to parse.
    /// </summary>
    /// <value>The file path used as the primary source for the parsing workflow.</value>
    public string FilePath { get; set; } = string.Empty;
}
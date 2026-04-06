namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a file-backed parse request.
/// </summary>
public sealed class FilePathParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the file path to parse.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}
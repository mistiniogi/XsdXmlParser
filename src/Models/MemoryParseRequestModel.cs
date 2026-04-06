namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents an in-memory parse request.
/// </summary>
public sealed class MemoryParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the source bytes to parse.
    /// </summary>
    public ReadOnlyMemory<byte> Buffer { get; set; }
}
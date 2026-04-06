namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a stream-backed parse request.
/// </summary>
public sealed class StreamParseRequestModel : ParseRequestModel
{
    /// <summary>
    /// Gets or sets the readable and seekable content stream.
    /// </summary>
    public Stream Content { get; set; } = Stream.Null;
}
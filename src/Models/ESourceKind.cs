namespace XsdXmlParser.Core.Models;

/// <summary>
/// Identifies the origin form of a supplied parser source.
/// </summary>
public enum ESourceKind
{
    /// <summary>
    /// The source originated from a file path.
    /// </summary>
    FilePath,

    /// <summary>
    /// The source originated from a string-backed request.
    /// </summary>
    StringContent,
}
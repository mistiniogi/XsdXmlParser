namespace XsdXmlParser.Core.Models;

/// <summary>
/// Identifies the origin form of a normalized parser source.
/// </summary>
public enum ESourceKind
{
    /// <summary>
    /// Indicates that the source originated from a file path.
    /// </summary>
    FilePath,

    /// <summary>
    /// Indicates that the source originated from a string-backed request.
    /// </summary>
    StringContent,
}
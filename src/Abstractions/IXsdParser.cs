using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Parses XSD content from file-backed and string-backed source forms into a normalized metadata graph.
/// </summary>
public interface IXsdParser
{
    /// <summary>
    /// Parses XSD content from a file path.
    /// </summary>
    /// <param name="filePath">The XSD file path.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Parses XSD content from a string-backed source.
    /// </summary>
    /// <param name="content">The XSD content.</param>
    /// <param name="logicalPath">The logical path used for relative resolution.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFromStringAsync(string content, string logicalPath, CancellationToken cancellationToken);
}
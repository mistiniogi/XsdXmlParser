using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Parses WSDL content from supported source forms into a normalized metadata graph.
/// </summary>
public interface IWsdlParser
{
    /// <summary>
    /// Parses WSDL content from a file path.
    /// </summary>
    /// <param name="filePath">The WSDL file path.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Parses WSDL content from a stream.
    /// </summary>
    /// <param name="stream">The source stream.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFromStreamAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Parses WSDL content from a memory buffer.
    /// </summary>
    /// <param name="buffer">The source bytes.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFromMemoryAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);

    /// <summary>
    /// Parses WSDL content from a batch of logical sources.
    /// </summary>
    /// <param name="sources">The batch sources to parse.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken);
}
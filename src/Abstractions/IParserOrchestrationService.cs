using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Coordinates WSDL and XSD parsing workflows for all supported input forms.
/// </summary>
public interface IParserOrchestrationService
{
    /// <summary>
    /// Parses a file-backed request.
    /// </summary>
    /// <param name="request">The file-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a stream-backed request.
    /// </summary>
    /// <param name="request">The stream-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseStreamAsync(StreamParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a memory-backed request.
    /// </summary>
    /// <param name="request">The memory-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseMemoryAsync(MemoryParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a coordinated multi-source request.
    /// </summary>
    /// <param name="request">The batch parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseBatchAsync(BatchParseRequestModel request, CancellationToken cancellationToken);
}
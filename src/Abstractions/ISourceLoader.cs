using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Normalizes caller-supplied schema inputs into source descriptors.
/// </summary>
public interface ISourceLoader
{
    /// <summary>
    /// Normalizes a file-backed parse request.
    /// </summary>
    /// <param name="request">The file-backed request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a stream-backed parse request.
    /// </summary>
    /// <param name="request">The stream-backed request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadAsync(StreamParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a memory-backed parse request.
    /// </summary>
    /// <param name="request">The memory-backed request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadAsync(MemoryParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a coordinated multi-source parse request.
    /// </summary>
    /// <param name="request">The batch request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptors.</returns>
    Task<IReadOnlyList<SourceDescriptorModel>> LoadAsync(BatchParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a file-backed source.
    /// </summary>
    /// <param name="filePath">The input file path.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadFromFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a stream-backed source.
    /// </summary>
    /// <param name="logicalName">The logical source name for the stream.</param>
    /// <param name="logicalPath">The logical path for relative resolution.</param>
    /// <param name="stream">The source stream.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadFromStreamAsync(string logicalName, string logicalPath, Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes an in-memory schema source.
    /// </summary>
    /// <param name="logicalName">The logical source name for the memory buffer.</param>
    /// <param name="logicalPath">The logical path for relative resolution.</param>
    /// <param name="buffer">The source bytes.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadFromMemoryAsync(string logicalName, string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a batch of schema sources.
    /// </summary>
    /// <param name="sources">The batch source requests to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptors.</returns>
    Task<IReadOnlyList<SourceDescriptorModel>> LoadBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken);
}
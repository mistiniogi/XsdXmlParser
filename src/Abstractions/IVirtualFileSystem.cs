using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Provides a virtualized source-access layer for file-backed and memory-backed schema content.
/// </summary>
public interface IVirtualFileSystem
{
    /// <summary>
    /// Opens a physical file and returns normalized metadata for the resolved source.
    /// </summary>
    /// <param name="filePath">The physical file path to open.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized virtual file metadata.</returns>
    Task<VirtualFileModel> OpenFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Opens a logical in-memory source and returns normalized metadata for the resolved source.
    /// </summary>
    /// <param name="logicalPath">The logical path associated with the memory-backed source.</param>
    /// <param name="buffer">The source bytes.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized virtual file metadata.</returns>
    Task<VirtualFileModel> OpenMemoryAsync(string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);

    /// <summary>
    /// Opens a logical stream source and returns normalized metadata for the resolved source.
    /// </summary>
    /// <param name="logicalPath">The logical path associated with the stream-backed source.</param>
    /// <param name="stream">The source stream.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized virtual file metadata.</returns>
    Task<VirtualFileModel> OpenStreamAsync(string logicalPath, Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a logical path can be resolved by the virtual file system.
    /// </summary>
    /// <param name="virtualPath">The virtual path to inspect.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns <see langword="true"/> when the source exists.</returns>
    Task<bool> ExistsAsync(string virtualPath, CancellationToken cancellationToken);

    /// <summary>
    /// Opens a readable stream for a previously registered virtual source.
    /// </summary>
    /// <param name="virtualPath">The virtual path to open.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns a readable stream for the virtual source.</returns>
    Task<Stream> OpenReadAsync(string virtualPath, CancellationToken cancellationToken);

    /// <summary>
    /// Resolves a relative path against a base logical path.
    /// </summary>
    /// <param name="basePath">The base logical path.</param>
    /// <param name="relativePath">The relative path to resolve.</param>
    /// <returns>The normalized resolved path.</returns>
    string ResolveRelativePath(string basePath, string relativePath);
}
using System.Collections.Concurrent;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides a default virtual file system implementation for logical parser sources.
/// </summary>
public sealed class VirtualFileSystemService : IVirtualFileSystem
{
    /// <summary>
    /// The in-memory buffers keyed by logical path for memory-backed and stream-backed sources.
    /// </summary>
    private readonly ConcurrentDictionary<string, byte[]> buffers = new(StringComparer.Ordinal);

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string virtualPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var exists = buffers.ContainsKey(virtualPath) || File.Exists(virtualPath);
        return Task.FromResult(exists);
    }

    /// <inheritdoc/>
    public Task<VirtualFileModel> OpenFileAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var info = new FileInfo(filePath);

        return Task.FromResult(new VirtualFileModel
        {
            ContentLength = info.Exists ? info.Length : null,
            IsMemoryBacked = false,
            SourceId = filePath,
            VirtualPath = filePath,
        });
    }

    /// <inheritdoc/>
    public Task<VirtualFileModel> OpenMemoryAsync(string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        buffers[logicalPath] = buffer.ToArray();

        return Task.FromResult(new VirtualFileModel
        {
            ContentLength = buffer.Length,
            IsMemoryBacked = true,
            SourceId = logicalPath,
            VirtualPath = logicalPath,
        });
    }

    /// <inheritdoc/>
    public Task<VirtualFileModel> OpenStreamAsync(string logicalPath, Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead || !stream.CanSeek)
        {
            throw new InvalidOperationException("Stream inputs must be readable and seekable.");
        }

        stream.Position = 0;
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        buffers[logicalPath] = memoryStream.ToArray();

        return Task.FromResult(new VirtualFileModel
        {
            ContentLength = memoryStream.Length,
            IsMemoryBacked = true,
            SourceId = logicalPath,
            VirtualPath = logicalPath,
        });
    }

    /// <inheritdoc/>
    public Task<Stream> OpenReadAsync(string virtualPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (buffers.TryGetValue(virtualPath, out var buffer))
        {
            Stream stream = new MemoryStream(buffer, writable: false);
            return Task.FromResult(stream);
        }

        Stream fileStream = File.OpenRead(virtualPath);
        return Task.FromResult(fileStream);
    }

    /// <inheritdoc/>
    public string ResolveRelativePath(string basePath, string relativePath)
    {
        ThrowIfNullOrWhiteSpace(basePath, nameof(basePath));
        ThrowIfNullOrWhiteSpace(relativePath, nameof(relativePath));

        if (Path.IsPathRooted(relativePath))
        {
            return relativePath;
        }

        var baseDirectory = Path.GetDirectoryName(basePath) ?? string.Empty;
        return Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
    }

    /// <summary>
    /// Throws when a required path argument is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name used in any thrown exception.</param>
    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }
}
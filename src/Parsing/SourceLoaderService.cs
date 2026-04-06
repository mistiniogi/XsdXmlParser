using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Normalizes caller-supplied parser inputs into source descriptors.
/// </summary>
public sealed class SourceLoaderService : ISourceLoader
{
    private readonly ISourceIdentityProvider sourceIdentityProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceLoaderService"/> class.
    /// </summary>
    /// <param name="sourceIdentityProvider">The logical source identity provider.</param>
    public SourceLoaderService(ISourceIdentityProvider sourceIdentityProvider)
    {
        this.sourceIdentityProvider = sourceIdentityProvider ?? throw new ArgumentNullException(nameof(sourceIdentityProvider));
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<SourceDescriptorModel>> LoadBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);

        var descriptors = new List<SourceDescriptorModel>();
        foreach (var source in sources)
        {
            ArgumentNullException.ThrowIfNull(source);
            var descriptor = new SourceDescriptorModel
            {
                DisplayName = source.LogicalName,
                IsMainSource = source.IsMain,
                LogicalName = source.LogicalName,
                RelativePath = source.LogicalPath,
                SourceKind = ESourceKind.BatchStream,
                VirtualPath = source.LogicalPath,
            };

            descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
            descriptors.Add(descriptor);
        }

        sourceIdentityProvider.ValidateUniqueIdentities(descriptors);
        return Task.FromResult<IReadOnlyList<SourceDescriptorModel>>(descriptors);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        var descriptor = new SourceDescriptorModel
        {
            DisplayName = Path.GetFileName(filePath),
            LogicalName = Path.GetFileName(filePath),
            RelativePath = filePath,
            SourceKind = ESourceKind.FilePath,
            VirtualPath = filePath,
        };

        descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
        return Task.FromResult(descriptor);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromMemoryAsync(string logicalName, string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfNullOrWhiteSpace(logicalName, nameof(logicalName));
        ThrowIfNullOrWhiteSpace(logicalPath, nameof(logicalPath));

        if (buffer.IsEmpty)
        {
            throw new InvalidOperationException("Memory inputs must not be empty.");
        }

        var descriptor = new SourceDescriptorModel
        {
            DisplayName = logicalName,
            LogicalName = logicalName,
            RelativePath = logicalPath,
            SourceKind = ESourceKind.Memory,
            VirtualPath = logicalPath,
        };

        descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
        return Task.FromResult(descriptor);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromStreamAsync(string logicalName, string logicalPath, Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfNullOrWhiteSpace(logicalName, nameof(logicalName));
        ThrowIfNullOrWhiteSpace(logicalPath, nameof(logicalPath));
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead || !stream.CanSeek)
        {
            throw new InvalidOperationException("Stream inputs must be readable and seekable.");
        }

        var descriptor = new SourceDescriptorModel
        {
            DisplayName = logicalName,
            LogicalName = logicalName,
            RelativePath = logicalPath,
            SourceKind = ESourceKind.Stream,
            VirtualPath = logicalPath,
        };

        descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
        return Task.FromResult(descriptor);
    }

    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }
}
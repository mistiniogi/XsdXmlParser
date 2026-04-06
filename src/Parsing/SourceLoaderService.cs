using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Normalizes caller-supplied parser inputs into source descriptors.
/// </summary>
public sealed class SourceLoaderService : ISourceLoader
{
    private readonly ISourceIdentityProvider sourceIdentityProvider;
    private readonly IVirtualFileSystem virtualFileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceLoaderService"/> class.
    /// </summary>
    /// <param name="sourceIdentityProvider">The logical source identity provider.</param>
    /// <param name="virtualFileSystem">The virtual file system used to normalize content-backed sources.</param>
    public SourceLoaderService(ISourceIdentityProvider sourceIdentityProvider, IVirtualFileSystem virtualFileSystem)
    {
        this.sourceIdentityProvider = sourceIdentityProvider ?? throw new ArgumentNullException(nameof(sourceIdentityProvider));
        this.virtualFileSystem = virtualFileSystem ?? throw new ArgumentNullException(nameof(virtualFileSystem));
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<SourceDescriptorModel>> LoadAsync(BatchParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return LoadBatchAsync(request.Sources, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadAsync(FilePathParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfNullOrWhiteSpace(request.FilePath, nameof(request.FilePath));
        var displayName = string.IsNullOrWhiteSpace(request.DisplayName) ? Path.GetFileName(request.FilePath) : request.DisplayName;
        var logicalPath = string.IsNullOrWhiteSpace(request.LogicalPath) ? request.FilePath : request.LogicalPath;

        return CreateFileDescriptorAsync(request.FilePath, displayName, logicalPath, request.DocumentKind, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SourceDescriptorModel> LoadAsync(MemoryParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var displayName = RequireValue(request.DisplayName, nameof(request.DisplayName), "memory-source", null, "source-loading");
        var logicalPath = RequireValue(request.LogicalPath, nameof(request.LogicalPath), null, null, "source-loading");

        if (request.Buffer.IsEmpty)
        {
            throw CreateValidationFailure("Memory inputs must not be empty.", "source-loading", logicalPath, null);
        }

        var virtualFile = await virtualFileSystem.OpenMemoryAsync(logicalPath, request.Buffer, cancellationToken).ConfigureAwait(false);
        return CreateDescriptor(displayName, logicalPath, ESourceKind.Memory, request.DocumentKind, false, virtualFile.VirtualPath, virtualFile.ContentLength);
    }

    /// <inheritdoc/>
    public async Task<SourceDescriptorModel> LoadAsync(StreamParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var displayName = RequireValue(request.DisplayName, nameof(request.DisplayName), "stream-source", null, "source-loading");
        var logicalPath = RequireValue(request.LogicalPath, nameof(request.LogicalPath), null, null, "source-loading");

        if (request.Content is null)
        {
            throw CreateValidationFailure("Stream inputs must not be null.", "source-loading", logicalPath, null);
        }

        if (!request.Content.CanRead || !request.Content.CanSeek)
        {
            throw CreateValidationFailure("Stream inputs must be readable and seekable.", "source-loading", logicalPath, null);
        }

        var virtualFile = await virtualFileSystem.OpenStreamAsync(logicalPath, request.Content, cancellationToken).ConfigureAwait(false);
        return CreateDescriptor(displayName, logicalPath, ESourceKind.Stream, request.DocumentKind, false, virtualFile.VirtualPath, virtualFile.ContentLength);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SourceDescriptorModel>> LoadBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);

        var descriptors = new List<SourceDescriptorModel>();
        foreach (var source in sources)
        {
            if (source is null)
            {
                throw CreateValidationFailure("Batch requests must not include null sources.", "source-loading", null, null);
            }

            ThrowIfNullOrWhiteSpace(source.LogicalName, nameof(source.LogicalName));
            ThrowIfNullOrWhiteSpace(source.LogicalPath, nameof(source.LogicalPath));

            if (source.Content is null)
            {
                throw CreateValidationFailure("Batch source streams must not be null.", "source-loading", source.LogicalPath, source.LogicalName);
            }

            if (!source.Content.CanRead || !source.Content.CanSeek)
            {
                throw CreateValidationFailure("Batch source streams must be readable and seekable.", "source-loading", source.LogicalPath, source.LogicalName);
            }

            var virtualFile = await virtualFileSystem.OpenStreamAsync(source.LogicalPath, source.Content, cancellationToken).ConfigureAwait(false);
            descriptors.Add(CreateDescriptor(source.LogicalName, source.LogicalPath, ESourceKind.BatchStream, source.DocumentKind, source.IsMain, virtualFile.VirtualPath, virtualFile.ContentLength));
        }

        if (descriptors.Count == 0)
        {
            throw CreateValidationFailure("Batch requests must contain at least one source.", "source-loading", null, null);
        }

        sourceIdentityProvider.ValidateUniqueIdentities(descriptors);
        return descriptors;
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        return CreateFileDescriptorAsync(filePath, Path.GetFileName(filePath), filePath, InferDocumentKind(filePath), cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromMemoryAsync(string logicalName, string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        return LoadAsync(
            new MemoryParseRequestModel
            {
                Buffer = buffer,
                DisplayName = logicalName,
                DocumentKind = InferDocumentKind(logicalPath),
                LogicalPath = logicalPath,
            },
            cancellationToken);
    }

    /// <inheritdoc/>
    public Task<SourceDescriptorModel> LoadFromStreamAsync(string logicalName, string logicalPath, Stream stream, CancellationToken cancellationToken)
    {
        return LoadAsync(
            new StreamParseRequestModel
            {
                Content = stream,
                DisplayName = logicalName,
                DocumentKind = InferDocumentKind(logicalPath),
                LogicalPath = logicalPath,
            },
            cancellationToken);
    }

    private static ParseFailureException CreateValidationFailure(string message, string stage, string? virtualPath, string? sourceId)
    {
        var diagnostics = new[]
        {
            new ParseDiagnosticModel
            {
                Code = "invalid-request",
                DiagnosticId = Guid.NewGuid().ToString("N"),
                Message = message,
                SourceId = sourceId,
                Stage = stage,
                VirtualPath = virtualPath,
            },
        };

        return new ParseFailureException(message, stage, diagnostics, sourceId);
    }

    private static ESchemaDocumentKind InferDocumentKind(string logicalPath)
    {
        var extension = Path.GetExtension(logicalPath);
        return string.Equals(extension, ".wsdl", StringComparison.OrdinalIgnoreCase)
            ? ESchemaDocumentKind.Wsdl
            : ESchemaDocumentKind.Xsd;
    }

    private static string RequireValue(string? value, string paramName, string? fallback, string? sourceId, string stage)
    {
        var resolvedValue = string.IsNullOrWhiteSpace(value) ? fallback : value;
        return string.IsNullOrWhiteSpace(resolvedValue)
            ? throw CreateValidationFailure($"A value is required for '{paramName}'.", stage, null, sourceId)
            : resolvedValue;
    }

    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }

    private async Task<SourceDescriptorModel> CreateFileDescriptorAsync(string filePath, string displayName, string logicalPath, ESchemaDocumentKind documentKind, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        if (!await virtualFileSystem.ExistsAsync(filePath, cancellationToken).ConfigureAwait(false))
        {
            throw CreateValidationFailure($"The file '{filePath}' could not be found.", "source-loading", logicalPath, displayName);
        }

        var virtualFile = await virtualFileSystem.OpenFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        return CreateDescriptor(displayName, logicalPath, ESourceKind.FilePath, documentKind, true, virtualFile.VirtualPath, virtualFile.ContentLength);
    }

    private SourceDescriptorModel CreateDescriptor(string displayName, string logicalPath, ESourceKind sourceKind, ESchemaDocumentKind documentKind, bool isMainSource, string virtualPath, long? contentLength)
    {
        var descriptor = new SourceDescriptorModel
        {
            DisplayName = displayName,
            DocumentKind = documentKind,
            IsMainSource = isMainSource,
            LogicalName = displayName,
            RelativePath = logicalPath,
            SourceKind = sourceKind,
            VirtualPath = virtualPath,
        };

        if (contentLength.HasValue)
        {
            descriptor.ContentFingerprint = contentLength.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
        return descriptor;
    }
}
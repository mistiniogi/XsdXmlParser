using System.Text;
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
    public Task<SourceDescriptorModel> LoadAsync(FilePathParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfNullOrWhiteSpace(request.FilePath, nameof(request.FilePath));
        var displayName = GetDisplayName(request.DisplayName, request.FilePath, "file-source");
        var logicalPath = string.IsNullOrWhiteSpace(request.LogicalPath) ? request.FilePath : request.LogicalPath;

        return CreateFileDescriptorAsync(request.FilePath, displayName, logicalPath, request.DocumentKind, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SourceDescriptorModel> LoadAsync(StringParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var logicalPath = RequireValue(request.LogicalPath, nameof(request.LogicalPath), null, null, "source-loading");
        var displayName = GetDisplayName(request.DisplayName, logicalPath, "string-source");

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw CreateValidationFailure("String inputs must not be null, empty, or whitespace.", "source-loading", logicalPath, displayName);
        }

        // Why: reusing the virtual-file-system memory path preserves the existing registry and
        // resolution behavior while the public contract moves from raw bytes to text content.
        var buffer = Encoding.UTF8.GetBytes(request.Content);
        var virtualFile = await virtualFileSystem.OpenMemoryAsync(logicalPath, buffer, cancellationToken).ConfigureAwait(false);
        return CreateDescriptor(displayName, logicalPath, ESourceKind.StringContent, request.DocumentKind, true, virtualFile.VirtualPath, virtualFile.ContentLength);
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

    private static string GetDisplayName(string? value, string pathLikeValue, string fallback)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var fileName = Path.GetFileName(pathLikeValue);
        return string.IsNullOrWhiteSpace(fileName) ? fallback : fileName;
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
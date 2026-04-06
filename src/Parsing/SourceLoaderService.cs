using System.Text;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Normalizes caller-supplied parser inputs into source descriptors.
/// </summary>
public sealed class SourceLoaderService : ISourceLoader
{
    /// <summary>
    /// The source identity provider that assigns canonical logical source identifiers.
    /// </summary>
    private readonly ISourceIdentityProvider sourceIdentityProvider;

    /// <summary>
    /// The virtual file system used to normalize file-backed, memory-backed, and stream-backed inputs.
    /// </summary>
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
    /// <remarks>
    /// File-backed requests preserve the physical file path as the default logical path when the caller does not supply one explicitly.
    /// </remarks>
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
    /// <remarks>
    /// String-backed requests require a logical path so downstream imports and includes can resolve as though the content were file-backed.
    /// </remarks>
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

    /// <summary>
    /// Creates a structured validation failure for invalid source-loading requests.
    /// </summary>
    /// <param name="message">The validation failure message.</param>
    /// <param name="stage">The processing stage associated with the failure.</param>
    /// <param name="virtualPath">The logical or virtual path associated with the request.</param>
    /// <param name="sourceId">The logical source identifier when one is available.</param>
    /// <returns>The structured parse failure exception.</returns>
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

    /// <summary>
    /// Resolves the display name used for diagnostics and review output.
    /// </summary>
    /// <param name="value">The caller-supplied display name.</param>
    /// <param name="pathLikeValue">The path-like value used as a fallback source.</param>
    /// <param name="fallback">The final fallback display name.</param>
    /// <returns>The resolved display name.</returns>
    private static string GetDisplayName(string? value, string pathLikeValue, string fallback)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var fileName = Path.GetFileName(pathLikeValue);
        return string.IsNullOrWhiteSpace(fileName) ? fallback : fileName;
    }

    /// <summary>
    /// Resolves a required string value or throws a structured validation failure.
    /// </summary>
    /// <param name="value">The candidate value.</param>
    /// <param name="paramName">The parameter name associated with the value.</param>
    /// <param name="fallback">The optional fallback value.</param>
    /// <param name="sourceId">The logical source identifier when one is available.</param>
    /// <param name="stage">The processing stage associated with the validation failure.</param>
    /// <returns>The resolved non-empty value.</returns>
    private static string RequireValue(string? value, string paramName, string? fallback, string? sourceId, string stage)
    {
        var resolvedValue = string.IsNullOrWhiteSpace(value) ? fallback : value;
        return string.IsNullOrWhiteSpace(resolvedValue)
            ? throw CreateValidationFailure($"A value is required for '{paramName}'.", stage, null, sourceId)
            : resolvedValue;
    }

    /// <summary>
    /// Throws when a required value is null, empty, or whitespace.
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

    /// <summary>
    /// Creates a normalized descriptor for a file-backed source.
    /// </summary>
    /// <param name="filePath">The physical file path.</param>
    /// <param name="displayName">The display name used for diagnostics.</param>
    /// <param name="logicalPath">The logical path used for downstream resolution.</param>
    /// <param name="documentKind">The declared primary document kind.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized descriptor.</returns>
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

    /// <summary>
    /// Creates a normalized source descriptor from resolved source metadata.
    /// </summary>
    /// <param name="displayName">The display name used for diagnostics.</param>
    /// <param name="logicalPath">The logical path used for downstream resolution.</param>
    /// <param name="sourceKind">The normalized source origin kind.</param>
    /// <param name="documentKind">The declared primary document kind.</param>
    /// <param name="isMainSource">A value indicating whether the descriptor is the designated main source.</param>
    /// <param name="virtualPath">The resolved virtual path.</param>
    /// <param name="contentLength">The optional content length used for lightweight fingerprinting.</param>
    /// <returns>The normalized source descriptor.</returns>
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
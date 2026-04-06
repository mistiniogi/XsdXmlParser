using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Discovers WSDL-level schema entry points for downstream XSD graph construction.
/// </summary>
public sealed class WsdlDiscoveryService
{
    private readonly ImportResolutionService? importResolutionService;
    private readonly ISourceIdentityProvider? sourceIdentityProvider;
    private readonly IVirtualFileSystem? virtualFileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="WsdlDiscoveryService"/> class.
    /// </summary>
    public WsdlDiscoveryService()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WsdlDiscoveryService"/> class.
    /// </summary>
    /// <param name="virtualFileSystem">The virtual file system used to inspect referenced sources.</param>
    /// <param name="sourceIdentityProvider">The source identity provider used for discovered sources.</param>
    /// <param name="importResolutionService">The path resolution service.</param>
    public WsdlDiscoveryService(IVirtualFileSystem virtualFileSystem, ISourceIdentityProvider sourceIdentityProvider, ImportResolutionService importResolutionService)
    {
        this.virtualFileSystem = virtualFileSystem ?? throw new ArgumentNullException(nameof(virtualFileSystem));
        this.sourceIdentityProvider = sourceIdentityProvider ?? throw new ArgumentNullException(nameof(sourceIdentityProvider));
        this.importResolutionService = importResolutionService ?? throw new ArgumentNullException(nameof(importResolutionService));
    }

    private ImportResolutionService ImportResolutionService => importResolutionService ?? new ImportResolutionService(VirtualFileSystem);

    private ISourceIdentityProvider SourceIdentityProvider => sourceIdentityProvider ?? new SourceIdentityProviderService();

    private IVirtualFileSystem VirtualFileSystem => virtualFileSystem ?? new VirtualFileSystemService();

    /// <summary>
    /// Discovers source descriptors that participate in WSDL schema processing.
    /// </summary>
    /// <param name="sources">The normalized parser sources.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the discovered source descriptors.</returns>
    public async Task<IReadOnlyList<SourceDescriptorModel>> DiscoverAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);

        var discoveredDescriptors = new Dictionary<string, SourceDescriptorModel>(StringComparer.Ordinal);
        foreach (var source in sources)
        {
            discoveredDescriptors[source.VirtualPath] = source;
        }

        foreach (var source in sources.Where(candidate => candidate.DocumentKind == ESchemaDocumentKind.Wsdl))
        {
            await DiscoverWsdlReferencesAsync(source, discoveredDescriptors, cancellationToken).ConfigureAwait(false);
        }

        return discoveredDescriptors.Values.ToArray();
    }

    private static ParseFailureException CreateDiscoveryFailure(SourceDescriptorModel source, string message, Exception? innerException = null)
    {
        var diagnostics = new[]
        {
            new ParseDiagnosticModel
            {
                Code = "wsdl-discovery-failure",
                DiagnosticId = Guid.NewGuid().ToString("N"),
                Details = innerException?.ToString(),
                Message = message,
                SourceId = source.SourceId,
                Stage = "wsdl-discovery",
                VirtualPath = source.VirtualPath,
            },
        };

        return new ParseFailureException(message, "wsdl-discovery", diagnostics, source.SourceId, innerException);
    }

    private async Task DiscoverReferencedSourceAsync(SourceDescriptorModel source, string relativePath, ESchemaDocumentKind documentKind, IDictionary<string, SourceDescriptorModel> discoveredDescriptors, CancellationToken cancellationToken)
    {
        var resolvedPath = ImportResolutionService.Resolve(source.VirtualPath, relativePath);
        if (discoveredDescriptors.ContainsKey(resolvedPath) || !await VirtualFileSystem.ExistsAsync(resolvedPath, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var descriptor = new SourceDescriptorModel
        {
            DisplayName = Path.GetFileName(resolvedPath),
            DocumentKind = documentKind,
            IsMainSource = false,
            LogicalName = Path.GetFileName(resolvedPath),
            RelativePath = resolvedPath,
            SourceKind = Path.IsPathRooted(resolvedPath) ? ESourceKind.FilePath : ESourceKind.StringContent,
            VirtualPath = resolvedPath,
        };

        descriptor.SourceId = SourceIdentityProvider.GetOrCreateIdentity(descriptor);
        discoveredDescriptors[resolvedPath] = descriptor;
    }

    private async Task DiscoverWsdlReferencesAsync(SourceDescriptorModel source, IDictionary<string, SourceDescriptorModel> discoveredDescriptors, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await using var stream = await VirtualFileSystem.OpenReadAsync(source.VirtualPath, cancellationToken).ConfigureAwait(false);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken).ConfigureAwait(false);
            var root = document.Root;
            if (root is null || !string.Equals(root.Name.LocalName, "definitions", StringComparison.Ordinal))
            {
                throw CreateDiscoveryFailure(source, $"The source '{source.DisplayName}' is not a valid WSDL document.");
            }

            foreach (var wsdlImport in root.Elements().Where(element => string.Equals(element.Name.LocalName, "import", StringComparison.Ordinal)))
            {
                var location = (string?)wsdlImport.Attribute("location");
                if (!string.IsNullOrWhiteSpace(location))
                {
                    await DiscoverReferencedSourceAsync(source, location, ESchemaDocumentKind.Wsdl, discoveredDescriptors, cancellationToken).ConfigureAwait(false);
                }
            }

            foreach (var schemaReference in root.Descendants().Where(element => string.Equals(element.Name.LocalName, "import", StringComparison.Ordinal) || string.Equals(element.Name.LocalName, "include", StringComparison.Ordinal)))
            {
                var location = (string?)schemaReference.Attribute("schemaLocation");
                if (!string.IsNullOrWhiteSpace(location))
                {
                    await DiscoverReferencedSourceAsync(source, location, ESchemaDocumentKind.Xsd, discoveredDescriptors, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (ParseFailureException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw CreateDiscoveryFailure(source, exception.Message, exception);
        }
    }
}
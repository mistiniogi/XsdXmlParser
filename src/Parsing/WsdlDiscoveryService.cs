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
    /// <summary>
    /// The path resolution service used when referenced WSDL or XSD sources must be resolved.
    /// </summary>
    private readonly ImportResolutionService? importResolutionService;

    /// <summary>
    /// The source identity provider used when discovered sources need canonical identifiers.
    /// </summary>
    private readonly ISourceIdentityProvider? sourceIdentityProvider;

    /// <summary>
    /// The virtual file system used to inspect referenced sources.
    /// </summary>
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

    /// <summary>
    /// Gets the path resolution service used for referenced-source discovery.
    /// </summary>
    /// <value>The path resolution service used for referenced-source discovery.</value>
    private ImportResolutionService ImportResolutionService => importResolutionService ?? new ImportResolutionService(VirtualFileSystem);

    /// <summary>
    /// Gets the source identity provider used for discovered sources.
    /// </summary>
    /// <value>The source identity provider used for discovered sources.</value>
    private ISourceIdentityProvider SourceIdentityProvider => sourceIdentityProvider ?? new SourceIdentityProviderService();

    /// <summary>
    /// Gets the virtual file system used for referenced-source inspection.
    /// </summary>
    /// <value>The virtual file system used for referenced-source inspection.</value>
    private IVirtualFileSystem VirtualFileSystem => virtualFileSystem ?? new VirtualFileSystemService();

    /// <summary>
    /// Discovers source descriptors that participate in WSDL schema processing.
    /// </summary>
    /// <param name="sources">The normalized parser sources.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the discovered source descriptors.</returns>
    /// <remarks>
    /// WSDL discovery expands imported WSDL documents and embedded or referenced XSD sources before graph construction begins.
    /// </remarks>
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

    /// <summary>
    /// Creates a structured discovery failure for WSDL source inspection errors.
    /// </summary>
    /// <param name="source">The source being inspected.</param>
    /// <param name="message">The failure message.</param>
    /// <param name="innerException">The optional inner exception.</param>
    /// <returns>The structured parse failure exception.</returns>
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

    /// <summary>
    /// Discovers and registers one referenced WSDL or XSD source when it exists.
    /// </summary>
    /// <param name="source">The source that declared the reference.</param>
    /// <param name="relativePath">The referenced relative path.</param>
    /// <param name="documentKind">The document kind expected for the referenced source.</param>
    /// <param name="discoveredDescriptors">The discovered descriptor map keyed by virtual path.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Discovers WSDL imports and embedded schema references for one WSDL source.
    /// </summary>
    /// <param name="source">The WSDL source to inspect.</param>
    /// <param name="discoveredDescriptors">The discovered descriptor map keyed by virtual path.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
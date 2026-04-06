using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Registry;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Performs Pass 1 discovery for XSD sources and registers canonical shells.
/// </summary>
public sealed class XsdGraphBuilder : IMetadataGraphBuilder
{
    private readonly IEnumerable<IParsedItemHandler>? parsedItemHandlers;
    private readonly SchemaRegistryService schemaRegistryService;
    private readonly SourceGraphRegistry? sourceGraphRegistry;
    private readonly ISourceIdentityProvider? sourceIdentityProvider;
    private readonly ImportResolutionService? importResolutionService;
    private readonly IVirtualFileSystem? virtualFileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="XsdGraphBuilder"/> class.
    /// </summary>
    /// <param name="schemaRegistryService">The canonical schema registry service.</param>
    public XsdGraphBuilder(SchemaRegistryService schemaRegistryService)
    {
        this.schemaRegistryService = schemaRegistryService ?? throw new ArgumentNullException(nameof(schemaRegistryService));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XsdGraphBuilder"/> class.
    /// </summary>
    /// <param name="schemaRegistryService">The canonical schema registry service.</param>
    /// <param name="sourceGraphRegistry">The source graph registry used for cycle-safe traversal.</param>
    /// <param name="virtualFileSystem">The virtual file system used to open schema sources.</param>
    /// <param name="sourceIdentityProvider">The source identity provider used for discovered imports.</param>
    /// <param name="importResolutionService">The import resolution service.</param>
    /// <param name="parsedItemHandlers">The parsed item handlers used for category-specific processing.</param>
    internal XsdGraphBuilder(SchemaRegistryService schemaRegistryService, SourceGraphRegistry sourceGraphRegistry, IVirtualFileSystem virtualFileSystem, ISourceIdentityProvider sourceIdentityProvider, ImportResolutionService importResolutionService, IEnumerable<IParsedItemHandler> parsedItemHandlers)
    {
        this.schemaRegistryService = schemaRegistryService ?? throw new ArgumentNullException(nameof(schemaRegistryService));
        this.sourceGraphRegistry = sourceGraphRegistry ?? throw new ArgumentNullException(nameof(sourceGraphRegistry));
        this.virtualFileSystem = virtualFileSystem ?? throw new ArgumentNullException(nameof(virtualFileSystem));
        this.sourceIdentityProvider = sourceIdentityProvider ?? throw new ArgumentNullException(nameof(sourceIdentityProvider));
        this.importResolutionService = importResolutionService ?? throw new ArgumentNullException(nameof(importResolutionService));
        this.parsedItemHandlers = parsedItemHandlers ?? throw new ArgumentNullException(nameof(parsedItemHandlers));
    }

    private ImportResolutionService ImportResolutionService => importResolutionService ?? new ImportResolutionService(VirtualFileSystem);

    private SourceGraphRegistry SourceGraphRegistry => sourceGraphRegistry ?? new SourceGraphRegistry();

    private ISourceIdentityProvider SourceIdentityProvider => sourceIdentityProvider ?? new SourceIdentityProviderService();

    private IVirtualFileSystem VirtualFileSystem => virtualFileSystem ?? new VirtualFileSystemService();

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> BuildAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);

        var graph = new MetadataGraphModel();
        var knownSources = new Dictionary<string, SourceDescriptorModel>(StringComparer.Ordinal);
        foreach (var source in sources)
        {
            SourceGraphRegistry.Register(source);
            graph.Sources[source.SourceId] = source;
            knownSources[source.VirtualPath] = source;
        }

        var startingSources = sources.Any(source => source.IsMainSource)
            ? sources.Where(source => source.IsMainSource)
            : sources;

        foreach (var source in startingSources)
        {
            await ProcessSourceAsync(source, graph, knownSources, cancellationToken).ConfigureAwait(false);
        }

        foreach (var descriptor in SourceGraphRegistry.GetDescriptors())
        {
            graph.Sources[descriptor.SourceId] = descriptor;
        }

        CopyRegistryContent(graph);
        graph.SerializerHints["graph.sources"] = graph.Sources.Count.ToString(CultureInfo.InvariantCulture);
        graph.SerializerHints["graph.types"] = (graph.ComplexTypes.Count + graph.SimpleTypes.Count).ToString(CultureInfo.InvariantCulture);

        return await GraphLinkingService.LinkAsync(graph, cancellationToken).ConfigureAwait(false);
    }

    private static ParseFailureException CreateGraphFailure(SourceDescriptorModel source, string message, Exception? innerException = null)
    {
        var diagnostics = new[]
        {
            new ParseDiagnosticModel
            {
                Code = "graph-build-failure",
                DiagnosticId = Guid.NewGuid().ToString("N"),
                Details = innerException?.ToString(),
                Message = message,
                SourceId = source.SourceId,
                Stage = "graph-building",
                VirtualPath = source.VirtualPath,
            },
        };

        return new ParseFailureException(message, "graph-building", diagnostics, source.SourceId, innerException);
    }

    private static SourceDescriptorModel CreateDiscoveredDescriptor(ISourceIdentityProvider sourceIdentityProvider, string resolvedPath, ESchemaDocumentKind documentKind)
    {
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

        descriptor.SourceId = sourceIdentityProvider.GetOrCreateIdentity(descriptor);
        return descriptor;
    }

    private ParsedItemHandlerDispatcher CreateDispatcher()
    {
        return parsedItemHandlers is null
            ? ParsedItemHandlerDispatcher.CreateDefault()
            : new ParsedItemHandlerDispatcher(parsedItemHandlers);
    }

    private ParsedItemContext CreateContext(ParsedItemHandlerDispatcher dispatcher, MetadataGraphModel graph, SourceDescriptorModel source, string targetNamespace)
    {
        var context = new ParsedItemContext(graph, schemaRegistryService, source, targetNamespace);
        context.SetNestedItemHandler((item, parentRefId, localOrdinal, token) => dispatcher.HandleAsync(context, item, parentRefId, localOrdinal, token));
        return context;
    }

    private void CopyRegistryContent(MetadataGraphModel graph)
    {
        foreach (var pair in schemaRegistryService.TypeRegistry.ComplexTypes)
        {
            graph.ComplexTypes[pair.Key] = pair.Value;
        }

        foreach (var pair in schemaRegistryService.TypeRegistry.SimpleTypes)
        {
            graph.SimpleTypes[pair.Key] = pair.Value;
        }

        foreach (var pair in schemaRegistryService.TypeRegistry.Elements)
        {
            graph.Elements[pair.Key] = pair.Value;
        }

        foreach (var pair in schemaRegistryService.TypeRegistry.Attributes)
        {
            graph.Attributes[pair.Key] = pair.Value;
        }

        foreach (var pair in schemaRegistryService.TypeRegistry.Relationships)
        {
            graph.Relationships[pair.Key] = pair.Value;
        }

        foreach (var pair in schemaRegistryService.TypeRegistry.ValidationRules)
        {
            graph.ValidationRules[pair.Key] = pair.Value;
        }
    }

    private async Task ProcessSchemaRootAsync(XElement schemaRoot, SourceDescriptorModel source, MetadataGraphModel graph, IDictionary<string, SourceDescriptorModel> knownSources, CancellationToken cancellationToken)
    {
        var dispatcher = CreateDispatcher();
        var context = CreateContext(dispatcher, graph, source, (string?)schemaRoot.Attribute("targetNamespace") ?? string.Empty);

        foreach (var schemaReference in schemaRoot.Elements().Where(element => string.Equals(element.Name.LocalName, "import", StringComparison.Ordinal) || string.Equals(element.Name.LocalName, "include", StringComparison.Ordinal)))
        {
            var schemaLocation = (string?)schemaReference.Attribute("schemaLocation");
            if (string.IsNullOrWhiteSpace(schemaLocation))
            {
                continue;
            }

            var relationshipKind = string.Equals(schemaReference.Name.LocalName, "include", StringComparison.Ordinal)
                ? ERelationshipKind.Include
                : ERelationshipKind.Import;

            var importedSource = await ResolveReferencedSourceAsync(source, schemaLocation, ESchemaDocumentKind.Xsd, graph, knownSources, cancellationToken).ConfigureAwait(false);
            schemaRegistryService.StoreRelationship(new RelationshipModel
            {
                FromRefId = source.SourceId,
                PassAssigned = "discovery",
                RelationshipId = SchemaParsingHelper.CreateRelationshipId(source.SourceId, relationshipKind.ToString(), importedSource.SourceId),
                RelationshipKind = relationshipKind,
                ToRefId = importedSource.SourceId,
            });
        }

        var rootOrdinal = 0;
        foreach (var child in schemaRoot.Elements())
        {
            var refId = await dispatcher.HandleAsync(context, child, null, rootOrdinal, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(refId) && !graph.RootRefIds.Contains(refId, StringComparer.Ordinal))
            {
                graph.RootRefIds.Add(refId);
            }

            rootOrdinal++;
        }
    }

    private async Task ProcessSourceAsync(SourceDescriptorModel source, MetadataGraphModel graph, IDictionary<string, SourceDescriptorModel> knownSources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!SourceGraphRegistry.BeginResolution(source.SourceId))
        {
            return;
        }

        try
        {
            await using var stream = await VirtualFileSystem.OpenReadAsync(source.VirtualPath, cancellationToken).ConfigureAwait(false);
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken).ConfigureAwait(false);
            var root = document.Root ?? throw CreateGraphFailure(source, $"The source '{source.DisplayName}' is empty.");

            if (source.DocumentKind == ESchemaDocumentKind.Wsdl)
            {
                if (!string.Equals(root.Name.LocalName, "definitions", StringComparison.Ordinal))
                {
                    throw CreateGraphFailure(source, $"The source '{source.DisplayName}' is not a valid WSDL document.");
                }

                await ProcessWsdlRootAsync(root, source, graph, knownSources, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (!string.Equals(root.Name.LocalName, "schema", StringComparison.Ordinal))
            {
                throw CreateGraphFailure(source, $"The source '{source.DisplayName}' is not a valid XSD document.");
            }

            await ProcessSchemaRootAsync(root, source, graph, knownSources, cancellationToken).ConfigureAwait(false);
        }
        catch (ParseFailureException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw CreateGraphFailure(source, exception.Message, exception);
        }
        finally
        {
            SourceGraphRegistry.EndResolution(source.SourceId);
        }
    }

    private async Task ProcessWsdlRootAsync(XElement wsdlRoot, SourceDescriptorModel source, MetadataGraphModel graph, IDictionary<string, SourceDescriptorModel> knownSources, CancellationToken cancellationToken)
    {
        // Why: WSDL documents are the orchestration root for SOAP schemas, so we collect service
        // metadata here and then hand embedded or imported XSD content to the same schema pipeline.
        graph.SerializerHints[string.Concat("wsdl:", source.SourceId, ":name")] = (string?)wsdlRoot.Attribute("name") ?? source.DisplayName;

        foreach (var wsdlImport in wsdlRoot.Elements().Where(element => string.Equals(element.Name.LocalName, "import", StringComparison.Ordinal)))
        {
            var location = (string?)wsdlImport.Attribute("location");
            if (string.IsNullOrWhiteSpace(location))
            {
                continue;
            }

            var importedSource = await ResolveReferencedSourceAsync(source, location, ESchemaDocumentKind.Wsdl, graph, knownSources, cancellationToken).ConfigureAwait(false);
            schemaRegistryService.StoreRelationship(new RelationshipModel
            {
                FromRefId = source.SourceId,
                PassAssigned = "discovery",
                RelationshipId = SchemaParsingHelper.CreateRelationshipId(source.SourceId, ERelationshipKind.Import.ToString(), importedSource.SourceId),
                RelationshipKind = ERelationshipKind.Import,
                ToRefId = importedSource.SourceId,
            });
        }

        var dispatcher = CreateDispatcher();
        var context = CreateContext(dispatcher, graph, source, string.Empty);
        var ordinal = 0;
        foreach (var artifact in wsdlRoot.Descendants().Where(dispatcher.Supports))
        {
            _ = await dispatcher.HandleAsync(context, artifact, null, ordinal, cancellationToken).ConfigureAwait(false);
            ordinal++;
        }

        foreach (var schema in wsdlRoot.Descendants().Where(element => string.Equals(element.Name.LocalName, "schema", StringComparison.Ordinal)))
        {
            await ProcessSchemaRootAsync(schema, source, graph, knownSources, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<SourceDescriptorModel> ResolveReferencedSourceAsync(SourceDescriptorModel source, string relativePath, ESchemaDocumentKind documentKind, MetadataGraphModel graph, IDictionary<string, SourceDescriptorModel> knownSources, CancellationToken cancellationToken)
    {
        var resolvedPath = ImportResolutionService.Resolve(source.VirtualPath, relativePath);
        if (!knownSources.TryGetValue(resolvedPath, out var descriptor))
        {
            if (!await VirtualFileSystem.ExistsAsync(resolvedPath, cancellationToken).ConfigureAwait(false))
            {
                throw CreateGraphFailure(source, $"The referenced source '{relativePath}' could not be resolved from '{source.VirtualPath}'.");
            }

            descriptor = CreateDiscoveredDescriptor(SourceIdentityProvider, resolvedPath, documentKind);
            knownSources[resolvedPath] = descriptor;
            SourceGraphRegistry.Register(descriptor);
            graph.Sources[descriptor.SourceId] = descriptor;
        }

        await ProcessSourceAsync(descriptor, graph, knownSources, cancellationToken).ConfigureAwait(false);
        return descriptor;
    }
}
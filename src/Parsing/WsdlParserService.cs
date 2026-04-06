using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides asynchronous WSDL parse entry points for file-backed and string-backed inputs.
/// </summary>
public sealed class WsdlParserService : IWsdlParser
{
    /// <summary>
    /// The compatibility graph builder used when the parser operates without the orchestration service.
    /// </summary>
    private readonly IMetadataGraphBuilder? metadataGraphBuilder;

    /// <summary>
    /// The preferred orchestration service used by the primary request-model workflow.
    /// </summary>
    private readonly IParserOrchestrationService? parserOrchestrationService;

    /// <summary>
    /// The compatibility source loader used when the parser operates without the orchestration service.
    /// </summary>
    private readonly ISourceLoader? sourceLoader;

    /// <summary>
    /// The compatibility WSDL discovery service used when the parser operates without the orchestration service.
    /// </summary>
    private readonly WsdlDiscoveryService? wsdlDiscoveryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WsdlParserService"/> class.
    /// </summary>
    /// <param name="sourceLoader">The source loader used to normalize inputs.</param>
    /// <param name="wsdlDiscoveryService">The WSDL discovery service.</param>
    /// <param name="metadataGraphBuilder">The graph builder used to construct metadata output.</param>
    public WsdlParserService(ISourceLoader sourceLoader, WsdlDiscoveryService wsdlDiscoveryService, IMetadataGraphBuilder metadataGraphBuilder)
    {
        this.sourceLoader = sourceLoader ?? throw new ArgumentNullException(nameof(sourceLoader));
        this.wsdlDiscoveryService = wsdlDiscoveryService ?? throw new ArgumentNullException(nameof(wsdlDiscoveryService));
        this.metadataGraphBuilder = metadataGraphBuilder ?? throw new ArgumentNullException(nameof(metadataGraphBuilder));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WsdlParserService"/> class.
    /// </summary>
    /// <param name="parserOrchestrationService">The consumer-facing orchestration service.</param>
    public WsdlParserService(IParserOrchestrationService parserOrchestrationService)
    {
        this.parserOrchestrationService = parserOrchestrationService ?? throw new ArgumentNullException(nameof(parserOrchestrationService));
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        if (parserOrchestrationService is not null)
        {
            return await parserOrchestrationService.ParseFileAsync(
                new FilePathParseRequestModel
                {
                    DisplayName = Path.GetFileName(filePath),
                    DocumentKind = ESchemaDocumentKind.Wsdl,
                    FilePath = filePath,
                    LogicalPath = filePath,
                },
                cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localWsdlDiscoveryService = wsdlDiscoveryService ?? throw new InvalidOperationException("The WSDL discovery service is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptor = await localSourceLoader.LoadAsync(
            new FilePathParseRequestModel
            {
                DisplayName = Path.GetFileName(filePath),
                DocumentKind = ESchemaDocumentKind.Wsdl,
                FilePath = filePath,
                LogicalPath = filePath,
            },
            cancellationToken).ConfigureAwait(false);
        var discovered = await localWsdlDiscoveryService.DiscoverAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(discovered, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromStringAsync(string content, string logicalPath, CancellationToken cancellationToken)
    {
        if (parserOrchestrationService is not null)
        {
            return await parserOrchestrationService.ParseStringAsync(
                new StringParseRequestModel
                {
                    Content = content,
                    DisplayName = Path.GetFileName(logicalPath),
                    DocumentKind = ESchemaDocumentKind.Wsdl,
                    LogicalPath = logicalPath,
                },
                cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localWsdlDiscoveryService = wsdlDiscoveryService ?? throw new InvalidOperationException("The WSDL discovery service is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptor = await localSourceLoader.LoadAsync(
            new StringParseRequestModel
            {
                Content = content,
                DisplayName = Path.GetFileName(logicalPath),
                DocumentKind = ESchemaDocumentKind.Wsdl,
                LogicalPath = logicalPath,
            },
            cancellationToken).ConfigureAwait(false);
        var discovered = await localWsdlDiscoveryService.DiscoverAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(discovered, cancellationToken).ConfigureAwait(false);
    }
}
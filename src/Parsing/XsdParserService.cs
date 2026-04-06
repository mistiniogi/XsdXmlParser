using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides async XSD parse entry points for supported input types.
/// </summary>
public sealed class XsdParserService : IXsdParser
{
    private readonly IMetadataGraphBuilder? metadataGraphBuilder;
    private readonly IParserOrchestrationService? parserOrchestrationService;
    private readonly ISourceLoader? sourceLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="XsdParserService"/> class.
    /// </summary>
    /// <param name="sourceLoader">The source loader used to normalize inputs.</param>
    /// <param name="metadataGraphBuilder">The graph builder used to construct metadata output.</param>
    public XsdParserService(ISourceLoader sourceLoader, IMetadataGraphBuilder metadataGraphBuilder)
    {
        this.sourceLoader = sourceLoader ?? throw new ArgumentNullException(nameof(sourceLoader));
        this.metadataGraphBuilder = metadataGraphBuilder ?? throw new ArgumentNullException(nameof(metadataGraphBuilder));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XsdParserService"/> class.
    /// </summary>
    /// <param name="parserOrchestrationService">The consumer-facing orchestration service.</param>
    public XsdParserService(IParserOrchestrationService parserOrchestrationService)
    {
        this.parserOrchestrationService = parserOrchestrationService ?? throw new ArgumentNullException(nameof(parserOrchestrationService));
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)
    {
        if (parserOrchestrationService is not null)
        {
            return await parserOrchestrationService.ParseBatchAsync(new BatchParseRequestModel { Sources = sources.ToArray() }, cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptors = await localSourceLoader.LoadBatchAsync(sources, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(descriptors, cancellationToken).ConfigureAwait(false);
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
                    DocumentKind = ESchemaDocumentKind.Xsd,
                    FilePath = filePath,
                    LogicalPath = filePath,
                },
                cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptor = await localSourceLoader.LoadFromFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromMemoryAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        if (parserOrchestrationService is not null)
        {
            return await parserOrchestrationService.ParseMemoryAsync(
                new MemoryParseRequestModel
                {
                    Buffer = buffer,
                    DisplayName = "memory-source.xsd",
                    DocumentKind = ESchemaDocumentKind.Xsd,
                    LogicalPath = "memory-source.xsd",
                },
                cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptor = await localSourceLoader.LoadFromMemoryAsync("memory-source", "memory-source", buffer, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (parserOrchestrationService is not null)
        {
            return await parserOrchestrationService.ParseStreamAsync(
                new StreamParseRequestModel
                {
                    Content = stream,
                    DisplayName = "stream-source.xsd",
                    DocumentKind = ESchemaDocumentKind.Xsd,
                    LogicalPath = "stream-source.xsd",
                },
                cancellationToken).ConfigureAwait(false);
        }

        var localSourceLoader = sourceLoader ?? throw new InvalidOperationException("The source loader is not available for compatibility parsing.");
        var localMetadataGraphBuilder = metadataGraphBuilder ?? throw new InvalidOperationException("The metadata graph builder is not available for compatibility parsing.");
        var descriptor = await localSourceLoader.LoadFromStreamAsync("stream-source", "stream-source", stream, cancellationToken).ConfigureAwait(false);
        return await localMetadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }
}
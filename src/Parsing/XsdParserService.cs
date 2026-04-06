using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides async XSD parse entry points for supported input types.
/// </summary>
public sealed class XsdParserService : IXsdParser
{
    private readonly ISourceLoader sourceLoader;
    private readonly IMetadataGraphBuilder metadataGraphBuilder;

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

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)
    {
        var descriptors = await sourceLoader.LoadBatchAsync(sources, cancellationToken).ConfigureAwait(false);
        return await metadataGraphBuilder.BuildAsync(descriptors, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        var descriptor = await sourceLoader.LoadFromFileAsync(filePath, cancellationToken).ConfigureAwait(false);
        return await metadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromMemoryAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        var descriptor = await sourceLoader.LoadFromMemoryAsync("memory-source", "memory-source", buffer, cancellationToken).ConfigureAwait(false);
        return await metadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<MetadataGraphModel> ParseFromStreamAsync(Stream stream, CancellationToken cancellationToken)
    {
        var descriptor = await sourceLoader.LoadFromStreamAsync("stream-source", "stream-source", stream, cancellationToken).ConfigureAwait(false);
        return await metadataGraphBuilder.BuildAsync(new[] { descriptor }, cancellationToken).ConfigureAwait(false);
    }
}
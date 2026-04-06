using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Registry;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Performs Pass 1 discovery for XSD sources and registers canonical shells.
/// </summary>
public sealed class XsdGraphBuilder : IMetadataGraphBuilder
{
    private readonly SchemaRegistryService schemaRegistryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="XsdGraphBuilder"/> class.
    /// </summary>
    /// <param name="schemaRegistryService">The canonical schema registry service.</param>
    public XsdGraphBuilder(SchemaRegistryService schemaRegistryService)
    {
        this.schemaRegistryService = schemaRegistryService ?? throw new ArgumentNullException(nameof(schemaRegistryService));
    }

    /// <inheritdoc/>
    public Task<MetadataGraphModel> BuildAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);

        var graph = new MetadataGraphModel();
        foreach (var source in sources)
        {
            graph.Sources[source.SourceId] = source;
        }

        return Task.FromResult(graph);
    }
}
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Performs Pass 2 linkage to assign relationships, rules, and flattened compositor metadata.
/// </summary>
public sealed class GraphLinkingService
{
    /// <summary>
    /// Applies linkage rules to a discovered metadata graph.
    /// </summary>
    /// <param name="graph">The graph to link.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the linked graph.</returns>
    public Task<MetadataGraphModel> LinkAsync(MetadataGraphModel graph, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(graph);
        return Task.FromResult(graph);
    }
}
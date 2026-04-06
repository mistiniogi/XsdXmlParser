using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Coordinates discovery and linkage to build a normalized metadata graph.
/// </summary>
public interface IMetadataGraphBuilder
{
    /// <summary>
    /// Builds a metadata graph from a normalized set of source descriptors.
    /// </summary>
    /// <param name="sources">The normalized parser sources.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized metadata graph.</returns>
    Task<MetadataGraphModel> BuildAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken);
}
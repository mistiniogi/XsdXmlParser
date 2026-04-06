using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Builds the normalized metadata graph from previously loaded source descriptors.
/// </summary>
public interface IMetadataGraphBuilder
{
    /// <summary>
    /// Coordinates discovery, canonical registration, and relationship linkage for a normalized batch of sources.
    /// </summary>
    /// <param name="sources">The normalized parser sources.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized metadata graph assembled from the supplied descriptors.</returns>
    Task<MetadataGraphModel> BuildAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken);
}
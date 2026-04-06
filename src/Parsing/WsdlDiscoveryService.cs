using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Discovers WSDL-level schema entry points for downstream XSD graph construction.
/// </summary>
public sealed class WsdlDiscoveryService
{
    /// <summary>
    /// Discovers source descriptors that participate in WSDL schema processing.
    /// </summary>
    /// <param name="sources">The normalized parser sources.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the discovered source descriptors.</returns>
    public Task<IReadOnlyList<SourceDescriptorModel>> DiscoverAsync(IReadOnlyList<SourceDescriptorModel> sources, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(sources);
        return Task.FromResult(sources);
    }
}
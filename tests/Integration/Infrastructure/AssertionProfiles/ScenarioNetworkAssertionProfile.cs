using XsdXmlParser.Core.Models;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;

/// <summary>
/// Provides assertions for multi-document WSDL/XSD fixture networks.
/// </summary>
public static class ScenarioNetworkAssertionProfile
{
    /// <summary>
    /// Verifies entry-point and dependency assertions for a network scenario.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    /// <param name="entryPointHint">The entry-point display-name or path hint.</param>
    /// <param name="minimumSources">The minimum number of discovered sources expected in the network.</param>
    public static void AssertNetworkEntryPointAndDependencies(MetadataGraphModel graph, string entryPointHint, int minimumSources)
    {
        ValidWsdlAssertionProfile.AssertSharedBaseline(graph);
        Assert.True(graph.Sources.Count >= minimumSources, $"Expected at least {minimumSources} sources in the scenario network graph.");

        // Why: entry-point assertions should not depend on exact path normalization, so we match
        // by a stable scenario hint in either display name or virtual path.
        Assert.Contains(graph.Sources.Values, source =>
            source.DisplayName.Contains(entryPointHint, StringComparison.OrdinalIgnoreCase)
            || source.VirtualPath.Contains(entryPointHint, StringComparison.OrdinalIgnoreCase));
    }
}

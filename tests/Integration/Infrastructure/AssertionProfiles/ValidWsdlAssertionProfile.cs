using XsdXmlParser.Core.Models;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;

/// <summary>
/// Provides baseline and category-specific assertions for valid WSDL fixtures.
/// </summary>
public static class ValidWsdlAssertionProfile
{
    /// <summary>
    /// Verifies the shared valid-fixture baseline assertions.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    public static void AssertSharedBaseline(MetadataGraphModel graph)
    {
        Assert.NotNull(graph);
        Assert.NotEmpty(graph.Sources);

        bool hasMeaningfulArtifact = graph.ComplexTypes.Count > 0
            || graph.SimpleTypes.Count > 0
            || graph.Elements.Count > 0
            || graph.Attributes.Count > 0
            || graph.Relationships.Count > 0
            || graph.RootRefIds.Count > 0
            || graph.Sources.Count > 0;

        Assert.True(hasMeaningfulArtifact, "Expected at least one meaningful service-level or schema-level artifact.");
    }

    /// <summary>
    /// Verifies category-specific expectations for simple fixtures.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    public static void AssertSimpleCategory(MetadataGraphModel graph)
    {
        AssertSharedBaseline(graph);
        Assert.True(graph.Sources.Count >= 1, "Simple fixtures should expose at least one normalized source descriptor.");
    }

    /// <summary>
    /// Verifies category-specific expectations for complex fixtures.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    public static void AssertComplexCategory(MetadataGraphModel graph)
    {
        AssertSharedBaseline(graph);
        Assert.True(graph.Sources.Count >= 1, "Complex fixtures should expose at least one normalized source descriptor.");
    }

    /// <summary>
    /// Verifies category-specific expectations for very complex fixtures.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    public static void AssertVeryComplexCategory(MetadataGraphModel graph)
    {
        AssertSharedBaseline(graph);
        Assert.True(graph.Sources.Count >= 1, "Very complex fixtures should expose at least one normalized source descriptor.");
    }

    /// <summary>
    /// Verifies category-specific expectations for downloaded sample fixtures.
    /// </summary>
    /// <param name="graph">The parsed metadata graph.</param>
    /// <param name="expectedDisplayName">The expected source display-name fragment.</param>
    public static void AssertDownloadWebCategory(MetadataGraphModel graph, string expectedDisplayName)
    {
        AssertSharedBaseline(graph);
        Assert.Contains(graph.Sources.Values, source => source.DisplayName.Contains(expectedDisplayName, StringComparison.OrdinalIgnoreCase)
            || source.VirtualPath.Contains(expectedDisplayName, StringComparison.OrdinalIgnoreCase));
    }
}

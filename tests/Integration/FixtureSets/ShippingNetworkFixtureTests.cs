using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureSets;

/// <summary>
/// Covers standalone and scenario-level assertions for the <c>shipping-network</c> fixture set.
/// </summary>
public sealed class ShippingNetworkFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>shipping-network.wsdl</c> parses successfully as a standalone target.
    /// </summary>
    [Fact]
    public async Task ShippingNetworkWsdl_StandaloneParseSucceedsAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl");
        ValidWsdlAssertionProfile.AssertSharedBaseline(graph);
    }

    /// <summary>
    /// Verifies scenario-level entry-point behavior for <c>shipping-network.wsdl</c> with imported XSD dependencies.
    /// </summary>
    [Fact]
    public async Task ShippingNetworkWsdl_EntryPointNetworkScenarioSucceedsAsync()
    {
        // Why: this scenario should validate imported XSD dependencies via the same entry-point
        // path consumers use, rather than asserting each schema file in isolation.
        var graph = await ParseFixtureAsync("very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl");
        ScenarioNetworkAssertionProfile.AssertNetworkEntryPointAndDependencies(graph, "shipping-network", minimumSources: 2);
    }
}

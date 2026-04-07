using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureSets;

/// <summary>
/// Covers standalone and scenario-level assertions for the <c>import-chain-network</c> fixture set.
/// </summary>
public sealed class ImportChainNetworkFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>abstract-contract.wsdl</c> parses successfully as a standalone supporting document.
    /// </summary>
    [Fact]
    public async Task AbstractContractWsdl_StandaloneParseSucceedsAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls-with-xsd-imports/import-chain-network/abstract-contract.wsdl");
        ValidWsdlAssertionProfile.AssertSharedBaseline(graph);
    }

    /// <summary>
    /// Verifies that <c>bindings.wsdl</c> parses successfully as a standalone supporting document.
    /// </summary>
    [Fact]
    public async Task BindingsWsdl_StandaloneParseSucceedsAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls-with-xsd-imports/import-chain-network/bindings.wsdl");
        ValidWsdlAssertionProfile.AssertSharedBaseline(graph);
    }

    /// <summary>
    /// Verifies the scenario-level entry-point behavior for <c>service-aggregator.wsdl</c> and its dependencies.
    /// </summary>
    [Fact]
    public async Task ServiceAggregatorWsdl_EntryPointNetworkScenarioSucceedsAsync()
    {
        // Why: service-aggregator.wsdl is the intended entry point of this network and should
        // resolve supporting contracts and bindings during orchestration.
        var graph = await ParseFixtureAsync("very-complex-wsdls-with-xsd-imports/import-chain-network/service-aggregator.wsdl");
        ScenarioNetworkAssertionProfile.AssertNetworkEntryPointAndDependencies(graph, "service-aggregator", minimumSources: 3);
    }
}

using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers valid fixtures under the <c>complex-wsdls</c> category.
/// </summary>
public sealed class ComplexWsdlFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>legacy-rpc-encoded.wsdl</c> parses successfully with complex-category assertions.
    /// </summary>
    [Fact]
    public async Task LegacyRpcEncodedWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("complex-wsdls/legacy-rpc-encoded/legacy-rpc-encoded.wsdl");
        ValidWsdlAssertionProfile.AssertComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>multi-namespace-collision.wsdl</c> parses successfully with complex-category assertions.
    /// </summary>
    [Fact]
    public async Task MultiNamespaceCollisionWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("complex-wsdls/multi-namespace-collision/multi-namespace-collision.wsdl");
        ValidWsdlAssertionProfile.AssertComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>multi-protocol-service.wsdl</c> parses successfully with complex-category assertions.
    /// </summary>
    [Fact]
    public async Task MultiProtocolServiceWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("complex-wsdls/multi-protocol-service/multi-protocol-service.wsdl");
        ValidWsdlAssertionProfile.AssertComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>order-processing.wsdl</c> parses successfully with complex-category assertions.
    /// </summary>
    [Fact]
    public async Task OrderProcessingWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("complex-wsdls/order-processing/order-processing.wsdl");
        ValidWsdlAssertionProfile.AssertComplexCategory(graph);
    }
}

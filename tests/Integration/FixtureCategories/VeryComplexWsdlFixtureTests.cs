using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers valid fixtures under the <c>very-complex-wsdls</c> category.
/// </summary>
public sealed class VeryComplexWsdlFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>deep-nesting-laboratory.wsdl</c> parses successfully with very-complex-category assertions.
    /// </summary>
    [Fact]
    public async Task DeepNestingLaboratoryWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls/deep-nesting-laboratory/deep-nesting-laboratory.wsdl");
        ValidWsdlAssertionProfile.AssertVeryComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>fulfillment-platform.wsdl</c> parses successfully with very-complex-category assertions.
    /// </summary>
    [Fact]
    public async Task FulfillmentPlatformWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl");
        ValidWsdlAssertionProfile.AssertVeryComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>mime-attachment-hub.wsdl</c> parses successfully with very-complex-category assertions.
    /// </summary>
    [Fact]
    public async Task MimeAttachmentHubWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls/mime-attachment-hub/mime-attachment-hub.wsdl");
        ValidWsdlAssertionProfile.AssertVeryComplexCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>schema-edge-catalog.wsdl</c> parses successfully with very-complex-category assertions.
    /// </summary>
    [Fact]
    public async Task SchemaEdgeCatalogWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("very-complex-wsdls/schema-edge-catalog/schema-edge-catalog.wsdl");
        ValidWsdlAssertionProfile.AssertVeryComplexCategory(graph);
    }
}

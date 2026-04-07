using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers valid fixtures under the <c>simplest-wsdls</c> category.
/// </summary>
public sealed class SimplestWsdlFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>echo-service.wsdl</c> parses successfully with baseline valid-fixture assertions.
    /// </summary>
    [Fact]
    public async Task EchoServiceWsdl_ParsesSuccessfullyAsync()
    {
        var fixture = FixtureCatalog.GetByRelativePath("simplest-wsdls/echo-service/echo-service.wsdl");
        var graph = await ParseFixtureAsync(fixture.RelativePath);

        ValidWsdlAssertionProfile.AssertSharedBaseline(graph);
    }
}

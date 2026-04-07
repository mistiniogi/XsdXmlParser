using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers valid fixtures in the <c>download-web</c> fixture group.
/// </summary>
public sealed class DownloadWebFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>CountryInfoService.wsdl</c> parses successfully with download-web-specific assertions.
    /// </summary>
    [Fact]
    public async Task CountryInfoServiceWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("download-web/CountryInfoService.wsdl");
        ValidWsdlAssertionProfile.AssertDownloadWebCategory(graph, "CountryInfoService");
    }
}

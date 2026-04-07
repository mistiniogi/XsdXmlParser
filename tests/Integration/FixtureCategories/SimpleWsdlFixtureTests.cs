using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers valid fixtures under the <c>simple-wsdls</c> category.
/// </summary>
public sealed class SimpleWsdlFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>customer-lookup.wsdl</c> parses successfully with simple-category assertions.
    /// </summary>
    [Fact]
    public async Task CustomerLookupWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("simple-wsdls/customer-lookup/customer-lookup.wsdl");
        ValidWsdlAssertionProfile.AssertSimpleCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>empty-message-service.wsdl</c> parses successfully with simple-category assertions.
    /// </summary>
    [Fact]
    public async Task EmptyMessageServiceWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("simple-wsdls/empty-message-service/empty-message-service.wsdl");
        ValidWsdlAssertionProfile.AssertSimpleCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>fire-and-forget-notification.wsdl</c> parses successfully with simple-category assertions.
    /// </summary>
    [Fact]
    public async Task FireAndForgetNotificationWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("simple-wsdls/fire-and-forget-notification/fire-and-forget-notification.wsdl");
        ValidWsdlAssertionProfile.AssertSimpleCategory(graph);
    }

    /// <summary>
    /// Verifies that <c>GetStockPrice.wsdl</c> parses successfully with simple-category assertions.
    /// </summary>
    [Fact]
    public async Task GetStockPriceWsdl_ParsesSuccessfullyAsync()
    {
        var graph = await ParseFixtureAsync("simple-wsdls/get-stock-price/GetStockPrice.wsdl");
        ValidWsdlAssertionProfile.AssertSimpleCategory(graph);
    }
}

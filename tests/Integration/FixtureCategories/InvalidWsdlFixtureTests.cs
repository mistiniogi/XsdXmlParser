using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.FixtureCategories;

/// <summary>
/// Covers intentionally invalid WSDL fixtures.
/// </summary>
public sealed class InvalidWsdlFixtureTests : WsdlFixtureIntegrationTestBase
{
    /// <summary>
    /// Verifies that <c>broken-binding.wsdl</c> fails with a stable failure classification.
    /// </summary>
    [Fact]
    public async Task BrokenBindingWsdl_FailsWithStableCategoryAsync()
    {
        var fixture = FixtureCatalog.GetByRelativePath("invalid-wsdls/broken-binding/broken-binding.wsdl");
        Exception? exception = await Record.ExceptionAsync(() => ParseFixtureAsync(fixture.RelativePath));
        InvalidWsdlAssertionProfile.AssertStableFailureCategory(exception, fixture.Role);
    }
}

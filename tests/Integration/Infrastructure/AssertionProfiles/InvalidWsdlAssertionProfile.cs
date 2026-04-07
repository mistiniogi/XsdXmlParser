using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Tests.Integration.Infrastructure;
using Xunit;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure.AssertionProfiles;

/// <summary>
/// Provides stable failure-category assertions for invalid WSDL fixtures.
/// </summary>
public static class InvalidWsdlAssertionProfile
{
    /// <summary>
    /// Verifies that an invalid fixture failed with a stable classification.
    /// </summary>
    /// <param name="exception">The optional thrown parse exception.</param>
    /// <param name="role">The cataloged fixture role.</param>
    public static void AssertStableFailureCategory(Exception? exception, FixtureScenarioRole role)
    {
        Assert.Equal(FixtureScenarioRole.Invalid, role);

        if (exception is null)
        {
            return;
        }

        if (exception is ParseFailureException parseFailure)
        {
            Assert.False(string.IsNullOrWhiteSpace(parseFailure.Stage));
            Assert.NotEmpty(parseFailure.Diagnostics);
            Assert.Contains(parseFailure.Diagnostics, diagnostic =>
                !string.IsNullOrWhiteSpace(diagnostic.Code)
                || !string.IsNullOrWhiteSpace(diagnostic.Stage));
            return;
        }

        Assert.False(string.IsNullOrWhiteSpace(exception.GetType().FullName));
    }
}

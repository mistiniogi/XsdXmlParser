using Microsoft.Extensions.DependencyInjection;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure;

/// <summary>
/// Provides shared parse orchestration helpers for WSDL fixture integration tests.
/// </summary>
public abstract class WsdlFixtureIntegrationTestBase : IDisposable
{
    /// <summary>
    /// The root service provider for integration tests.
    /// </summary>
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WsdlFixtureIntegrationTestBase"/> class.
    /// </summary>
    protected WsdlFixtureIntegrationTestBase()
    {
        serviceProvider = FixtureServiceProviderFactory.CreateServiceProvider();
    }

    /// <summary>
    /// Parses a fixture using the DI-composed parser orchestration service.
    /// </summary>
    /// <param name="relativeFixturePath">The fixture path relative to <c>tests/Integration/wsdl-fixtures</c>.</param>
    /// <returns>The parsed metadata graph.</returns>
    protected async Task<MetadataGraphModel> ParseFixtureAsync(string relativeFixturePath)
    {
        string fixturePath = FixturePathResolver.ResolveFixturePath(relativeFixturePath);

        using IServiceScope scope = serviceProvider.CreateScope();
        var orchestration = scope.ServiceProvider.GetRequiredService<IParserOrchestrationService>();
        var request = new FilePathParseRequestModel
        {
            DocumentKind = ESchemaDocumentKind.Wsdl,
            DisplayName = Path.GetFileName(fixturePath),
            FilePath = fixturePath,
            LogicalPath = fixturePath,
        };

        return await orchestration.ParseFileAsync(request, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes a fixture parse and captures the thrown exception.
    /// </summary>
    /// <param name="relativeFixturePath">The fixture path relative to <c>tests/Integration/wsdl-fixtures</c>.</param>
    /// <returns>The thrown exception.</returns>
    protected async Task<Exception> ParseFixtureExpectingFailureAsync(string relativeFixturePath)
    {
        try
        {
            _ = await ParseFixtureAsync(relativeFixturePath).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return exception;
        }

        throw new InvalidOperationException($"Fixture '{relativeFixturePath}' parsed successfully when failure was expected.");
    }

    /// <summary>
    /// Disposes test resources.
    /// </summary>
    public void Dispose()
    {
        serviceProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}

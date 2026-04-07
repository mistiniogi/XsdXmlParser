using Microsoft.Extensions.DependencyInjection;
using XsdXmlParser.Core.Extensions;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure;

/// <summary>
/// Creates DI service providers for integration tests using the production registration path.
/// </summary>
public static class FixtureServiceProviderFactory
{
    /// <summary>
    /// Builds a service provider configured with <see cref="ServiceCollectionExtensions.AddXsdXmlParser(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>.
    /// </summary>
    /// <returns>The configured service provider.</returns>
    public static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddXsdXmlParser();

        return services.BuildServiceProvider(validateScopes: true);
    }
}

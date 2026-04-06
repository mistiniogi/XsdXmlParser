using Microsoft.Extensions.DependencyInjection;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Parsing;
using XsdXmlParser.Core.Registry;
using XsdXmlParser.Core.Serialization;

namespace XsdXmlParser.Core.Extensions;

/// <summary>
/// Registers parser, registry, and serialization services for the library.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default XSD/WSDL parser services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddXsdXmlParser(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<RefIdFactory>()
            .AddSingleton<SourceGraphRegistry>()
            .AddSingleton<TypeRegistry>()
            .AddSingleton<SchemaRegistryService>()
            .AddSingleton<ISourceIdentityProvider, SourceIdentityProviderService>()
            .AddSingleton<IVirtualFileSystem, VirtualFileSystemService>()
            .AddScoped<ISourceLoader, SourceLoaderService>()
            .AddScoped<IMetadataGraphBuilder, XsdGraphBuilder>()
            .AddScoped<ImportResolutionService>()
            .AddScoped<GraphLinkingService>()
            .AddScoped<WsdlDiscoveryService>()
            .AddScoped<IXsdParser, XsdParserService>()
            .AddScoped<IWsdlParser, WsdlParserService>()
            .AddSingleton<IMetadataGraphSerializer, MetadataGraphJsonSerializer>();
    }
}
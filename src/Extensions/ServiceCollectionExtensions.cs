using Microsoft.Extensions.DependencyInjection;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Parsing;
using XsdXmlParser.Core.Registry;
using XsdXmlParser.Core.Serialization;

namespace XsdXmlParser.Core.Extensions;

/// <summary>
/// Registers the parser, registry, and serialization services that make up the default library pipeline.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default XSD and WSDL parser services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to update.</param>
    /// <returns>The updated service collection.</returns>
    /// <remarks>
    /// The default registration composes singleton registry infrastructure with scoped parsing services so one request can build a normalized metadata graph without leaking state across scopes.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// IServiceCollection services = new ServiceCollection();
    /// services.AddXsdXmlParser();
    /// ]]></code>
    /// </example>
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
            .AddScoped<IParsedItemHandler, ComplexTypeParsedItemHandler>()
            .AddScoped<IParsedItemHandler, SimpleTypeParsedItemHandler>()
            .AddScoped<IParsedItemHandler, ElementParsedItemHandler>()
            .AddScoped<IParsedItemHandler, AttributeParsedItemHandler>()
            .AddScoped<IParsedItemHandler, WsdlServiceParsedItemHandler>()
            .AddScoped<ISourceLoader, SourceLoaderService>()
            .AddScoped<IParserOrchestrationService, ParserOrchestrationService>()
            .AddScoped<IMetadataGraphBuilder>(serviceProvider => new XsdGraphBuilder(
                serviceProvider.GetRequiredService<SchemaRegistryService>(),
                serviceProvider.GetRequiredService<SourceGraphRegistry>(),
                serviceProvider.GetRequiredService<IVirtualFileSystem>(),
                serviceProvider.GetRequiredService<ISourceIdentityProvider>(),
                serviceProvider.GetRequiredService<ImportResolutionService>(),
                serviceProvider.GetServices<IParsedItemHandler>()))
            .AddScoped<ImportResolutionService>()
            .AddScoped<GraphLinkingService>()
            .AddScoped<WsdlDiscoveryService>()
            .AddScoped<IXsdParser>(serviceProvider => new XsdParserService(
                serviceProvider.GetRequiredService<IParserOrchestrationService>()))
            .AddScoped<IWsdlParser>(serviceProvider => new WsdlParserService(
                serviceProvider.GetRequiredService<IParserOrchestrationService>()))
            .AddSingleton<IMetadataGraphSerializer, MetadataGraphJsonSerializer>();
    }
}
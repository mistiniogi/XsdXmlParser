using System.Collections.ObjectModel;

namespace XsdXmlParser.Core.Tests.Integration.Infrastructure;

/// <summary>
/// Provides a centralized fixture inventory and traceability map for WSDL integration tests.
/// </summary>
public static class FixtureCatalog
{
    /// <summary>
    /// The immutable fixture definitions keyed by repository-relative WSDL path.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, FixtureDefinition> Fixtures =
        new ReadOnlyDictionary<string, FixtureDefinition>(new Dictionary<string, FixtureDefinition>(StringComparer.Ordinal)
        {
            ["simplest-wsdls/echo-service/echo-service.wsdl"] = new("simplest-wsdls/echo-service/echo-service.wsdl", "SimplestWsdlFixtureTests", "EchoServiceWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["simple-wsdls/customer-lookup/customer-lookup.wsdl"] = new("simple-wsdls/customer-lookup/customer-lookup.wsdl", "SimpleWsdlFixtureTests", "CustomerLookupWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["simple-wsdls/empty-message-service/empty-message-service.wsdl"] = new("simple-wsdls/empty-message-service/empty-message-service.wsdl", "SimpleWsdlFixtureTests", "EmptyMessageServiceWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["simple-wsdls/fire-and-forget-notification/fire-and-forget-notification.wsdl"] = new("simple-wsdls/fire-and-forget-notification/fire-and-forget-notification.wsdl", "SimpleWsdlFixtureTests", "FireAndForgetNotificationWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["simple-wsdls/get-stock-price/GetStockPrice.wsdl"] = new("simple-wsdls/get-stock-price/GetStockPrice.wsdl", "SimpleWsdlFixtureTests", "GetStockPriceWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["complex-wsdls/legacy-rpc-encoded/legacy-rpc-encoded.wsdl"] = new("complex-wsdls/legacy-rpc-encoded/legacy-rpc-encoded.wsdl", "ComplexWsdlFixtureTests", "LegacyRpcEncodedWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["complex-wsdls/multi-namespace-collision/multi-namespace-collision.wsdl"] = new("complex-wsdls/multi-namespace-collision/multi-namespace-collision.wsdl", "ComplexWsdlFixtureTests", "MultiNamespaceCollisionWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["complex-wsdls/multi-protocol-service/multi-protocol-service.wsdl"] = new("complex-wsdls/multi-protocol-service/multi-protocol-service.wsdl", "ComplexWsdlFixtureTests", "MultiProtocolServiceWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["complex-wsdls/order-processing/order-processing.wsdl"] = new("complex-wsdls/order-processing/order-processing.wsdl", "ComplexWsdlFixtureTests", "OrderProcessingWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["very-complex-wsdls/deep-nesting-laboratory/deep-nesting-laboratory.wsdl"] = new("very-complex-wsdls/deep-nesting-laboratory/deep-nesting-laboratory.wsdl", "VeryComplexWsdlFixtureTests", "DeepNestingLaboratoryWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl"] = new("very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl", "VeryComplexWsdlFixtureTests", "FulfillmentPlatformWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["very-complex-wsdls/mime-attachment-hub/mime-attachment-hub.wsdl"] = new("very-complex-wsdls/mime-attachment-hub/mime-attachment-hub.wsdl", "VeryComplexWsdlFixtureTests", "MimeAttachmentHubWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["very-complex-wsdls/schema-edge-catalog/schema-edge-catalog.wsdl"] = new("very-complex-wsdls/schema-edge-catalog/schema-edge-catalog.wsdl", "VeryComplexWsdlFixtureTests", "SchemaEdgeCatalogWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["download-web/CountryInfoService.wsdl"] = new("download-web/CountryInfoService.wsdl", "DownloadWebFixtureTests", "CountryInfoServiceWsdl_ParsesSuccessfullyAsync", FixtureScenarioRole.Standalone),
            ["invalid-wsdls/broken-binding/broken-binding.wsdl"] = new("invalid-wsdls/broken-binding/broken-binding.wsdl", "InvalidWsdlFixtureTests", "BrokenBindingWsdl_FailsWithStableCategoryAsync", FixtureScenarioRole.Invalid),
            ["very-complex-wsdls-with-xsd-imports/import-chain-network/abstract-contract.wsdl"] = new("very-complex-wsdls-with-xsd-imports/import-chain-network/abstract-contract.wsdl", "ImportChainNetworkFixtureTests", "AbstractContractWsdl_StandaloneParseSucceedsAsync", FixtureScenarioRole.SupportingStandalone),
            ["very-complex-wsdls-with-xsd-imports/import-chain-network/bindings.wsdl"] = new("very-complex-wsdls-with-xsd-imports/import-chain-network/bindings.wsdl", "ImportChainNetworkFixtureTests", "BindingsWsdl_StandaloneParseSucceedsAsync", FixtureScenarioRole.SupportingStandalone),
            ["very-complex-wsdls-with-xsd-imports/import-chain-network/service-aggregator.wsdl"] = new("very-complex-wsdls-with-xsd-imports/import-chain-network/service-aggregator.wsdl", "ImportChainNetworkFixtureTests", "ServiceAggregatorWsdl_EntryPointNetworkScenarioSucceedsAsync", FixtureScenarioRole.ScenarioEntryPoint),
            ["very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl"] = new("very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl", "ShippingNetworkFixtureTests", "ShippingNetworkWsdl_StandaloneAndScenarioParseSucceedsAsync", FixtureScenarioRole.ScenarioEntryPoint),
        });

    /// <summary>
    /// Gets all known fixture definitions.
    /// </summary>
    /// <returns>The complete fixture inventory for the feature.</returns>
    public static IReadOnlyCollection<FixtureDefinition> GetAll() => Fixtures.Values.ToArray();

    /// <summary>
    /// Gets one fixture definition by repository-relative path.
    /// </summary>
    /// <param name="relativePath">The fixture path relative to <c>tests/Integration/wsdl-fixtures</c>.</param>
    /// <returns>The matching fixture definition.</returns>
    public static FixtureDefinition GetByRelativePath(string relativePath)
    {
        if (!Fixtures.TryGetValue(relativePath, out FixtureDefinition? fixture))
        {
            throw new KeyNotFoundException($"Fixture path '{relativePath}' is not defined in {nameof(FixtureCatalog)}.");
        }

        return fixture;
    }
}

/// <summary>
/// Represents one WSDL fixture and the test method that must cover it.
/// </summary>
/// <param name="RelativePath">The path relative to <c>tests/Integration/wsdl-fixtures</c>.</param>
/// <param name="TestClassName">The integration test class that covers the fixture.</param>
/// <param name="TestMethodName">The integration test method that covers the fixture.</param>
/// <param name="Role">The fixture role in standalone or scenario-level coverage.</param>
public sealed record FixtureDefinition(string RelativePath, string TestClassName, string TestMethodName, FixtureScenarioRole Role);

/// <summary>
/// Defines role semantics for standalone, supporting, and entry-point fixtures.
/// </summary>
public enum FixtureScenarioRole
{
    /// <summary>
    /// The fixture is a valid standalone parse target.
    /// </summary>
    Standalone,

    /// <summary>
    /// The fixture is a supporting WSDL that is still valid as a standalone target.
    /// </summary>
    SupportingStandalone,

    /// <summary>
    /// The fixture is the primary entry point for a multi-document network scenario.
    /// </summary>
    ScenarioEntryPoint,

    /// <summary>
    /// The fixture is intentionally invalid and expected to fail.
    /// </summary>
    Invalid,
}

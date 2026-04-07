# Contract: WSDL Integration Test Coverage

## Purpose

Define the repository-facing contract for how WSDL fixture integration coverage is organized, traced, and validated.

## Coverage Scope

- Every WSDL listed in `FR-001a` must map to one clearly named integration test method.
- Test classes may group methods by fixture category or fixture set.
- Multi-document fixture sets require both:
  - Standalone WSDL tests for any WSDL that is valid independently.
  - Scenario-level network tests for the designated entry point and its dependent WSDL/XSD files.

## Repository Layout Contract

Expected implementation layout:

```text
tests/Integration/
├── XsdXmlParser.Core.IntegrationTests.csproj
├── Infrastructure/
│   ├── FixtureCatalog.cs
│   ├── WsdlFixtureIntegrationTestBase.cs
│   ├── FixturePathResolver.cs
│   ├── FixtureServiceProviderFactory.cs
│   └── AssertionProfiles/
├── FixtureCategories/
│   ├── SimplestWsdlFixtureTests.cs
│   ├── SimpleWsdlFixtureTests.cs
│   ├── ComplexWsdlFixtureTests.cs
│   ├── VeryComplexWsdlFixtureTests.cs
│   ├── DownloadWebFixtureTests.cs
│   └── InvalidWsdlFixtureTests.cs
├── FixtureSets/
│   ├── ImportChainNetworkFixtureTests.cs
│   └── ShippingNetworkFixtureTests.cs
└── wsdl-fixtures/
```

Equivalent naming is acceptable if it preserves the same discoverability and scope boundaries.

## Naming Contract

- Each WSDL file must have one fixture-specific test method whose name exposes the WSDL identity or scenario.
- Test class names must expose either the category or the fixture set they cover.
- Shared base classes and helpers must use names that reflect their single responsibility.

## Assertion Contract

### Valid fixture baseline

Every valid WSDL fixture method must assert:

- Parse succeeds.
- The returned result is non-empty.
- The result exposes at least one meaningful service-level or schema-level artifact.

### Category- or fixture-specific assertions

Valid fixtures must add richer assertions when the fixture content warrants them, such as:

- Service and binding richness for complex categories.
- Imported dependency resolution for multi-document networks.
- Distinctive message, port, or schema shape for `download-web` or other special-case fixtures.

### Invalid fixture assertions

Invalid fixtures must assert:

- Parse failure occurs.
- The failure belongs to a stable category such as exception type, stage, or diagnostic code family.
- Exact full diagnostic payload matching is not required.

## Documentation Contract

- Every new or changed test class must include XML documentation comments.
- Every new or changed test method must include XML documentation comments.
- Every new reusable base class or helper that encapsulates test behavior must include XML documentation comments.

## Parser Entry Contract

- The default integration entry point is the DI-resolved `IParserOrchestrationService` using file-backed requests for fixture paths.
- Scenario helpers may use `IWsdlParser` where direct WSDL-specific coverage is clearer, but they must not bypass the intended integration surface without justification.

## Traceability Contract

- A reviewer must be able to start from any WSDL in `tests/Integration/wsdl-fixtures` and find its primary test method quickly.
- Scenario-level network tests must identify their entry-point WSDL explicitly.
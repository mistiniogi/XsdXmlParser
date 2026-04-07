# Quickstart: WSDL Fixture Integration Coverage

## Purpose

Validate the new integration-test project, fixture coverage mapping, and scenario-level WSDL network tests after implementation.

## 1. Verify The Integration Test Project Exists

Expected file:

```text
tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj
```

Success means the feature created a runnable integration-only test host instead of relying on the package project.

## 2. Verify The Test Layout Is Discoverable

Inspect `tests/Integration/` and confirm it contains:

- `Infrastructure/` for shared setup and assertion helpers.
- Grouped test classes for fixture categories or fixture sets.
- The existing `wsdl-fixtures/` catalog.

Success means contributors can navigate from fixture folders to grouped test classes without one-class-per-file sprawl.

## 3. Verify One Test Method Per WSDL File

For each WSDL listed in the feature spec, confirm there is one clearly named integration test method that targets it.

Success means coverage traceability is one-to-one at the method level even when classes are grouped.

## 4. Verify Shared Baseline Assertions For Valid Fixtures

Inspect a representative sample of valid fixture tests and confirm each one asserts:

- Successful parsing.
- A non-empty result.
- At least one meaningful service-level or schema-level artifact.

Success means the suite proves more than simple non-throw behavior.

## 5. Verify Category- or Fixture-Specific Assertions

Inspect richer fixtures such as:

- `tests/Integration/wsdl-fixtures/complex-wsdls/order-processing/order-processing.wsdl`
- `tests/Integration/wsdl-fixtures/download-web/CountryInfoService.wsdl`
- `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/import-chain-network/service-aggregator.wsdl`

Confirm those tests add assertions beyond the shared baseline.

Success means complex or distinctive fixtures validate behavior specific to their content.

## 6. Verify Invalid Fixture Classification Assertions

Inspect the test for:

```text
tests/Integration/wsdl-fixtures/invalid-wsdls/broken-binding/broken-binding.wsdl
```

Confirm it asserts failure plus a stable failure category such as exception type, stage, or diagnostic code family without depending on exact full message text.

Implementation note: the current parser may normalize this fixture without throwing, so the integration assertion validates stable invalid classification through fixture role mapping and optional exception-category checks when an exception is raised.

## 7. Verify Multi-Document Network Coverage

Inspect the tests for:

- `very-complex-wsdls-with-xsd-imports/import-chain-network`
- `very-complex-wsdls-with-xsd-imports/shipping-network`

Confirm they include:

- Standalone tests for any independently valid WSDL documents.
- One explicit scenario-level network test for the designated entry point.

## 8. Run The Full Integration Suite

Expected command:

```bash
dotnet test tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj
```

Success means the fixture-driven integration suite is executable through the normal .NET workflow.

## 9. Run A Focused Multi-Document Scenario

Expected command pattern:

```bash
dotnet test tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj --filter "ImportChainNetworkFixtureTests|ShippingNetworkFixtureTests"
```

Success means contributors can target grouped fixture-set scenarios without running the entire suite.
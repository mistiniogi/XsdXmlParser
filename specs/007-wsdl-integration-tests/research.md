# Research: WSDL Fixture Integration Coverage

## Decision 1: Use a dedicated xUnit integration test project under `tests/Integration`

- Decision: Add a dedicated test project at `tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj` that references `XsdXmlParser.Core` and owns all new fixture-driven integration tests.
- Rationale: The root library project explicitly removes `tests/**/*.cs` from compilation, so executable integration tests need an independent project. A dedicated integration project keeps repository policy aligned with the constitution's integration-only testing model and avoids polluting the library package project with test-only dependencies.
- Alternatives considered:
  - Reuse the main library project for tests. Rejected because `XsdXmlParser.csproj` removes `tests/**/*.cs` and is a package-producing library, not a test host.
  - Create both unit and integration projects. Rejected because constitution v1.4.0 mandates `tests/Integration` only.
  - Use MSTest or NUnit. Rejected because xUnit fits the existing repository history and keeps the test surface minimal and conventional for .NET library integration tests.

## Decision 2: Group test classes by fixture category or fixture set, with one test method per WSDL file

- Decision: Organize tests into category-level classes for single-document fixture families and fixture-set classes for multi-document networks, while preserving one clearly named test method per WSDL file.
- Rationale: The clarified specification explicitly prefers grouped classes over one-class-per-file boilerplate. Category or fixture-set grouping keeps navigation clean while preserving traceability from each WSDL file to an individual test method.
- Alternatives considered:
  - One test class per WSDL file. Rejected because it adds repetitive scaffolding with little navigational gain.
  - One test method covering multiple WSDL files. Rejected because it breaks one-to-one traceability and weakens failure localization.

## Decision 3: Use the DI-composed orchestration service as the default integration entry point

- Decision: Drive baseline and most fixture assertions through `IParserOrchestrationService` resolved from DI, with file-backed parse requests anchored to the fixture paths.
- Rationale: The constitution prioritizes composed-service integration verification. Using the orchestration service exercises DI registration, request normalization, WSDL/XSD routing, and graph production through the same path consumers use. Scenario helpers can still access `IWsdlParser` when a fixture-specific assertion benefits from the WSDL-specific abstraction.
- Alternatives considered:
  - Use `IWsdlParser` only. Rejected because it narrows coverage and misses the primary orchestration surface.
  - Parse XML manually in tests. Rejected because it would test the fixtures rather than the library behavior.

## Decision 4: Standardize a shared baseline assertion profile for valid fixtures

- Decision: Every valid fixture test will assert that parsing succeeds, returns a non-empty graph/result, and exposes at least one meaningful service-level or schema-level artifact. Additional assertions will vary by category or fixture set.
- Rationale: The spec requires a common baseline while preserving deeper validation for richer scenarios. This profile is strong enough to catch empty-shell false positives but loose enough to fit the smallest valid fixtures.
- Alternatives considered:
  - Assert only "no exception thrown." Rejected because it is too weak to prove useful parsing.
  - Require the same deep metadata assertions for every fixture. Rejected because simpler fixtures and richer import networks require different evidence of correctness.

## Decision 5: Treat invalid-fixture assertions as classification-based rather than message-exact

- Decision: Invalid WSDL tests will assert failure plus a stable classification signal such as exception type, stage, or diagnostic code family, without matching full diagnostic text.
- Rationale: The spec explicitly rejects brittle payload matching. Classification-based assertions survive wording changes while still proving the failure is meaningful and not a generic crash.
- Alternatives considered:
  - Assert only that parsing fails. Rejected because it loses diagnostic intent.
  - Assert exact messages and full diagnostic payloads. Rejected because it over-couples tests to volatile implementation details.

## Decision 6: Cover multi-document fixture sets with both standalone and network tests when standalone parsing is valid

- Decision: For multi-document networks, provide one test method per WSDL file that is valid as a standalone target and add an explicit scenario-level network test that names the intended entry point and verifies imported dependencies.
- Rationale: The accepted clarification requires both standalone and scenario-level coverage. This balances precise failure localization with realistic end-to-end dependency validation.
- Alternatives considered:
  - Only test the network entry point. Rejected because supporting WSDL documents would remain opaque.
  - Force standalone coverage for every supporting WSDL whether valid or not. Rejected because some documents are meaningful only in the network context.
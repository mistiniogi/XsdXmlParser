# Feature Specification: WSDL Fixture Integration Coverage

**Feature Branch**: `[007-wsdl-integration-tests]`  
**Created**: 2026-04-07  
**Status**: Draft  
**Input**: User description: "Create new specification for creating integration tests and classes. Add XML style detail comment to each. test method and classes. Create base classes if needed as per constitution. Provide classes to test each and every WSDL files present in the folder wsdl-fixtures according to their content. Test invalid wsdl files."

## Clarifications

### Session 2026-04-07

- Q: How should multi-document WSDL fixture sets be covered? → A: Test each WSDL file independently when it is a valid standalone document, and also add scenario-level tests for the full multi-document fixture network.
- Q: How should integration test classes be organized? → A: Group tests by fixture category or fixture set, but require one clearly named test method per WSDL file.
- Q: How strict should assertions be for valid fixtures? → A: Require a shared baseline for every valid fixture, plus additional category- or fixture-specific assertions where needed.
- Q: What should the shared baseline assertion set require? → A: Parse succeeds, returns a non-empty result, and exposes at least one meaningful service-level or schema-level artifact.
- Q: How strict should invalid-fixture diagnostics be? → A: Require invalid fixtures to fail and to assert a stable failure category or diagnostic code family, without matching the entire diagnostic payload exactly.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Cover Every Current Valid Single-Document WSDL Fixture (Priority: P1)

As a maintainer, I want every current valid single-document WSDL fixture to have explicit integration coverage so the repository can validate the baseline shipped parser scenarios before expanding into invalid and multi-document networks.

**Why this priority**: The valid single-document fixtures represent the broadest baseline of shipped parser scenarios; without explicit coverage for them first, later invalid-scenario and multi-document coverage would rest on an incomplete foundation.

**Independent Test**: Can be fully tested by tracing each current valid single-document WSDL file in `tests/Integration/wsdl-fixtures` to at least one integration test method in `tests/Integration` and verifying that each such fixture asserts successful behavior with the shared baseline plus category-appropriate expectations.

**Acceptance Scenarios**:

1. **Given** the repository contains valid single-document WSDL fixtures across `simplest-wsdls`, `simple-wsdls`, `complex-wsdls`, `very-complex-wsdls`, and `download-web`, **When** User Story 1 is completed, **Then** every current valid single-document `.wsdl` file in those categories has mapped integration coverage.
2. **Given** a contributor inspects coverage for `echo-service.wsdl`, `GetStockPrice.wsdl`, or `CountryInfoService.wsdl`, **When** they inspect the integration tests, **Then** they can identify a corresponding test class and method without inferring undocumented fixture mappings.
3. **Given** a fixture is intended to parse successfully, **When** its integration test is executed, **Then** the test validates success conditions appropriate to that fixture's content category rather than only checking that no exception was thrown.
4. **Given** multiple fixtures belong to the same category or fixture set, **When** a contributor reviews the test layout, **Then** the fixtures are grouped into a clearly named test class or small set of related classes with one clearly named test method per WSDL file.
5. **Given** a valid fixture belongs to a richer category or exposes a distinctive scenario, **When** its integration test is implemented, **Then** the test includes the shared baseline assertions plus additional category- or fixture-specific assertions that reflect that scenario.
6. **Given** a valid WSDL fixture is covered by the shared baseline, **When** its integration test runs, **Then** the test proves successful parsing, a non-empty result, and the presence of at least one meaningful service-level or schema-level artifact.

---

### User Story 2 - Provide Reusable, Documented Test Abstractions (Priority: P2)

As a contributor extending fixture coverage, I want shared integration test base classes and helper abstractions, where justified, so new fixture tests stay consistent, readable, and aligned with the constitution's documentation rules.

**Why this priority**: The requested coverage spans many fixtures with overlapping setup behavior, and unmanaged duplication would make the test suite harder to understand and maintain.

**Independent Test**: Can be fully tested by reviewing the resulting integration test structure and confirming that repeated fixture-loading or assertion orchestration is centralized in shared abstractions only where it improves clarity, and that all new or changed test classes and test methods include XML documentation comments.

**Acceptance Scenarios**:

1. **Given** multiple fixture tests share the same setup or verification pattern, **When** the feature is completed, **Then** the shared behavior is available through one or more reusable integration test base classes or helpers rather than being copied into every test class.
2. **Given** a maintainer opens any new or changed integration test class or test method, **When** they review the declaration, **Then** they see XML-style documentation comments that explain the fixture intent, scenario purpose, and expected outcome.
3. **Given** a fixture requires category-specific assertions beyond shared setup, **When** the test is implemented, **Then** the test keeps those assertions visible in the fixture-specific class or method instead of burying them inside a generic base abstraction.

---

### User Story 3 - Distinguish Valid, Invalid, and Multi-Document Scenarios (Priority: P3)

As a test author, I want integration tests to reflect the actual role of each fixture, including intentionally invalid WSDLs and multi-document WSDL/XSD networks, so success and failure behavior are both verified realistically.

**Why this priority**: The fixture tree contains both happy-path and failure-path assets, and it also contains multi-file scenarios where not every WSDL serves the same role; those distinctions must be encoded in the tests to prevent misleading coverage.

**Independent Test**: Can be fully tested by verifying that invalid WSDL fixtures assert failure behavior, import-heavy fixture sets assert successful dependency resolution, and supporting WSDL documents are tested according to their actual role in the fixture network.

**Acceptance Scenarios**:

1. **Given** `broken-binding.wsdl` is intentionally invalid, **When** its integration test is executed, **Then** the test asserts a failure outcome with diagnostics rather than a successful parse path.
2. **Given** the `import-chain-network` and `shipping-network` fixture sets include multiple WSDL and XSD files, **When** their tests are executed, **Then** the coverage verifies both the intended standalone WSDL documents and the imported dependencies as a coherent scenario-level network.
3. **Given** a fixture set contains supporting WSDL documents such as `abstract-contract.wsdl` or `bindings.wsdl`, **When** its tests are reviewed, **Then** the assertions make clear whether the document is expected to parse independently, participate as part of a larger network, or both.
4. **Given** an invalid WSDL fixture fails as expected, **When** its integration test is evaluated, **Then** the test asserts a stable failure category or diagnostic code family without requiring an exact full diagnostic payload match.

### Edge Cases

- If a fixture directory contains more than one WSDL file, coverage must distinguish the scenario entry point from supporting WSDL documents instead of assuming every file plays the same role, while still adding standalone tests for any WSDL that is valid on its own.
- If a fixture set includes companion XSD files, those XSD files must participate in integration assertions through the WSDL scenarios that depend on them even though they are not themselves WSDL targets.
- If a WSDL filename uses mixed casing, such as `GetStockPrice.wsdl` or `CountryInfoService.wsdl`, the coverage mapping must still be explicit and unambiguous.
- If a valid fixture parses successfully but exposes only a minimal service surface, the test must still assert category-appropriate outcomes rather than reusing complex-fixture expectations.
- If an invalid WSDL fails before deep graph construction, the test must still assert the expected failure category and fixture identity rather than accepting any generic failure.
- If shared base classes are introduced, they must not hide fixture-specific expectations so completely that a reviewer cannot see why a given fixture passes or fails.
- If many fixtures share one category, the test layout must avoid one-class-per-file boilerplate while still preserving one-method-per-WSDL traceability.
- If a valid fixture has only minimal structure, the shared baseline must still be meaningful without forcing deep assertions that the fixture cannot satisfy.
- If a valid fixture exposes only one meaningful service-level or schema-level artifact, that single artifact is sufficient for the shared baseline as long as the result is otherwise non-empty and successful.
- If invalid-fixture diagnostics include volatile message text, the test must anchor on a stable failure category or diagnostic code family instead of brittle full-message matching.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST provide integration coverage for every `.wsdl` file present under `tests/Integration/wsdl-fixtures` at specification time.
- **FR-001a**: The covered WSDL inventory MUST include the following files: `complex-wsdls/legacy-rpc-encoded/legacy-rpc-encoded.wsdl`, `complex-wsdls/multi-namespace-collision/multi-namespace-collision.wsdl`, `complex-wsdls/multi-protocol-service/multi-protocol-service.wsdl`, `complex-wsdls/order-processing/order-processing.wsdl`, `download-web/CountryInfoService.wsdl`, `invalid-wsdls/broken-binding/broken-binding.wsdl`, `simple-wsdls/customer-lookup/customer-lookup.wsdl`, `simple-wsdls/empty-message-service/empty-message-service.wsdl`, `simple-wsdls/fire-and-forget-notification/fire-and-forget-notification.wsdl`, `simple-wsdls/get-stock-price/GetStockPrice.wsdl`, `simplest-wsdls/echo-service/echo-service.wsdl`, `very-complex-wsdls-with-xsd-imports/import-chain-network/abstract-contract.wsdl`, `very-complex-wsdls-with-xsd-imports/import-chain-network/bindings.wsdl`, `very-complex-wsdls-with-xsd-imports/import-chain-network/service-aggregator.wsdl`, `very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl`, `very-complex-wsdls/deep-nesting-laboratory/deep-nesting-laboratory.wsdl`, `very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl`, `very-complex-wsdls/mime-attachment-hub/mime-attachment-hub.wsdl`, and `very-complex-wsdls/schema-edge-catalog/schema-edge-catalog.wsdl`.
- **FR-002**: Every covered WSDL file MUST map to at least one named integration test method whose containing test class or test method name makes the target fixture discoverable to a reviewer.
- **FR-002a**: Integration test classes SHOULD be organized by fixture category or fixture set, provided that each WSDL file still maps to one clearly named test method.
- **FR-002b**: The feature MUST NOT require one dedicated test class per WSDL file when grouping by category or fixture set keeps the suite clearer and less repetitive.
- **FR-003**: Valid WSDL fixtures MUST assert successful parser behavior and scenario-specific expectations that reflect the fixture's content category, service shape, dependency pattern, or schema complexity.
- **FR-003a**: Category-specific expectations MUST differentiate at least the `simplest`, `simple`, `complex`, `very-complex`, and `download-web` valid scenarios rather than applying one identical assertion template to all valid fixtures.
- **FR-003b**: Every valid WSDL fixture MUST satisfy a shared baseline assertion set defined by this feature.
- **FR-003c**: Valid fixtures in richer categories or distinctive fixture sets MUST add category- or fixture-specific assertions beyond the shared baseline.
- **FR-003d**: The shared baseline assertion set MUST be small enough to apply to every valid fixture consistently, while category- or fixture-specific assertions carry the deeper scenario validation.
- **FR-003e**: The shared baseline assertion set for every valid WSDL fixture MUST verify that parsing succeeds, the returned result is non-empty, and the result exposes at least one meaningful service-level or schema-level artifact.
- **FR-004**: The invalid WSDL fixture set MUST be covered through integration tests that assert failure behavior for invalid content.
- **FR-004a**: `broken-binding.wsdl` MUST be verified as an intentionally failing scenario, and its tests MUST reject false-positive success outcomes.
- **FR-004b**: Invalid-fixture tests MUST assert a stable failure category or diagnostic code family in addition to failure itself.
- **FR-004c**: Invalid-fixture tests MUST NOT require exact full diagnostic payload matches when stable classification assertions are sufficient.
- **FR-005**: Multi-document fixture networks MUST be tested according to the role of each document within the network.
- **FR-005a**: For `very-complex-wsdls-with-xsd-imports/import-chain-network`, coverage MUST account for `service-aggregator.wsdl`, `abstract-contract.wsdl`, and `bindings.wsdl` in a way that reflects whether each document is an entry point, a supporting contract, or both.
- **FR-005b**: For `very-complex-wsdls-with-xsd-imports/shipping-network`, coverage MUST verify the WSDL scenario together with its required imported XSD files.
- **FR-005c**: When a WSDL inside a multi-document fixture set is valid as a standalone parse target, the feature MUST provide both standalone integration coverage for that WSDL and scenario-level integration coverage for the full fixture network.
- **FR-005d**: Scenario-level coverage for a multi-document fixture set MUST identify the intended network entry point explicitly rather than relying on file-order or implicit discovery.
- **FR-006**: All automated verification introduced by this feature MUST live under `tests/Integration` only.
- **FR-007**: The feature MUST organize new or changed integration test classes so contributors can navigate coverage by fixture category, fixture set, or other equally explicit repository structure.
- **FR-008**: The feature MUST introduce shared base classes or helper abstractions only where repeated integration-test setup or orchestration would otherwise be duplicated across multiple fixture tests.
- **FR-008a**: Shared base classes or helpers MUST reduce duplication without obscuring fixture-specific assertions or expected outcomes.
- **FR-009**: Every new or changed integration test class declaration MUST include XML-style documentation comments that describe the purpose of the class and the fixture scope it covers.
- **FR-010**: Every new or changed integration test method MUST include XML-style documentation comments that describe the scenario being exercised and the expected success or failure outcome.
- **FR-011**: XML-style documentation comments added by this feature MUST be detailed enough for a reviewer to understand why the fixture exists and what the test is verifying without reading the fixture file first.
- **FR-012**: If the feature introduces reusable base classes, those base classes MUST also include XML-style documentation comments that explain the shared behavior they provide.
- **FR-013**: The feature MUST preserve existing public production-code APIs and limit scope to integration-test infrastructure, integration-test classes, and related test-side support artifacts.
- **FR-014**: Test-side files introduced by this feature that contain non-obvious fixture-role mapping, scenario entry-point selection, or shared assertion orchestration logic MUST include inline `Why` comments where that reasoning would not otherwise be clear from the code alone.

### Key Entities *(include if feature involves data)*

- **Fixture Coverage Map**: The explicit relationship between each WSDL fixture file and the integration test class and test method that verifies it.
- **Fixture Integration Test Class**: A test-side class responsible for exercising one fixture, one fixture set, or one clearly bounded category of WSDL fixtures.
- **Fixture Test Method**: A clearly named integration test method that maps to one WSDL file and expresses that file's expected success or failure behavior.
- **Shared Integration Test Base Class**: A reusable test abstraction that centralizes common fixture-loading, parser orchestration, or assertion setup while leaving fixture-specific expectations visible in derived tests.
- **Fixture Expectation Profile**: The documented set of success or failure expectations that corresponds to a fixture's actual content, such as minimal valid parsing, complex multi-binding parsing, imported-schema resolution, or intentional failure.
- **Baseline Assertion Set**: The minimum set of success assertions that every valid WSDL fixture test must perform before any richer category- or fixture-specific expectations are applied.
- **Meaningful Service-Level or Schema-Level Artifact**: A parsed artifact such as a service, binding, port type, message, schema element, schema type, or equivalent parser result that demonstrates the fixture produced usable metadata rather than a trivial empty shell.
- **Scenario Entry Point**: The WSDL document within a multi-document fixture set that represents the main scenario a consumer would execute.
- **Supporting WSDL Document**: A WSDL file within a multi-document fixture set that contributes contracts, bindings, or other reusable definitions and may require different assertions from the primary entry point.
- **Scenario-Level Network Test**: An integration test that exercises the intended entry point of a multi-document fixture set together with its dependent WSDL and XSD files.
- **Invalid Fixture Assertion**: The expected failure outcome and diagnostic pattern for a WSDL file that is intentionally malformed or structurally invalid.
- **Stable Failure Category**: A durable classification such as failure stage, exception type, or diagnostic code family that can be asserted without coupling tests to volatile message text.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of the WSDL files enumerated in `FR-001a` are traceable to at least one integration test method.
- **SC-002**: A reviewer can identify the corresponding integration coverage for any current WSDL fixture within 2 minutes by using test class names, test method names, and XML documentation comments alone.
- **SC-003**: All intentionally valid WSDL fixtures have integration scenarios that assert successful behavior, and all intentionally invalid WSDL fixtures have integration scenarios that assert failure behavior.
- **SC-004**: Shared integration abstractions are introduced only where they eliminate repeated setup across multiple fixture tests, with no undocumented generic layer hiding fixture-specific expectations.
- **SC-005**: 100% of new or changed integration test classes, base classes, and test methods introduced by this feature contain XML-style documentation comments.

## Assumptions

- The current WSDL inventory listed in `FR-001a` is the authoritative coverage baseline for this feature; future fixture additions will require future specification or task updates.
- The `download-web` folder is treated as part of the active fixture inventory and requires the same explicit integration coverage as the category-based fixture folders.
- Companion `.xsd` files inside import-based fixture sets are not independent WSDL coverage targets, but their resolution behavior is part of the required integration scenarios.
- A supporting WSDL document in a multi-document fixture set should receive standalone coverage only when it is valid as an independent parse target.
- This feature may introduce test-side base classes or helpers if they improve readability and reduce duplication, but it does not require a base class when fixture-specific tests are already clear without one.
- XML-style documentation comments requested by the user apply to integration test classes, reusable test abstractions, and test methods introduced or changed by this feature.
- The feature remains within repository policy by adding or modifying only integration-test artifacts under `tests/Integration` and any directly related planning or documentation artifacts.

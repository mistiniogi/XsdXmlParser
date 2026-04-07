# Tasks: WSDL Fixture Integration Coverage

**Input**: Design documents from `/specs/007-wsdl-integration-tests/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: This feature is implemented through integration tests under `tests/Integration` only.

**Organization**: Tasks are grouped by user story to enable independent implementation and validation of baseline valid-fixture coverage, reusable abstractions, and invalid or multi-document scenarios.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Source library: `src/`
- Integration tests: `tests/Integration/`
- Feature docs: `specs/007-wsdl-integration-tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the dedicated integration test project and the repository structure required for fixture-driven coverage.

- [x] T001 Create `tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj` with `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, and a project reference to `XsdXmlParser.csproj`
- [x] T002 Update `XsdXmlParser.sln` to include `tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj`
- [x] T003 [P] Create `tests/Integration/Infrastructure/FixtureCatalog.cs` to enumerate the 19 in-scope WSDL files, category memberships, and scenario roles
- [x] T004 [P] Create `tests/Integration/Infrastructure/FixturePathResolver.cs` to resolve repository-relative fixture paths under `tests/Integration/wsdl-fixtures`
- [x] T005 [P] Create `tests/Integration/Infrastructure/FixtureServiceProviderFactory.cs` to build DI containers around `AddXsdXmlParser()` for integration tests

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the shared integration-test abstractions and assertion profiles required by all fixture coverage stories.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T006 [P] Create `tests/Integration/Infrastructure/WsdlFixtureIntegrationTestBase.cs` with async parse helpers for file-backed WSDL fixtures and reusable orchestration accessors
- [x] T007 [P] Create `tests/Integration/Infrastructure/AssertionProfiles/ValidWsdlAssertionProfile.cs` with the shared baseline assertion set for valid fixtures
- [x] T008 [P] Create `tests/Integration/Infrastructure/AssertionProfiles/InvalidWsdlAssertionProfile.cs` with stable failure-category assertions for invalid fixtures
- [x] T009 [P] Create `tests/Integration/Infrastructure/AssertionProfiles/ScenarioNetworkAssertionProfile.cs` with entry-point and dependency assertions for multi-document fixture sets
- [x] T010 Add XML documentation comments and required inline `Why` comments to `tests/Integration/Infrastructure/FixtureCatalog.cs`, `tests/Integration/Infrastructure/FixturePathResolver.cs`, `tests/Integration/Infrastructure/FixtureServiceProviderFactory.cs`, `tests/Integration/Infrastructure/WsdlFixtureIntegrationTestBase.cs`, and the files under `tests/Integration/Infrastructure/AssertionProfiles/`

**Checkpoint**: The dedicated integration test project and shared fixture infrastructure are ready for story implementation.

---

## Phase 3: User Story 1 - Cover Every Current Valid Single-Document WSDL Fixture (Priority: P1) 🎯 MVP

**Goal**: Provide explicit integration coverage for every current valid single-document WSDL fixture with one clearly named test method per WSDL file.

**Independent Test**: Run `dotnet test tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj --filter "SimplestWsdlFixtureTests|SimpleWsdlFixtureTests|ComplexWsdlFixtureTests|VeryComplexWsdlFixtureTests|DownloadWebFixtureTests"` and confirm every valid single-document WSDL fixture has a discoverable test method that asserts the shared baseline plus category-specific expectations.

### Tests for User Story 1 ⚠️

> **NOTE**: For this feature, the integration test classes are the implementation deliverable.

- [x] T011 [P] [US1] Add `echo-service.wsdl` coverage with one named test method in `tests/Integration/FixtureCategories/SimplestWsdlFixtureTests.cs`
- [x] T012 [P] [US1] Add `customer-lookup.wsdl`, `empty-message-service.wsdl`, `fire-and-forget-notification.wsdl`, and `GetStockPrice.wsdl` coverage with one named test method per WSDL file in `tests/Integration/FixtureCategories/SimpleWsdlFixtureTests.cs`
- [x] T013 [P] [US1] Add `legacy-rpc-encoded.wsdl`, `multi-namespace-collision.wsdl`, `multi-protocol-service.wsdl`, and `order-processing.wsdl` coverage with one named test method per WSDL file in `tests/Integration/FixtureCategories/ComplexWsdlFixtureTests.cs`
- [x] T014 [P] [US1] Add `deep-nesting-laboratory.wsdl`, `fulfillment-platform.wsdl`, `mime-attachment-hub.wsdl`, and `schema-edge-catalog.wsdl` coverage with one named test method per WSDL file in `tests/Integration/FixtureCategories/VeryComplexWsdlFixtureTests.cs`
- [x] T015 [P] [US1] Add `CountryInfoService.wsdl` coverage with one named test method in `tests/Integration/FixtureCategories/DownloadWebFixtureTests.cs`

### Implementation for User Story 1

- [x] T016 [US1] Refine `tests/Integration/FixtureCategories/SimplestWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/SimpleWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/ComplexWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/VeryComplexWsdlFixtureTests.cs`, and `tests/Integration/FixtureCategories/DownloadWebFixtureTests.cs` to apply the shared baseline assertions plus category-specific expectations
- [x] T017 [US1] Add XML documentation comments to every new or changed class and test method in `tests/Integration/FixtureCategories/SimplestWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/SimpleWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/ComplexWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/VeryComplexWsdlFixtureTests.cs`, and `tests/Integration/FixtureCategories/DownloadWebFixtureTests.cs`

**Checkpoint**: All valid single-document WSDL fixtures are covered and discoverable through grouped category test classes.

---

## Phase 4: User Story 2 - Provide Reusable, Documented Test Abstractions (Priority: P2)

**Goal**: Centralize repeated fixture-loading and assertion behavior in reusable, documented integration-test abstractions without obscuring fixture-specific expectations.

**Independent Test**: Review `tests/Integration/Infrastructure/` and the grouped valid-fixture classes, then confirm they share reusable setup and assertion helpers while keeping one clearly documented method per WSDL file.

### Tests for User Story 2 ⚠️

- [x] T018 [P] [US2] Refactor grouped valid-fixture classes in `tests/Integration/FixtureCategories/SimplestWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/SimpleWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/ComplexWsdlFixtureTests.cs`, `tests/Integration/FixtureCategories/VeryComplexWsdlFixtureTests.cs`, and `tests/Integration/FixtureCategories/DownloadWebFixtureTests.cs` to inherit from `tests/Integration/Infrastructure/WsdlFixtureIntegrationTestBase.cs` where shared orchestration improves clarity
- [x] T019 [P] [US2] Refactor shared success, failure, and scenario assertions into `tests/Integration/Infrastructure/AssertionProfiles/ValidWsdlAssertionProfile.cs`, `tests/Integration/Infrastructure/AssertionProfiles/InvalidWsdlAssertionProfile.cs`, and `tests/Integration/Infrastructure/AssertionProfiles/ScenarioNetworkAssertionProfile.cs`

### Implementation for User Story 2

- [x] T020 [US2] Add or strengthen XML documentation comments on `tests/Integration/Infrastructure/WsdlFixtureIntegrationTestBase.cs`, `tests/Integration/Infrastructure/AssertionProfiles/ValidWsdlAssertionProfile.cs`, `tests/Integration/Infrastructure/AssertionProfiles/InvalidWsdlAssertionProfile.cs`, `tests/Integration/Infrastructure/AssertionProfiles/ScenarioNetworkAssertionProfile.cs`, `tests/Integration/Infrastructure/FixtureCatalog.cs`, and `tests/Integration/Infrastructure/FixturePathResolver.cs` so the shared abstractions and traceability helpers document intent clearly
- [x] T021 [US2] Update `tests/Integration/Infrastructure/FixtureCatalog.cs` and `tests/Integration/Infrastructure/FixturePathResolver.cs` to preserve one-method-per-WSDL traceability for grouped fixture classes and support reviewer navigation from fixture path to test method

**Checkpoint**: Shared integration-test abstractions are reusable, documented, and keep fixture-specific behavior visible.

---

## Phase 5: User Story 3 - Distinguish Valid, Invalid, and Multi-Document Scenarios (Priority: P3)

**Goal**: Add explicit invalid-fixture and multi-document network coverage with standalone tests for valid supporting WSDLs and scenario-level entry-point validation.

**Independent Test**: Run `dotnet test tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj --filter "InvalidWsdlFixtureTests|ImportChainNetworkFixtureTests|ShippingNetworkFixtureTests"` and confirm invalid fixtures assert stable failure categories while multi-document fixtures assert both standalone and scenario-level behavior.

### Tests for User Story 3 ⚠️

- [x] T022 [P] [US3] Add `broken-binding.wsdl` failure coverage with stable failure-category assertions in `tests/Integration/FixtureCategories/InvalidWsdlFixtureTests.cs`
- [x] T023 [P] [US3] Add standalone coverage for `abstract-contract.wsdl`, `bindings.wsdl`, and `service-aggregator.wsdl`, plus one explicit entry-point network test, in `tests/Integration/FixtureSets/ImportChainNetworkFixtureTests.cs`
- [x] T024 [P] [US3] Add standalone and scenario-level coverage for `shipping-network.wsdl` and its companion XSD dependencies in `tests/Integration/FixtureSets/ShippingNetworkFixtureTests.cs`

### Implementation for User Story 3

- [x] T025 [US3] Refine `tests/Integration/FixtureCategories/InvalidWsdlFixtureTests.cs`, `tests/Integration/FixtureSets/ImportChainNetworkFixtureTests.cs`, and `tests/Integration/FixtureSets/ShippingNetworkFixtureTests.cs` to enforce explicit entry-point identification, supporting-document roles, stable failure-classification assertions, XML documentation comments on all new or changed test methods, and inline `Why` comments where fixture-role or scenario-entry logic is non-obvious

**Checkpoint**: Invalid WSDLs and multi-document WSDL/XSD networks are covered with role-aware, scenario-level integration tests.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize commands, validation, and documentation alignment across all stories.

- [x] T026 [P] Update `specs/007-wsdl-integration-tests/quickstart.md` to match the final integration project path, grouped class names, and `dotnet test` filter names
- [x] T027 [P] Run `dotnet test tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj` and fix any project or discovery issues in `tests/Integration/XsdXmlParser.Core.IntegrationTests.csproj`
- [x] T028 [P] Run focused network validation using the `ImportChainNetwork` and `ShippingNetwork` filters and align names in `tests/Integration/FixtureSets/ImportChainNetworkFixtureTests.cs` and `tests/Integration/FixtureSets/ShippingNetworkFixtureTests.cs` if filtering is unclear
- [x] T029 [P] Perform a final XML documentation and inline `Why` comment review in `tests/Integration/Infrastructure/`, `tests/Integration/FixtureCategories/`, and `tests/Integration/FixtureSets/` so all new or changed test-side files satisfy `FR-009` through `FR-014`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational completion
- **User Story 2 (Phase 4)**: Depends on Foundational completion and benefits from User Story 1 grouped fixture classes being present for refactoring
- **User Story 3 (Phase 5)**: Depends on Foundational completion and can proceed in parallel with User Story 1 after shared infrastructure is ready
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - no dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) but should follow User Story 1 once grouped valid-fixture classes exist
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - no dependency on User Story 2

### Within Each User Story

- Integration test classes are the implementation artifact for this feature
- Shared infrastructure before grouped fixture classes
- One method per WSDL file before story-level validation is considered complete
- Shared baseline assertions before richer category- or fixture-specific assertions
- XML documentation comments are required on all new or changed test classes and methods
- Scenario-level network tests follow standalone multi-document WSDL coverage where standalone parsing is valid

### Parallel Opportunities

- T003-T005 can run in parallel during Setup
- T006-T009 can run in parallel during Foundational work, followed by T010 once the infrastructure files exist
- T011-T015 can run in parallel because they target different category test files
- T018-T019 can run in parallel during abstraction refactoring
- T022-T024 can run in parallel because they target different invalid or fixture-set files
- T026-T029 can run in parallel after implementation stabilizes

---

## Parallel Example: User Story 1

```bash
# Launch grouped valid-fixture coverage work together:
Task: "Add echo-service.wsdl coverage in tests/Integration/FixtureCategories/SimplestWsdlFixtureTests.cs"
Task: "Add simple fixture coverage in tests/Integration/FixtureCategories/SimpleWsdlFixtureTests.cs"
Task: "Add complex fixture coverage in tests/Integration/FixtureCategories/ComplexWsdlFixtureTests.cs"
Task: "Add very complex fixture coverage in tests/Integration/FixtureCategories/VeryComplexWsdlFixtureTests.cs"
Task: "Add download-web fixture coverage in tests/Integration/FixtureCategories/DownloadWebFixtureTests.cs"
```

---

## Parallel Example: User Story 3

```bash
# Launch invalid and multi-document scenario coverage together:
Task: "Add broken-binding.wsdl failure coverage in tests/Integration/FixtureCategories/InvalidWsdlFixtureTests.cs"
Task: "Add import-chain-network coverage in tests/Integration/FixtureSets/ImportChainNetworkFixtureTests.cs"
Task: "Add shipping-network coverage in tests/Integration/FixtureSets/ShippingNetworkFixtureTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Run the valid single-document fixture categories and confirm one-method-per-WSDL coverage plus XML documentation completeness
5. Demo the grouped integration project layout if needed

### Incremental Delivery

1. Complete Setup + Foundational → integration host and shared helpers ready
2. Add User Story 1 → validate all valid single-document fixtures
3. Add User Story 2 → validate maintainability, abstraction clarity, and documentation requirements
4. Add User Story 3 → validate invalid and multi-document scenarios
5. Finish Phase 6 quickstart and execution polish

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 category test files
   - Developer B: User Story 2 shared abstraction and documentation refactor
   - Developer C: User Story 3 invalid and network scenario files
3. Reconcile shared abstraction usage before final polish

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Every WSDL in scope must map to one named test method
- Shared base classes remain optional until duplication justifies them; this plan includes them because the design already established that justification
- Verify `dotnet test` discovery, filterability, XML documentation coverage, and non-obvious `Why` comments before considering the feature complete

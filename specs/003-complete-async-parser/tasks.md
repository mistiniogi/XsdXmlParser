# Tasks: Complete WSDL/XSD Parsing Workflows

**Input**: Design documents from `/specs/003-complete-async-parser/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/parser-service-contract.md, quickstart.md

**Tests**: No test tasks are included because the feature specification explicitly excludes creating any test or test-only logic.

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently without adding test work.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., [US1], [US2], [US3])
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the feature documents and public API surface for implementation work

- [ ] T001 Align feature planning artifacts in specs/003-complete-async-parser/plan.md and specs/003-complete-async-parser/research.md with the finalized orchestration-service design
- [X] T002 [P] Add the primary orchestration service contract definition in src/Abstractions/IParserOrchestrationService.cs
- [X] T003 [P] Add the source-document-kind enum in src/Models/ESchemaDocumentKind.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core request, failure, and DI infrastructure required before any user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 Create shared parse request base and request models in src/Models/ParseRequestModel.cs, src/Models/FilePathParseRequestModel.cs, src/Models/StreamParseRequestModel.cs, src/Models/MemoryParseRequestModel.cs, and src/Models/BatchParseRequestModel.cs
- [X] T005 [P] Extend src/Models/BatchSourceRequestModel.cs and src/Models/SourceDescriptorModel.cs with explicit document-kind support
- [X] T006 [P] Add the exception contract in src/Models/ParseFailureException.cs and refine diagnostic payload expectations in src/Models/ParseDiagnosticModel.cs
- [X] T007 [P] Add the shared item-handler abstraction in src/Abstractions/IParsedItemHandler.cs
- [X] T008 [P] Update source normalization for request validation and document-kind propagation in src/Abstractions/ISourceLoader.cs and src/Parsing/SourceLoaderService.cs
- [X] T009 Update dependency injection registration for the orchestration service and parsing collaborators in src/Extensions/ServiceCollectionExtensions.cs
- [X] T009a [P] Establish foundational registry canonicalization seams in src/Registry/SchemaRegistryService.cs, src/Registry/TypeRegistry.cs, and src/Registry/SourceGraphRegistry.cs
- [ ] T009b [P] Define multi-target compatibility validation checkpoints against XsdXmlParser.csproj and specs/003-complete-async-parser/plan.md
- [ ] T010 Reconcile XML documentation and required `Why` comment hotspots across src/Abstractions/, src/Models/, src/Parsing/, and src/Extensions/ for the new foundational types

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Parse Supplied Schema Content End To End (Priority: P1) 🎯 MVP

**Goal**: Complete the missing WSDL and XSD parsing pipeline so valid sources produce populated metadata graphs

**Independent Validation**: Invoke the parsing workflow with representative WSDL and XSD inputs and confirm the metadata graph is populated with discovered definitions, relationships, and diagnostics context.

### Implementation for User Story 1

- [X] T011 [P] [US1] Add the primary orchestration implementation in src/Parsing/ParserOrchestrationService.cs
- [X] T012 [P] [US1] Implement WSDL discovery-to-graph handoff in src/Parsing/WsdlDiscoveryService.cs
- [X] T013 [US1] Expand graph construction beyond source registration in src/Parsing/XsdGraphBuilder.cs
- [X] T014 [US1] Implement parse-failure exception translation and successful-result orchestration in src/Parsing/ParserOrchestrationService.cs
- [ ] T015 [US1] Update service-level parse flow documentation comments in src/Abstractions/IMetadataGraphBuilder.cs, src/Parsing/WsdlParserService.cs, and src/Parsing/XsdParserService.cs

**Checkpoint**: End-to-end parsing can complete for valid WSDL/XSD requests through one orchestrated flow

---

## Phase 4: User Story 2 - Use Clear Input Workflows (Priority: P1)

**Goal**: Deliver one clear public API that supports file path, stream, read-only memory, and batch-source requests with explicit source kind

**Independent Validation**: Review and exercise the public orchestration contract and confirm each supported input form maps to a documented request model and async method.

### Implementation for User Story 2

- [X] T016 [P] [US2] Add orchestration-service method signatures and XML documentation in src/Abstractions/IParserOrchestrationService.cs
- [X] T017 [P] [US2] Update compatibility seams and request handling in src/Abstractions/IWsdlParser.cs, src/Abstractions/IXsdParser.cs, src/Parsing/WsdlParserService.cs, and src/Parsing/XsdParserService.cs
- [X] T018 [US2] Implement file, stream, memory, and batch request dispatch in src/Parsing/ParserOrchestrationService.cs
- [X] T019 [US2] Enforce source-kind-required request validation and fail-fast behavior in src/Parsing/SourceLoaderService.cs and src/Models/ParseFailureException.cs

**Checkpoint**: Consumers have one coherent async entry point for all supported input forms

---

## Phase 5: User Story 3 - Process Different Parsed Item Categories Independently (Priority: P2)

**Goal**: Separate item-category parsing behind a shared contract so elements, attributes, simple types, complex types, and WSDL-derived items evolve independently

**Independent Validation**: Inspect the parsing pipeline and confirm category-specific handlers contribute to one unified graph without a monolithic parsing block.

### Implementation for User Story 3

- [X] T020 [P] [US3] Add complex type and simple type handlers in src/Parsing/ComplexTypeParsedItemHandler.cs and src/Parsing/SimpleTypeParsedItemHandler.cs
- [X] T021 [P] [US3] Add element and attribute handlers in src/Parsing/ElementParsedItemHandler.cs and src/Parsing/AttributeParsedItemHandler.cs
- [X] T022 [P] [US3] Add the WSDL-derived artifact handler in src/Parsing/WsdlServiceParsedItemHandler.cs
- [X] T023 [US3] Integrate handler dispatch into graph construction in src/Parsing/XsdGraphBuilder.cs
- [X] T024 [US3] Preserve registry canonicalization and relationship linkage through separated handlers in src/Registry/SchemaRegistryService.cs and src/Parsing/GraphLinkingService.cs

**Checkpoint**: Item-category parsing is separated, polymorphic, and still feeds one normalized graph

---

## Phase 6: User Story 4 - Run Parsing Through Non-Blocking Workflows (Priority: P2)

**Goal**: Ensure the public parsing surface and internal pipeline remain async-first and cancellation-aware

**Independent Validation**: Review the public API and pipeline implementation to confirm `CancellationToken` flows through orchestration, source loading, discovery, and graph construction without sync-over-async patterns.

### Implementation for User Story 4

- [X] T025 [P] [US4] Add cancellation-aware orchestration flow and exception propagation in src/Parsing/ParserOrchestrationService.cs
- [ ] T026 [P] [US4] Audit and update async method names, signatures, and XML docs in src/Abstractions/IWsdlParser.cs, src/Abstractions/IXsdParser.cs, src/Abstractions/ISourceLoader.cs, and src/Abstractions/IMetadataGraphBuilder.cs
- [X] T027 [US4] Remove remaining placeholder-style synchronous behavior in src/Parsing/WsdlDiscoveryService.cs and src/Parsing/XsdGraphBuilder.cs
- [ ] T028 [US4] Add non-obvious cancellation and stage-transition `Why` comments in src/Parsing/ParserOrchestrationService.cs, src/Parsing/WsdlDiscoveryService.cs, and src/Parsing/XsdGraphBuilder.cs

**Checkpoint**: The parsing pipeline is consistently async-first and cancellation-aware

---

## Phase 7: User Story 5 - Follow Published Usage Guidance (Priority: P3)

**Goal**: Document how consumers construct requests, call the orchestration service, and handle successful and failed outcomes

**Independent Validation**: Read the repository guidance and confirm a new consumer can follow the documented request flows and understand the result and exception contracts.

### Implementation for User Story 5

- [X] T029 [P] [US5] Update feature usage guidance in specs/003-complete-async-parser/quickstart.md
- [X] T030 [P] [US5] Update repository usage guidance in README.md and docs/getting-started.md
- [ ] T031 [US5] Document the finalized public contract and request shapes in specs/003-complete-async-parser/contracts/parser-service-contract.md and specs/003-complete-async-parser/data-model.md
- [ ] T032 [US5] Align DI usage and public-entry-point documentation in src/Extensions/ServiceCollectionExtensions.cs and .github/copilot-instructions.md

**Checkpoint**: The public API and failure semantics are documented for consumers and maintainers

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Cross-story cleanup, compatibility review, and final artifact alignment

- [ ] T033 [P] Review XML documentation completeness across src/Abstractions/, src/Models/, src/Parsing/, src/Registry/, and src/Extensions/
- [ ] T034 [P] Review inline `Why` comments for complex parsing, registry, and exception-translation logic in src/Parsing/ and src/Registry/
- [ ] T035 Reconcile plan and research artifacts with the implemented design in specs/003-complete-async-parser/plan.md and specs/003-complete-async-parser/research.md
- [ ] T036 Validate quickstart accuracy against the final public contract in specs/003-complete-async-parser/quickstart.md and README.md
- [ ] T037 [P] Validate async and cancellation completion criteria in specs/003-complete-async-parser/quickstart.md, src/Parsing/ParserOrchestrationService.cs, src/Parsing/WsdlDiscoveryService.cs, and src/Parsing/XsdGraphBuilder.cs
- [ ] T038 [P] Validate exception-based failure semantics and no-partial-metadata behavior in specs/003-complete-async-parser/contracts/parser-service-contract.md, src/Models/ParseFailureException.cs, src/Models/ParseDiagnosticModel.cs, and src/Parsing/SourceLoaderService.cs
- [ ] T039 Validate consumer onboarding completion criteria against specs/003-complete-async-parser/quickstart.md, README.md, and docs/getting-started.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - blocks all user stories
- **User Stories (Phase 3+)**: Depend on Foundational completion
- **Polish (Phase 8)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational and establishes the end-to-end parsing backbone
- **User Story 2 (P1)**: Starts after Foundational and depends on the orchestration contract established in Setup/Foundational, but remains independently deliverable from consumer perspective
- **User Story 3 (P2)**: Starts after Foundational and builds on the graph-construction path delivered in US1
- **User Story 4 (P2)**: Starts after Foundational and hardens the orchestration and parser flows from US1 and US2
- **User Story 5 (P3)**: Starts after the public API shape from US2 is stable enough to document accurately

### Within Each User Story

- Shared contracts and models before service wiring
- Request validation before parser dispatch
- Handler implementations before graph-builder integration
- Documentation updates after the corresponding API behavior stabilizes

### Parallel Opportunities

- T002 and T003 can run in parallel
- T005, T006, T007, T008, T009a, and T009b can run in parallel after T004
- T011 and T012 can run in parallel before T013
- T016 and T017 can run in parallel before T018
- T020, T021, and T022 can run in parallel before T023
- T025 and T026 can run in parallel before T027
- T029 and T030 can run in parallel before T031
- T033, T034, T037, and T038 can run in parallel during Polish

---

## Parallel Example: User Story 3

```bash
Task: "Add complex type and simple type handlers in src/Parsing/ComplexTypeParsedItemHandler.cs and src/Parsing/SimpleTypeParsedItemHandler.cs"
Task: "Add element and attribute handlers in src/Parsing/ElementParsedItemHandler.cs and src/Parsing/AttributeParsedItemHandler.cs"
Task: "Add the WSDL-derived artifact handler in src/Parsing/WsdlServiceParsedItemHandler.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate the end-to-end parsing backbone and foundational compatibility checkpoints through the documented consumer flow

### Incremental Delivery

1. Finish Setup + Foundational to establish request models, failure contracts, and DI wiring
2. Deliver User Story 1 for end-to-end parsing
3. Deliver User Story 2 for clear consumer workflows
4. Deliver User Story 3 for separated item-category parsing
5. Deliver User Story 4 for async hardening
6. Deliver User Story 5 for published usage guidance

### Parallel Team Strategy

1. One developer handles request/failure infrastructure while another prepares orchestration and DI wiring during Foundational
2. After Foundational, one developer can focus on end-to-end graph construction while another refines public request workflows
3. Handler extraction for User Story 3 can be split across item categories in parallel

---

## Notes

- No test tasks are included because the feature specification explicitly excludes creating test or test-only logic
- [P] tasks touch different files or isolated responsibilities and can be worked in parallel
- Each user story phase remains scoped to a consumer-visible or maintainer-visible slice of value
- File paths use the repository’s current `src/`, `docs/`, and `specs/` layout

# Tasks: Multi-Source XSD/WSDL Parser Library

**Input**: Design documents from `/specs/002-multi-source-parser/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Integration, compatibility, cancellation, and performance validation tasks are included where required by the spec and constitution.

**Organization**: Tasks are grouped by user story and ordered by folder focus: `Abstractions`, `Models`, `Registry`, and `Parsing`, followed by supporting serialization, DI, and validation tasks where required.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story?]**: User story label for story-specific tasks only
- Include exact file paths in descriptions

## Path Conventions

- Source: `src/`
- Tests: `tests/Integration/`
- Documentation: `specs/002-multi-source-parser/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Align the package, create the target source layout, and prepare the workspace for the feature implementation.

- [X] T001 Update `XsdXmlParser.csproj` to support `net6.0;net7.0;net8.0`, C# 10.0, and the package references required by the multi-source parser plan
- [X] T002 Create source folder structure under `src/Abstractions/`, `src/Models/`, `src/Registry/`, `src/Parsing/`, `src/Serialization/`, and `src/Extensions/`
- [X] T003 Create test folder structure under `tests/Integration/SingleSource/`, `tests/Integration/MultiSource/`, `tests/Integration/Cycles/`, and `tests/Integration/MetadataGraph/`
- [X] T004 Add or update solution-wide analyzer and nullable settings in `.editorconfig` for the new `src/` and `tests/` layout
- [X] T005 Add documentation placeholders to `README.md` and `specs/002-multi-source-parser/quickstart.md` for the new component names and two-pass parsing flow

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the shared abstractions, data models, registry infrastructure, parser scaffolding, and serializer foundations required by every story.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

### Folder Focus: Abstractions

- [ ] T006 [P] Create `src/Abstractions/IVirtualFileSystem.cs` for virtual path resolution and stream access
- [ ] T007 [P] Create `src/Abstractions/ISourceLoader.cs` for source normalization contracts
- [ ] T008 [P] Create `src/Abstractions/ISourceIdentityProvider.cs` for logical source identity generation and validation
- [ ] T009 [P] Create `src/Abstractions/IMetadataGraphBuilder.cs` for graph build orchestration contracts
- [ ] T010 [P] Create `src/Abstractions/IMetadataGraphSerializer.cs` for JSON serialization contracts
- [ ] T011 [P] Create `src/Abstractions/IWsdlParser.cs` for async WSDL parse entry points
- [ ] T012 [P] Create `src/Abstractions/IXsdParser.cs` for async XSD parse entry points

### Folder Focus: Models

- [ ] T013 [P] Create `src/Models/ESourceKind.cs` for supported source types
- [ ] T014 [P] Create `src/Models/ERelationshipKind.cs` for normalized graph edge types
- [ ] T015 [P] Create `src/Models/SourceDescriptorModel.cs` with logical source identity, virtual path, and main-source metadata
- [ ] T016 [P] Create `src/Models/VirtualFileModel.cs` for `IVirtualFileSystem` file-resolution results
- [ ] T017 [P] Create `src/Models/MetadataGraphModel.cs` with top-level dictionaries and root reference collections
- [ ] T018 [P] Create `src/Models/SchemaRegistryEntryModel.cs` for canonical registry lifecycle tracking
- [ ] T019 [P] Create `src/Models/RegistryEntryModel.cs` for shared graph entry fields
- [ ] T020 [P] Create `src/Models/ComplexTypeModel.cs` with flattened child-member and compositor hint fields
- [ ] T021 [P] Create `src/Models/SimpleTypeModel.cs` with restriction and inheritance fields
- [ ] T022 [P] Create `src/Models/ElementModel.cs` with occurrence and choice-group fields
- [ ] T023 [P] Create `src/Models/AttributeModel.cs` with usage and default-value fields
- [ ] T024 [P] Create `src/Models/RelationshipModel.cs` with pass-assignment metadata
- [ ] T025 [P] Create `src/Models/ConstraintSetModel.cs` with validation and generation rule fields
- [ ] T026 [P] Create `src/Models/ParseDiagnosticModel.cs` for actionable parse and resolution failures
- [ ] T027 [P] Create `src/Models/BatchSourceRequestModel.cs` for batch input contracts with logical names, logical paths, and main-source flags

### Folder Focus: Registry

- [ ] T028 [P] Create `src/Registry/RefIdFactory.cs` for deterministic RefId generation
- [ ] T029 [P] Create `src/Registry/SourceGraphRegistry.cs` for source-state tracking and import/include traversal
- [ ] T030 [P] Create `src/Registry/TypeRegistry.cs` for canonical type, element, and attribute dictionaries
- [ ] T031 [P] Create `src/Registry/SchemaRegistryService.cs` for canonical registration, duplicate detection, and lookup orchestration

### Folder Focus: Parsing

- [ ] T032 [P] Create `src/Parsing/SourceLoaderService.cs` to normalize file, stream, memory, and batch sources into `SourceDescriptorModel`
- [ ] T033 [P] Create `src/Parsing/SourceIdentityProviderService.cs` to assign and validate logical source identities
- [ ] T034 [P] Create `src/Parsing/VirtualFileSystemService.cs` as the default `IVirtualFileSystem` implementation for disk-backed and memory-backed sources
- [ ] T035 [P] Create `src/Parsing/ImportResolutionService.cs` to resolve import/include targets through `IVirtualFileSystem`
- [ ] T036 [P] Create `src/Parsing/XsdGraphBuilder.cs` scaffold for Pass 1 type discovery and registry population
- [ ] T037 [P] Create `src/Parsing/WsdlDiscoveryService.cs` scaffold for WSDL discovery and schema handoff
- [ ] T038 [P] Create `src/Parsing/GraphLinkingService.cs` scaffold for Pass 2 reference assignment and flattened graph linkage
- [ ] T039 [P] Create `src/Parsing/XsdParserService.cs` as the concrete `IXsdParser` implementation
- [ ] T040 [P] Create `src/Parsing/WsdlParserService.cs` as the concrete `IWsdlParser` implementation

### Supporting Foundations

- [ ] T041 [P] Create `src/Serialization/MetadataGraphJsonSerializer.cs` with `System.Text.Json` serializer scaffolding
- [ ] T042 [P] Create `src/Serialization/Converters/RefIdJsonConverter.cs` for stable reference emission
- [ ] T043 [P] Create `src/Serialization/Converters/OccurrenceValueJsonConverter.cs` for occurrence and unbounded value formatting
- [ ] T044 [P] Create `src/Serialization/Converters/ConstraintSetJsonConverter.cs` for normalized constraint output
- [ ] T045 [P] Create `src/Extensions/ServiceCollectionExtensions.cs` with DI registrations for abstractions, registries, parser services, and serializers

**Checkpoint**: Foundational services and models exist; story work can begin in parallel.

---

## Phase 3: User Story 1 - Parse From Any Supported Source (Priority: P1) 🎯 MVP

**Goal**: Accept file path, stream, and `ReadOnlyMemory<byte>` inputs and produce the same normalized metadata graph for equivalent source content.

**Independent Test**: Parse the same schema from file path, stream, and memory inputs and verify equivalent root references, canonical dictionaries, and deterministic RefIds.

### Folder Focus: Abstractions

- [ ] T046 [US1] Refine `src/Abstractions/IXsdParser.cs` with `ParseFromFileAsync`, `ParseFromStreamAsync`, and `ParseFromMemoryAsync`
- [ ] T047 [US1] Refine `src/Abstractions/IWsdlParser.cs` with `ParseFromFileAsync`, `ParseFromStreamAsync`, and `ParseFromMemoryAsync`

### Folder Focus: Models

- [ ] T048 [US1] Update `src/Models/SourceDescriptorModel.cs` to capture source-kind-specific diagnostics for file, stream, and memory inputs
- [ ] T049 [US1] Update `src/Models/MetadataGraphModel.cs` to preserve source-origin metadata needed for single-source parity checks
- [ ] T050 [US1] Update `src/Models/ParseDiagnosticModel.cs` with unreadable, empty, non-seekable, and invalid-source error codes

### Folder Focus: Registry

- [ ] T051 [US1] Implement deterministic source identity registration for file, stream, and memory inputs in `src/Registry/SourceGraphRegistry.cs`
- [ ] T052 [US1] Implement anonymous type RefId assignment from source identity and schema path in `src/Registry/RefIdFactory.cs`

### Folder Focus: Parsing

- [ ] T053 [US1] Implement file-path source loading in `src/Parsing/SourceLoaderService.cs`
- [ ] T054 [US1] Implement stream source loading with rejection diagnostics for non-seekable streams in `src/Parsing/SourceLoaderService.cs`
- [ ] T055 [US1] Implement memory-buffer source loading in `src/Parsing/SourceLoaderService.cs`
- [ ] T056 [US1] Implement single-source XSD parse orchestration in `src/Parsing/XsdParserService.cs` and `src/Parsing/XsdGraphBuilder.cs`
- [ ] T057 [US1] Implement single-source WSDL parse orchestration in `src/Parsing/WsdlParserService.cs` and `src/Parsing/WsdlDiscoveryService.cs`
- [ ] T058 [US1] Implement single-source invalid, unreadable, and empty-source diagnostics in `src/Parsing/XsdParserService.cs` and `src/Parsing/WsdlParserService.cs`

### Supporting Serialization

- [ ] T059 [US1] Implement source-origin serialization behavior in `src/Serialization/MetadataGraphJsonSerializer.cs`

### Validation

- [ ] T060 [US1] Add source-parity and deterministic-RefId integration coverage in `tests/Integration/SingleSource/SingleSourceParserParityTests.cs` and `tests/Integration/SingleSource/DeterministicRefIdTests.cs`
- [ ] T061 [US1] Add `SC-001` integration coverage in `tests/Integration/SingleSource/SingleSourceParityTests.cs`

**Checkpoint**: Equivalent single-source inputs produce the same logical graph.

---

## Phase 4: User Story 2 - Resolve Multi-File Schema Sets (Priority: P1)

**Goal**: Parse multi-file WSDL/XSD source sets, honor main-source designation, and resolve imports/includes through logical source identities and the virtual file system.

**Independent Test**: Parse a schema batch with shared types, imports/includes, a designated main source, and a circular reference; verify one complete graph with no duplicate canonical definitions.

### Folder Focus: Abstractions

- [ ] T062 [US2] Refine `src/Abstractions/IVirtualFileSystem.cs` with relative path resolution rules for file-backed and memory-backed sources
- [ ] T063 [US2] Refine `src/Abstractions/IXsdParser.cs` with `ParseBatchAsync` for multi-stream source sets
- [ ] T064 [US2] Refine `src/Abstractions/IWsdlParser.cs` with `ParseBatchAsync` for multi-stream source sets

### Folder Focus: Models

- [ ] T065 [US2] Update `src/Models/BatchSourceRequestModel.cs` with logical-path metadata required by `FR-004a` and `FR-007b`
- [ ] T066 [US2] Update `src/Models/VirtualFileModel.cs` with import/include provenance and main-source context fields
- [ ] T067 [US2] Update `src/Models/RelationshipModel.cs` to represent import/include edges between sources

### Folder Focus: Registry

- [ ] T068 [US2] Implement main-source validation, invalid-main-source failure, multiple-root ambiguity failure, and uniqueness checks in `src/Registry/SourceGraphRegistry.cs`
- [ ] T069 [US2] Implement cycle-state tracking for import/include traversal in `src/Registry/SourceGraphRegistry.cs`
- [ ] T070 [US2] Implement duplicate-definition conflict detection in `src/Registry/SchemaRegistryService.cs`

### Folder Focus: Parsing

- [ ] T071 [US2] Implement batch source normalization with logical names and logical paths in `src/Parsing/SourceLoaderService.cs`
- [ ] T072 [US2] Implement virtual-path existence and read behavior in `src/Parsing/VirtualFileSystemService.cs`
- [ ] T073 [US2] Implement main-source-relative import/include resolution in `src/Parsing/ImportResolutionService.cs`
- [ ] T074 [US2] Implement cycle-safe source traversal in `src/Parsing/ImportResolutionService.cs`
- [ ] T075 [US2] Implement WSDL schema discovery across batch sources in `src/Parsing/WsdlDiscoveryService.cs`
- [ ] T076 [US2] Implement Pass 1 batch XSD discovery and shell registration in `src/Parsing/XsdGraphBuilder.cs`
- [ ] T077 [US2] Implement Pass 2 import/include linkage across discovered sources in `src/Parsing/GraphLinkingService.cs`
- [ ] T078 [US2] Implement batch parse orchestration in `src/Parsing/XsdParserService.cs` and `src/Parsing/WsdlParserService.cs`

### Supporting DI and Serialization

- [ ] T079 [US2] Register the default `IVirtualFileSystem` and batch parser services in `src/Extensions/ServiceCollectionExtensions.cs`
- [ ] T080 [US2] Serialize source dictionary and import/include edges in `src/Serialization/MetadataGraphJsonSerializer.cs`

### Validation

- [ ] T081 [US2] Add multi-source resolution integration coverage for `SC-002` in `tests/Integration/MultiSource/MultiSourceResolutionTests.cs`
- [ ] T082 [US2] Add cycle-resolution, ambiguous-root, and invalid-main-source diagnostics coverage in `tests/Integration/Cycles/CycleResolutionTests.cs`, `tests/Integration/MultiSource/AmbiguousRootSelectionTests.cs`, and `tests/Integration/MultiSource/InvalidMainSourceTests.cs`

**Checkpoint**: Multi-file schema sets resolve correctly through the virtual file system and logical source identities.

---

## Phase 5: User Story 3 - Consume A Normalized Metadata Graph (Priority: P2)

**Goal**: Emit a dictionary-based JSON graph with canonical entries, `RefId` links, and enough constraint metadata for downstream XML validation.

**Independent Test**: Serialize a parsed graph and verify dictionary sections, reference-only child relationships, inheritance metadata, occurrence bounds, and validation rules without consulting original schemas.

### Folder Focus: Abstractions

- [ ] T083 [US3] Refine `src/Abstractions/IMetadataGraphBuilder.cs` to expose normalized graph assembly boundaries between discovery and linkage
- [ ] T084 [US3] Refine `src/Abstractions/IMetadataGraphSerializer.cs` with string and stream serialization entry points for the graph contract

### Folder Focus: Models

- [ ] T085 [US3] Update `src/Models/MetadataGraphModel.cs` with dictionary sections for complex types, simple types, elements, attributes, relationships, validation rules, and serializer hints
- [ ] T086 [US3] Update `src/Models/ConstraintSetModel.cs` with base-type, pattern, bounds, and enumeration metadata required for validation
- [ ] T087 [US3] Update `src/Models/ElementModel.cs` and `src/Models/AttributeModel.cs` with normalized occurrence and rule-reference fields

### Folder Focus: Registry

- [ ] T088 [US3] Implement canonical dictionary export ordering in `src/Registry/TypeRegistry.cs`
- [ ] T089 [US3] Implement registry-to-graph projection rules in `src/Registry/SchemaRegistryService.cs`

### Folder Focus: Parsing

- [ ] T090 [US3] Implement Pass 1 registration of complex and simple types in `src/Parsing/XsdGraphBuilder.cs`
- [ ] T091 [US3] Implement Pass 1 registration of elements and attributes in `src/Parsing/XsdGraphBuilder.cs`
- [ ] T092 [US3] Implement Pass 2 base-type and type-reference assignment in `src/Parsing/GraphLinkingService.cs`
- [ ] T093 [US3] Implement Pass 2 occurrence and validation-rule assignment in `src/Parsing/GraphLinkingService.cs`
- [ ] T094 [US3] Implement actionable diagnostics for unresolved imports, invalid schema content, and duplicate-definition conflicts in `src/Parsing/XsdParserService.cs` and `src/Parsing/WsdlParserService.cs`

### Supporting Serialization

- [ ] T095 [US3] Implement dictionary-based graph serialization in `src/Serialization/MetadataGraphJsonSerializer.cs`
- [ ] T096 [US3] Implement `RefId` wrapper serialization in `src/Serialization/Converters/RefIdJsonConverter.cs`
- [ ] T097 [US3] Implement occurrence-value formatting in `src/Serialization/Converters/OccurrenceValueJsonConverter.cs`
- [ ] T098 [US3] Implement constraint-set formatting in `src/Serialization/Converters/ConstraintSetJsonConverter.cs`

### Validation

- [ ] T099 [US3] Add metadata graph integration coverage for `SC-003` and `SC-004` in `tests/Integration/MetadataGraph/NormalizedGraphIntegrationTests.cs`
- [ ] T100 [US3] Add graph-only validation integration coverage for `SC-005` in `tests/Integration/MetadataGraph/GraphOnlyValidationMetadataTests.cs`

**Checkpoint**: Downstream consumers can validate XML using only the exported metadata graph.

---

## Phase 6: User Story 4 - Prepare Metadata For Rule-Based XML Generation (Priority: P3)

**Goal**: Preserve generation-relevant structure and rules, including reconstructable `xs:choice` exclusivity and deterministic anonymous-type identities.

**Independent Test**: Export a graph containing inheritance, choices, sequences, anonymous complex types, and rule metadata, then verify a downstream generator could distinguish branches and derive candidate XML structure.

### Folder Focus: Abstractions

- [ ] T101 [US4] Refine `src/Abstractions/IMetadataGraphBuilder.cs` with linkage requirements for generation-oriented metadata

### Folder Focus: Models

- [ ] T102 [US4] Update `src/Models/ComplexTypeModel.cs` with flattened compositor grouping metadata for `xs:choice` and `xs:sequence`
- [ ] T103 [US4] Update `src/Models/ElementModel.cs` with branch ordering and choice-group identifiers required by `FR-013b`
- [ ] T104 [US4] Update `src/Models/ConstraintSetModel.cs` with generation-oriented hints derived from preserved validation rules

### Folder Focus: Registry

- [ ] T105 [US4] Implement canonical grouping metadata storage for flattened choices in `src/Registry/TypeRegistry.cs`
- [ ] T106 [US4] Implement deterministic anonymous-type de-duplication safeguards in `src/Registry/SchemaRegistryService.cs`

### Folder Focus: Parsing

- [ ] T107 [US4] Implement flattened `xs:sequence` member ordering hints in `src/Parsing/GraphLinkingService.cs`
- [ ] T108 [US4] Implement flattened `xs:choice` grouping and exclusivity hints in `src/Parsing/GraphLinkingService.cs`
- [ ] T109 [US4] Implement anonymous complex type schema-path capture in `src/Parsing/XsdGraphBuilder.cs`
- [ ] T110 [US4] Implement generation-oriented rule projection in `src/Parsing/GraphLinkingService.cs`

### Supporting Serialization

- [ ] T111 [US4] Serialize compositor grouping metadata and generation hints in `src/Serialization/MetadataGraphJsonSerializer.cs` and `src/Serialization/Converters/ConstraintSetJsonConverter.cs`

### Validation

- [ ] T112 [US4] Add generation-metadata integration coverage for `SC-006` and `SC-007` in `tests/Integration/MetadataGraph/GenerationMetadataIntegrationTests.cs`

**Checkpoint**: Exported graphs preserve enough structure and rules for future rule-based XML generation.

---

## Final Phase: Polish & Cross-Cutting Concerns

- [ ] T113 Add XML documentation comments to all shared production declarations in `src/Abstractions/`, `src/Models/`, `src/Registry/`, `src/Parsing/`, `src/Serialization/`, and `src/Extensions/`
- [ ] T114 Update `specs/002-multi-source-parser/contracts/public-api.md` to reflect any naming or signature adjustments made during implementation
- [ ] T115 Update `specs/002-multi-source-parser/quickstart.md` with final DI setup and usage examples for `IVirtualFileSystem`, `VirtualFileSystemService`, `XsdParserService`, and `WsdlParserService`
- [ ] T116 Update `README.md` with package-level guidance for multi-source parsing and normalized metadata graph serialization
- [ ] T117 Verify the feature build and package metadata in `XsdXmlParser.csproj` against the completed `src/` layout and DI registrations
- [ ] T118 Add multi-target compatibility validation for `net6.0`, `net7.0`, and `net8.0` in `XsdXmlParser.csproj` and document the validation matrix in `README.md`
- [ ] T119 Add cancellation propagation coverage in `tests/Integration/SingleSource/CancellationPropagationTests.cs` and `tests/Integration/MultiSource/CancellationFlowTests.cs`
- [ ] T120 Add representative multi-file parsing performance validation in `tests/Integration/MultiSource/ParserPerformanceValidationTests.cs`
- [ ] T121 Add and review inline `Why` comments in complex traversal, canonicalization, and cycle-handling code under `src/Registry/` and `src/Parsing/`

## Dependencies

### Story order

- US1 depends on Phase 2 only.
- US2 depends on Phase 2 and builds on the single-source abstractions from US1.
- US3 depends on Phase 2 and the canonical discovery/linkage capabilities from US1 and US2.
- US4 depends on US3 because generation-oriented metadata extends the normalized graph contract.

### Task dependencies

- Phase 2 (T006-T045) blocks all story phases.
- US1 tasks depend on foundational abstractions, models, registry services, and parser scaffolding.
- US2 tasks depend on `IVirtualFileSystem`, batch source models, and import resolution scaffolding.
- US3 tasks depend on registry population and linkage behavior from US1 and US2.
- US4 tasks depend on the normalized graph shape and serializer behavior from US3.
- Final-phase validation and documentation tasks depend on completed story implementation and test assets.

## Parallel Execution Examples

### Per story

- US1: T046-T050 can proceed in parallel with T053-T058 after Phase 2 is complete.
- US2: T065-T067 can proceed in parallel with T071-T078 after Phase 2 is complete.
- US3: T085-T087 can proceed in parallel with T095-T098 after linkage requirements are stable.
- US4: T102-T104 can proceed in parallel with T107-T111 after US3 graph contracts are in place.

### Across stories

- Phase 2 tasks marked `[P]` can be distributed by folder across the team.
- US1 and US2 can overlap after Phase 2 if the team treats single-source and multi-source flows as separate workstreams.
- US3 can begin once registry population and linkage contracts stabilize from US1 and US2.
- Validation and documentation cleanup tasks `T118` through `T121` can run in parallel after feature implementation stabilizes.

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete US1 for single-source parsing.
3. Complete US2 for multi-file resolution.
4. Complete US3 for normalized graph export.
5. Validate the metadata graph contract before extending to US4.

### Incremental Delivery

1. Deliver source normalization and parser entry points.
2. Deliver multi-source import/include resolution.
3. Deliver canonical graph serialization.
4. Deliver generation-oriented metadata enhancements.

### Parallel Team Strategy

1. One engineer focuses on `Abstractions` and `Models`.
2. One engineer focuses on `Registry` and deterministic identity behavior.
3. One engineer focuses on `Parsing` and `Serialization` once foundational contracts land.
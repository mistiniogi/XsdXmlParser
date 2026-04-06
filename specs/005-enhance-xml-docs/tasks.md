# Tasks: Enhance XML Documentation Comments

**Input**: Design documents from `/specs/005-enhance-xml-docs/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: No new automated test tasks are generated because the feature is documentation-only. Validation tasks below use source-level declaration review, diff inspection, multi-target `dotnet build`, and representative generated XML documentation spot-checking.

**Organization**: Tasks are grouped by user story so complete declaration coverage, intent preservation, and selective example/reference enrichment can be delivered and reviewed independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., [US1], [US2], [US3])
- Every task includes exact file paths

## Phase 1: Setup (Documentation Inventory)

**Purpose**: Establish the all-visibility declaration inventory and validation baseline for the documentation-only pass

- [x] T001 Create the all-visibility declaration inventory and source-area checklist in specs/005-enhance-xml-docs/coverage-inventory.md
- [x] T002 [P] Update the source-first validation workflow and completion checklist in specs/005-enhance-xml-docs/coverage-inventory.md and specs/005-enhance-xml-docs/quickstart.md
- [x] T003 [P] Record the 55-file source inventory and all-visibility review baseline in specs/005-enhance-xml-docs/coverage-inventory.md and specs/005-enhance-xml-docs/contracts/xml-documentation-coverage-contract.md

---

## Phase 2: Foundational (Blocking Documentation Rules And Shared Terminology)

**Purpose**: Lock the XML documentation rules, inline reference-tag policy, and architectural vocabulary that every user story depends on

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Update the declaration-kind coverage matrix, all-visibility scope rule, and XML reference-tag policy in specs/005-enhance-xml-docs/coverage-inventory.md and specs/005-enhance-xml-docs/contracts/xml-documentation-coverage-contract.md
- [x] T005 [P] Record the detailed-comment and inline-reference guidance for `summary`, `param`, `typeparam`, `value`, `returns`, `remarks`, `example`, `exception`, `paramref`, `typeparamref`, `see`, and `seealso` in specs/005-enhance-xml-docs/research.md, specs/005-enhance-xml-docs/data-model.md, and specs/005-enhance-xml-docs/quickstart.md
- [x] T006 [P] Normalize shared parser, graph, registry, and source terminology anchors within XML documentation comments and inline XML reference tags in src/Abstractions/IMetadataGraphBuilder.cs, src/Abstractions/IMetadataGraphSerializer.cs, src/Abstractions/IParserOrchestrationService.cs, src/Parsing/ParserOrchestrationService.cs, src/Parsing/XsdGraphBuilder.cs, src/Registry/SchemaRegistryService.cs, and src/Serialization/MetadataGraphJsonSerializer.cs
- [x] T007 [P] Record all-visibility source-review checkpoints and note existing rationale-comment hotspots for reviewer attention in specs/005-enhance-xml-docs/coverage-inventory.md, referencing src/Parsing/ImportResolutionService.cs, src/Parsing/SchemaParsingHelper.cs, src/Parsing/WsdlDiscoveryService.cs, src/Parsing/XsdGraphBuilder.cs, and src/Parsing/ParserOrchestrationService.cs

**Checkpoint**: Coverage rules, detailed XML tag policy, and constitution-aligned terminology are locked for execution

---

## Phase 3: User Story 1 - Complete Reference Coverage (Priority: P1) 🎯 MVP

**Goal**: Deliver complete XML reference coverage for abstractions, DI entry points, and models across all declaration visibilities so the core surface is fully documented and independently reviewable

**Independent Test**: Review the abstraction, extension, and model files below against specs/005-enhance-xml-docs/coverage-inventory.md and confirm every in-scope declaration across all visibility levels has a complete XML comment block with all applicable tags.

### Implementation for User Story 1

- [x] T008 [P] [US1] Enhance orchestration and source-handling abstraction XML comments in src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/ISourceIdentityProvider.cs, src/Abstractions/ISourceLoader.cs, and src/Abstractions/IVirtualFileSystem.cs
- [x] T009 [P] [US1] Enhance parser, graph, and parsed-item abstraction XML comments in src/Abstractions/IMetadataGraphBuilder.cs, src/Abstractions/IMetadataGraphSerializer.cs, src/Abstractions/IParsedItemHandler.cs, src/Abstractions/IWsdlParser.cs, and src/Abstractions/IXsdParser.cs
- [x] T010 [P] [US1] Enhance DI entry-point XML comments and inline references in src/Extensions/ServiceCollectionExtensions.cs
- [x] T011 [P] [US1] Enhance request, source, diagnostic, and failure model XML comments in src/Models/FilePathParseRequestModel.cs, src/Models/ParseDiagnosticModel.cs, src/Models/ParseFailureException.cs, src/Models/ParseRequestModel.cs, src/Models/SourceDescriptorModel.cs, src/Models/StringParseRequestModel.cs, and src/Models/VirtualFileModel.cs
- [x] T012 [P] [US1] Enhance graph, registry, and enum model XML comments in src/Models/ERelationshipKind.cs, src/Models/ESchemaDocumentKind.cs, src/Models/ESourceKind.cs, src/Models/MetadataGraphModel.cs, src/Models/RegistryEntryModel.cs, src/Models/RelationshipModel.cs, and src/Models/SchemaRegistryEntryModel.cs
- [x] T013 [P] [US1] Enhance schema-type model XML comments in src/Models/AttributeModel.cs, src/Models/ComplexTypeModel.cs, src/Models/ConstraintSetModel.cs, src/Models/ElementModel.cs, and src/Models/SimpleTypeModel.cs
- [x] T014 [US1] Validate source-level coverage for all declaration visibilities and declaration kinds across every file in src/Abstractions/, src/Extensions/, and src/Models/ using specs/005-enhance-xml-docs/coverage-inventory.md and specs/005-enhance-xml-docs/quickstart.md

**Checkpoint**: Abstractions, DI entry points, and models are fully documented across all declaration visibilities and independently reviewable

---

## Phase 4: User Story 2 - Preserve Existing Documentation Intent (Priority: P1)

**Goal**: Enhance parsing, registry, and serialization documentation while preserving existing comment intent and architecture vocabulary

**Independent Test**: Compare representative before-and-after comment blocks in parsing, registry, and serialization files and confirm that existing accurate intent remains intact while missing tags, inline references, and clarifying detail are added.

### Implementation for User Story 2

- [x] T015 [P] [US2] Enhance parsed-item handler, helper, and parsing-context XML comments in src/Parsing/AttributeParsedItemHandler.cs, src/Parsing/ComplexTypeParsedItemHandler.cs, src/Parsing/ElementParsedItemHandler.cs, src/Parsing/ParsedItemContext.cs, src/Parsing/ParsedItemHandlerDispatcher.cs, src/Parsing/SchemaParsingHelper.cs, src/Parsing/SimpleTypeParsedItemHandler.cs, and src/Parsing/WsdlServiceParsedItemHandler.cs
- [x] T016 [P] [US2] Enhance orchestration, discovery, loading, graph-building, and lower-visibility helper XML comments in src/Parsing/GraphLinkingService.cs, src/Parsing/ImportResolutionService.cs, src/Parsing/ParserOrchestrationService.cs, src/Parsing/SourceIdentityProviderService.cs, src/Parsing/SourceLoaderService.cs, src/Parsing/VirtualFileSystemService.cs, src/Parsing/WsdlDiscoveryService.cs, src/Parsing/WsdlParserService.cs, src/Parsing/XsdGraphBuilder.cs, and src/Parsing/XsdParserService.cs
- [x] T017 [P] [US2] Enhance registry factory and registry service XML comments in src/Registry/RefIdFactory.cs, src/Registry/SchemaRegistryService.cs, src/Registry/SourceGraphRegistry.cs, and src/Registry/TypeRegistry.cs
- [x] T018 [P] [US2] Enhance serializer and converter XML comments in src/Serialization/MetadataGraphJsonSerializer.cs, src/Serialization/Converters/ConstraintSetJsonConverter.cs, src/Serialization/Converters/OccurrenceValueJsonConverter.cs, and src/Serialization/Converters/RefIdJsonConverter.cs
- [x] T019 [US2] Validate preserved intent and documentation-only edits across every file in src/Parsing/, src/Registry/, and src/Serialization/ using specs/005-enhance-xml-docs/coverage-inventory.md

**Checkpoint**: Parsing, registry, and serialization comments are richer, accurate, and still faithful to current behavior

---

## Phase 5: User Story 3 - Improve Consumer Understanding (Priority: P2)

**Goal**: Add selective examples, remarks, and inline reference tags where readers need extra help to understand intended usage and member interplay

**Independent Test**: Review the selected entry points and representative lower-visibility helpers and confirm a reader can understand invocation, usage expectations, and related parameter/member references from the XML comments alone without opening implementation logic.

### Implementation for User Story 3

- [x] T020 [P] [US3] Add selective `<remarks>`, `<example>`, and inline reference tags for consumer-facing orchestration and parser abstractions in src/Abstractions/IMetadataGraphSerializer.cs, src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IWsdlParser.cs, and src/Abstractions/IXsdParser.cs
- [x] T021 [P] [US3] Add selective `<remarks>`, `<example>`, and inline reference tags for DI, failure, and serialization entry points in src/Extensions/ServiceCollectionExtensions.cs, src/Models/ParseFailureException.cs, and src/Serialization/MetadataGraphJsonSerializer.cs
- [x] T022 [P] [US3] Refine high-value inline reference tags and detailed helper comments in src/Parsing/ParsedItemContext.cs, src/Parsing/ParsedItemHandlerDispatcher.cs, src/Parsing/SchemaParsingHelper.cs, src/Parsing/ImportResolutionService.cs, and src/Registry/RefIdFactory.cs
- [x] T023 [US3] Validate selective example and inline-reference coverage across all declarations identified as example or inline-reference candidates using specs/005-enhance-xml-docs/contracts/xml-documentation-coverage-contract.md and specs/005-enhance-xml-docs/quickstart.md

**Checkpoint**: Non-obvious APIs and helper relationships are documented with examples or inline references only where they add real value

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency review, source-level coverage verification, comment-only diff review, and multi-target validation across all stories

- [x] T024 Update final all-visibility coverage status for all 55 source files, skipped out-of-scope notes, and review signoff in specs/005-enhance-xml-docs/coverage-inventory.md
- [x] T025 [P] Run documentation-only diff review against src/Abstractions/IMetadataGraphBuilder.cs, src/Extensions/ServiceCollectionExtensions.cs, src/Models/MetadataGraphModel.cs, src/Parsing/ParserOrchestrationService.cs, src/Registry/SchemaRegistryService.cs, src/Serialization/MetadataGraphJsonSerializer.cs, and specs/005-enhance-xml-docs/coverage-inventory.md
- [x] T026 [P] Run multi-target build validation from XsdXmlParser.csproj with dotnet build
- [x] T027 [P] Spot-check generated XML documentation output for representative emitted declarations from src/Abstractions/IMetadataGraphSerializer.cs, src/Models/ParseFailureException.cs, src/Parsing/ParserOrchestrationService.cs, src/Registry/SchemaRegistryService.cs, and src/Serialization/MetadataGraphJsonSerializer.cs
- [x] T028 [P] Run a final source-level sweep across all 55 files in src/ for private, internal, protected, and public declarations to confirm full all-visibility coverage using specs/005-enhance-xml-docs/coverage-inventory.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; start immediately
- **Foundational (Phase 2)**: Depends on Phase 1; blocks all user stories
- **User Story 1 (Phase 3)**: Depends on Phase 2
- **User Story 2 (Phase 4)**: Depends on Phase 2
- **User Story 3 (Phase 5)**: Depends on Phase 2 and should layer on stabilized summaries from User Story 1 and User Story 2
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational; no dependency on other stories
- **User Story 2 (P1)**: Starts after Foundational; no dependency on other stories
- **User Story 3 (P2)**: Starts after Foundational, but should build on the stabilized summaries and preserved intent established in User Story 1 and User Story 2

### Within Each User Story

- Shared terminology and tag rules before large-scale comment expansion
- Summary and required tags before selective examples and inline references
- Coverage edits before validation
- Validation before polish

### Parallel Opportunities

- T002 and T003 can run in parallel after T001 starts the inventory refresh
- T005-T007 can run in parallel after T004 defines the all-visibility matrix and tag policy
- T008-T013 can run in parallel within User Story 1 because they touch different file groups
- T015-T018 can run in parallel within User Story 2 because they touch different file groups
- T020-T022 can run in parallel within User Story 3 because they touch different file groups
- T025-T028 can run in parallel during final validation

---

## Parallel Example: Foundational Phase

```bash
# Launch documentation-policy updates together after the coverage matrix is defined:
Task: "Record the detailed-comment and inline-reference guidance for summary, param, typeparam, value, returns, remarks, example, exception, paramref, typeparamref, see, and seealso in specs/005-enhance-xml-docs/research.md, specs/005-enhance-xml-docs/data-model.md, and specs/005-enhance-xml-docs/quickstart.md"
Task: "Record all-visibility source-review checkpoints and note existing rationale-comment hotspots for reviewer attention in specs/005-enhance-xml-docs/coverage-inventory.md, referencing src/Parsing/ImportResolutionService.cs, src/Parsing/SchemaParsingHelper.cs, src/Parsing/WsdlDiscoveryService.cs, src/Parsing/XsdGraphBuilder.cs, and src/Parsing/ParserOrchestrationService.cs"
```

---

## Parallel Example: User Story 1

```bash
# Launch independent comment-coverage work across abstractions and models:
Task: "Enhance parser, graph, and parsed-item abstraction XML comments in src/Abstractions/IMetadataGraphBuilder.cs, src/Abstractions/IMetadataGraphSerializer.cs, src/Abstractions/IParsedItemHandler.cs, src/Abstractions/IWsdlParser.cs, and src/Abstractions/IXsdParser.cs"
Task: "Enhance schema-type model XML comments in src/Models/AttributeModel.cs, src/Models/ComplexTypeModel.cs, src/Models/ConstraintSetModel.cs, src/Models/ElementModel.cs, and src/Models/SimpleTypeModel.cs"
```

---

## Parallel Example: User Story 2

```bash
# Launch independent service-area documentation work after shared terminology is locked:
Task: "Enhance orchestration, discovery, loading, graph-building, and lower-visibility helper XML comments in src/Parsing/GraphLinkingService.cs, src/Parsing/ImportResolutionService.cs, src/Parsing/ParserOrchestrationService.cs, src/Parsing/SourceIdentityProviderService.cs, src/Parsing/SourceLoaderService.cs, src/Parsing/VirtualFileSystemService.cs, src/Parsing/WsdlDiscoveryService.cs, src/Parsing/WsdlParserService.cs, src/Parsing/XsdGraphBuilder.cs, and src/Parsing/XsdParserService.cs"
Task: "Enhance serializer and converter XML comments in src/Serialization/MetadataGraphJsonSerializer.cs, src/Serialization/Converters/ConstraintSetJsonConverter.cs, src/Serialization/Converters/OccurrenceValueJsonConverter.cs, and src/Serialization/Converters/RefIdJsonConverter.cs"
```

---

## Parallel Example: User Story 3

```bash
# Launch example and inline-reference enrichment in parallel once core summaries are stable:
Task: "Add selective <remarks>, <example>, and inline reference tags for consumer-facing orchestration and parser abstractions in src/Abstractions/IMetadataGraphSerializer.cs, src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IWsdlParser.cs, and src/Abstractions/IXsdParser.cs"
Task: "Refine high-value inline reference tags and detailed helper comments in src/Parsing/ParsedItemContext.cs, src/Parsing/ParsedItemHandlerDispatcher.cs, src/Parsing/SchemaParsingHelper.cs, src/Parsing/ImportResolutionService.cs, and src/Registry/RefIdFactory.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate complete all-visibility coverage for abstractions, extensions, and models with T014 before broader final validation with T026 and T028

### Incremental Delivery

1. Finish Setup + Foundational to lock scope, terminology, source-first validation, and XML tag rules
2. Deliver User Story 1 for complete reference coverage on abstractions, DI entry points, and models
3. Deliver User Story 2 for intent-preserving parsing, registry, and serialization documentation
4. Deliver User Story 3 for selective examples, remarks, and inline references on non-obvious entry points and helpers
5. Finish with Phase 6 source-level verification, diff review, and build validation

### Parallel Team Strategy

1. One developer refreshes the coverage inventory and tag policy in Phase 1-2
2. After Foundational completes:
   Developer A handles User Story 1 abstraction and model coverage
   Developer B handles User Story 2 parsing and registry coverage
   Developer C handles User Story 2 serialization coverage and User Story 3 example/reference enrichment after summaries stabilize
3. Complete Phase 6 source review and build validation together

---

## Notes

- `[P]` tasks touch different file groups and can be coordinated in parallel
- The implementation is intentionally limited to XML documentation comments in `src/`
- Generated files, test files, `README.md`, and `docs/` remain out of scope
- Validation is source-first because the revised scope includes declaration visibilities that are not fully represented in generated XML documentation output
- Generated XML documentation is still used as a secondary spot-check for representative emitted declarations
- Each user story remains independently reviewable even though all work contributes to one documentation-only feature
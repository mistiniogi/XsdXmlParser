# Tasks: Simplify Source Loading Inputs

**Input**: Design documents from `/specs/004-simplify-source-loading/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: No new automated test tasks are generated because the feature spec and plan explicitly exclude adding new test logic. Validation tasks below use the retained quickstart scenarios, contract review, and `dotnet build`.

**Organization**: Tasks are grouped by user story so file-backed support, string-backed support, and public-surface reduction can be delivered and validated independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., [US1], [US2], [US3])
- Every task includes exact file paths

## Phase 1: Setup (Shared Model Preparation)

**Purpose**: Create the shared request-model surface required by the retained source-loading workflows

- [X] T001 Create the retained string request model in src/Models/StringParseRequestModel.cs
- [X] T002 [P] Update shared request-model XML documentation for explicit document kind and logical-path semantics in src/Models/ParseRequestModel.cs and src/Models/FilePathParseRequestModel.cs

---

## Phase 2: Foundational (Blocking Contract And Infrastructure Updates)

**Purpose**: Narrow the shared contracts and verify constitution-mandated DI and registry flow before story-specific implementation begins

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T003 Update the primary loader contract to retain only file and string workflows in src/Abstractions/ISourceLoader.cs
- [X] T004 [P] Update the primary orchestration contract to retain ParseFileAsync and add ParseStringAsync in src/Abstractions/IParserOrchestrationService.cs
- [X] T005 [P] Update typed parser adapters to retain only file/string entry points and document them as secondary seams in src/Abstractions/IXsdParser.cs and src/Abstractions/IWsdlParser.cs
- [X] T006 Update dependency-injection registration and XML documentation for the narrowed service graph in src/Extensions/ServiceCollectionExtensions.cs
- [X] T007 [P] Update retained source-origin and descriptor reporting for the narrowed contract in src/Models/ESourceKind.cs and src/Models/SourceDescriptorModel.cs
- [X] T008 Verify shared source-loading rationale comments and centralized registry handoff expectations in src/Parsing/SourceLoaderService.cs and src/Parsing/ParserOrchestrationService.cs

**Checkpoint**: Shared contracts, DI registration, and source-descriptor flow all reflect the retained file/string-only surface

---

## Phase 3: User Story 1 - Load Schema Content From File (Priority: P1) 🎯 MVP

**Goal**: Preserve file-backed WSDL/XSD loading as a first-class workflow under the narrowed contract

**Independent Test**: Run the file-backed scenarios in specs/004-simplify-source-loading/quickstart.md against representative WSDL and XSD files and confirm the retained file entry points normalize requests into the existing parsing flow.

### Implementation for User Story 1

- [X] T009 [US1] Update file request validation and descriptor creation for explicit document-kind handling in src/Parsing/SourceLoaderService.cs
- [X] T010 [US1] Update the retained file parsing flow in src/Parsing/ParserOrchestrationService.cs
- [X] T011 [US1] Update file-based parser convenience methods in src/Parsing/XsdParserService.cs and src/Parsing/WsdlParserService.cs
- [X] T012 [P] [US1] Refresh file-workflow XML documentation in src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IXsdParser.cs, and src/Abstractions/IWsdlParser.cs
- [X] T013 [US1] Validate representative WSDL and XSD file workflows against specs/004-simplify-source-loading/quickstart.md and XsdXmlParser.csproj

**Checkpoint**: File-backed parsing remains functional and independently validated

---

## Phase 4: User Story 2 - Load Schema Content From String (Priority: P1)

**Goal**: Add a supported string-backed WSDL/XSD workflow that uses explicit document kind and required logical paths

**Independent Test**: Run the string-backed scenarios in specs/004-simplify-source-loading/quickstart.md against representative WSDL and XSD strings and confirm the retained string entry points normalize input and resolve related files from the supplied logical path.

### Implementation for User Story 2

- [X] T014 [US2] Implement string-backed source normalization with logical-path validation and virtual-file registration in src/Parsing/SourceLoaderService.cs
- [X] T015 [US2] Add the retained string parsing flow to src/Parsing/ParserOrchestrationService.cs
- [X] T016 [US2] Add string-based parser convenience methods in src/Parsing/XsdParserService.cs and src/Parsing/WsdlParserService.cs
- [X] T017 [P] [US2] Update string-workflow XML documentation in src/Models/StringParseRequestModel.cs, src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IXsdParser.cs, and src/Abstractions/IWsdlParser.cs
- [X] T018 [US2] Validate representative WSDL and XSD string workflows plus logical-path-based related-file resolution against specs/004-simplify-source-loading/quickstart.md and XsdXmlParser.csproj

**Checkpoint**: String-backed parsing is available and independently validated

---

## Phase 5: User Story 3 - Work Against One Reduced Input Surface (Priority: P2)

**Goal**: Remove unsupported stream, memory, and batch source-loading workflows from the public contract and guidance

**Independent Test**: Review the public contracts and consumer guidance and confirm that only file-path and string-content workflows remain exposed after the unsupported request types and entry points are removed.

### Implementation for User Story 3

- [X] T019 [US3] Remove unsupported request model files src/Models/StreamParseRequestModel.cs, src/Models/MemoryParseRequestModel.cs, src/Models/BatchParseRequestModel.cs, and src/Models/BatchSourceRequestModel.cs
- [X] T020 [US3] Remove unsupported loader methods and validation paths from src/Abstractions/ISourceLoader.cs and src/Parsing/SourceLoaderService.cs
- [X] T021 [US3] Remove unsupported orchestration and parser entry points from src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IXsdParser.cs, src/Abstractions/IWsdlParser.cs, src/Parsing/ParserOrchestrationService.cs, src/Parsing/XsdParserService.cs, and src/Parsing/WsdlParserService.cs
- [X] T022 [P] [US3] Remove unsupported workflow references and replace examples in README.md and docs/getting-started.md
- [X] T023 [US3] Validate reduced public-surface documentation against specs/004-simplify-source-loading/contracts/public-source-loading-contract.md and specs/004-simplify-source-loading/quickstart.md

**Checkpoint**: The public contract and consumer guidance now expose only file and string workflows

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency, failure-path validation, and multi-target verification across all stories

- [X] T024 Update remaining source-loading XML documentation and rationale comments across src/Models/StringParseRequestModel.cs, src/Parsing/SourceLoaderService.cs, src/Parsing/ParserOrchestrationService.cs, src/Parsing/XsdParserService.cs, and src/Parsing/WsdlParserService.cs
- [X] T025 [P] Validate retained invalid-input scenarios from specs/004-simplify-source-loading/quickstart.md for missing file, empty string, missing logical path, and unresolved related-file behavior using XsdXmlParser.csproj
- [X] T026 [P] Validate equivalent file-backed and string-backed readiness for the same representative WSDL or XSD content using specs/004-simplify-source-loading/quickstart.md and XsdXmlParser.csproj
- [X] T027 Run multi-target build validation from XsdXmlParser.csproj with dotnet build

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; start immediately
- **Foundational (Phase 2)**: Depends on Phase 1; blocks all user-story work
- **User Story 1 (Phase 3)**: Depends on Phase 2
- **User Story 2 (Phase 4)**: Depends on Phase 2
- **User Story 3 (Phase 5)**: Depends on Phase 2 and should be applied after the retained file/string workflows are stable
- **Polish (Phase 6)**: Depends on the desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational; no dependency on other stories
- **User Story 2 (P1)**: Starts after Foundational; no dependency on User Story 1
- **User Story 3 (P2)**: Starts after Foundational and integrates the retained outcomes from User Story 1 and User Story 2 into one reduced public contract

### Within Each User Story

- Shared contracts before service implementations
- Source normalization before parser convenience methods
- Service implementation before story-specific validation
- Code changes before consumer-documentation cleanup

### Parallel Opportunities

- T002 can run in parallel with T001 after the feature branch is ready
- T004, T005, and T007 can run in parallel after T003 defines the primary contract boundary
- T012 can run in parallel with T011 once T010 defines the retained file flow
- T017 can run in parallel with T016 once T015 defines the retained string flow
- T022 can run in parallel with T019-T021 after removed workflows are confirmed
- T025 and T026 can run in parallel during final validation

---

## Parallel Example: User Story 1

```bash
# Launch retained file workflow implementation and documentation together after the shared contract is ready:
Task: "Update file-based parser convenience methods in src/Parsing/XsdParserService.cs and src/Parsing/WsdlParserService.cs"
Task: "Refresh file-workflow XML documentation in src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IXsdParser.cs, and src/Abstractions/IWsdlParser.cs"
```

---

## Parallel Example: User Story 2

```bash
# Launch retained string workflow documentation and adapter work together after normalization is defined:
Task: "Add string-based parser convenience methods in src/Parsing/XsdParserService.cs and src/Parsing/WsdlParserService.cs"
Task: "Update string-workflow XML documentation in src/Models/StringParseRequestModel.cs, src/Abstractions/IParserOrchestrationService.cs, src/Abstractions/IXsdParser.cs, and src/Abstractions/IWsdlParser.cs"
```

---

## Parallel Example: User Story 3

```bash
# Launch contract cleanup and consumer-doc cleanup together after removed workflows are confirmed:
Task: "Remove unsupported request model files src/Models/StreamParseRequestModel.cs, src/Models/MemoryParseRequestModel.cs, src/Models/BatchParseRequestModel.cs, and src/Models/BatchSourceRequestModel.cs"
Task: "Remove unsupported workflow references and replace examples in README.md and docs/getting-started.md"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate the retained file-backed workflow with T013 and T027

### Incremental Delivery

1. Finish Setup + Foundational to lock the retained contract and DI/registry verification points
2. Deliver User Story 1 for file-backed parsing continuity
3. Deliver User Story 2 for string-backed parsing support
4. Deliver User Story 3 to remove unsupported workflows and clean the public surface
5. Finish with Phase 6 failure-path and multi-target validation

### Parallel Team Strategy

1. One developer handles Phase 1 and Phase 2 shared contracts and infrastructure verification
2. After Foundational completes:
   - Developer A: User Story 1 file workflow
   - Developer B: User Story 2 string workflow
3. User Story 3 cleanup and Phase 6 validation follow once retained workflows are stable

---

## Notes

- `[P]` tasks touch different files and can be coordinated in parallel
- User story tasks are intentionally limited to the file/string source-loading change requested in the spec
- Validation is performed through quickstart execution, contract review, and `dotnet build` instead of new automated tests
- Preserve existing parser behavior outside the narrowed source-loading surface

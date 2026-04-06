# Tasks: WSDL XML Parser

**Input**: Design documents from `/specs/001-wsdl-xml-parser/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/, quickstart.md

**Tests**: Tests are integrated per user story (xUnit with Moq and integration tests).

**Organization**: Tasks are grouped by user story to enable independent implementation, validation, and parallel work.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story?]**: User story label for story-specific tasks only
- Include exact file paths in descriptions

## Path Conventions

- Source: `src/`
- Tests: `tests/`
- Follow plan.md project structure with Models/, Services/, Parsers/, Validators/, Generators/, Extensions/

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize project structure, DI, testing, and static analysis support.

- [ ] T001 Configure `XsdXmlParser.csproj` for multi-targeting net6.0;net7.0;net8.0, package identity `XsdXmlParser.Core`, and C# 10.0 compatibility
- [ ] T002 Create `src/` folders: Models/, Services/Abstractions/, Services/Implementations/, Parsers/, Validators/, Generators/, Extensions/
- [ ] T003 Create `tests/` folders: Unit/Parsers/, Unit/Services/, Unit/Validators/, Unit/Generators/, Integration/
- [ ] T004 Add `.editorconfig` at repository root with StyleCop, FxCop, SonarAnalyzer, and AsyncFixer rules
- [ ] T005 Add unit test project references for xUnit and Moq
- [ ] T006 Add GitHub workflow skeleton for build, test, and analyzer validation
- [ ] T007 Add documentation placeholders for `README.md`, `ARCHITECTURE.md`, and `API.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build core schema models, parser infrastructure, DI registration, and exception handling.

**⚠️ CRITICAL**: This phase must complete before implementation of all user stories.

- [ ] T008 [P] Create `src/Models/SchemaModel.cs` with Id, Name, Namespace, Elements, Types, and Imports
- [ ] T009 [P] Create `src/Models/ElementModel.cs` with Name, Type, MinOccurs, MaxOccurs, Attributes, Children, and Documentation
- [ ] T010 [P] Create `src/Models/TypeModel.cs` with Name, BaseType, IsSimple, IsComplex, and Facets
- [ ] T011 [P] Create `src/Models/JSONMetadata.cs` with Schema, Metadata, and GeneratedAt
- [ ] T012 [P] Create `src/Models/XMLOutput.cs` with RootElement, Content, and ValidationErrors
- [ ] T013 [P] Create `src/Models/TypeRegistry.cs` as a centralized ID-based singleton service for element parser and type registration
- [ ] T014 [P] Create `src/Services/Abstractions/IWSDLParser.cs` per public API contract
- [ ] T015 [P] Create `src/Services/Abstractions/IXSDParser.cs` per public API contract
- [ ] T016 [P] Create `src/Services/Abstractions/IJSONGenerator.cs` per public API contract
- [ ] T017 [P] Create `src/Services/Abstractions/IXMLGenerator.cs` per public API contract
- [ ] T018 [P] Create exception types in `src/Exceptions/`: `InvalidWsdlException.cs`, `InvalidXsdException.cs`, `InvalidDataException.cs`
- [ ] T019 [P] Create `src/Extensions/ServiceCollectionExtensions.cs` with public `AddXsdXmlParser()` extension method
- [ ] T020 [P] Create `src/Parsers/BaseElementParser.cs` with abstract parse/validation contract and stateless helper methods
- [ ] T021 [P] Create unit test scaffolding for all service interfaces in `tests/Unit/Services/`
- [ ] T022 [P] Define async-first service contracts in `src/Services/Abstractions/` with `ParseAsync`/`GenerateAsync` methods and `CancellationToken` support
- [ ] T023 [P] Create a benchmark harness and representative 10,000-element schema fixture in `tests/Performance/` for early parser validation across net6.0, net7.0, and net8.0
- [ ] T024 [P] Create async cancellation and concurrent parsing test scaffolding in `tests/Integration/AsyncExecutionTests.cs`

**Checkpoint**: Foundation complete; all story implementations can begin in parallel.

---

## Phase 3: User Story 1 - Parse WSDL 1.1 files (Priority: P1) 🎯 MVP

**Goal**: Parse WSDL 1.1 content into `SchemaModel` with service, binding, portType, message, and embedded schema extraction.

**Independent Test Criteria**: Parse a WSDL 1.1 document and verify the resulting `SchemaModel` includes extracted definitions, proper namespaces, and embedded XSD delegation.

- [ ] T025 [US1] Create `src/Services/Implementations/WSDLParserService.cs` implementing `IWSDLParser`
- [ ] T026 [P] [US1] Create `src/Parsers/WsdlElementParser.cs` for shared WSDL parsing helpers
- [ ] T027 [US1] Implement WSDL service extraction in `WSDLParserService`
- [ ] T028 [US1] Implement WSDL binding extraction in `WSDLParserService`
- [ ] T029 [US1] Implement WSDL portType extraction in `WSDLParserService`
- [ ] T030 [US1] Implement WSDL message extraction in `WSDLParserService`
- [ ] T031 [US1] Implement embedded XSD schema extraction in `WSDLParserService` and delegate to `IXSDParser`
- [ ] T032 [P] [US1] Implement WSDL namespace resolution and prefix handling
- [ ] T033 [US1] Implement strict error handling with `InvalidWsdlException` and line/column context
- [ ] T034 [US1] Create unit tests in `tests/Unit/Services/WSDLParserServiceTests.cs`
- [ ] T035 [P] [US1] Create per-element WSDL parser classes in `src/Parsers/ElementParsers/` for service, binding, portType, and message

**Checkpoint**: US1 can be validated independently with unit tests.

---

## Phase 4: User Story 2 - Parse XSD schemas (Priority: P1)

**Goal**: Parse XSD schemas into `SchemaModel` with named elements, types, facets, attributes, and annotations.

**Independent Test Criteria**: Parse sample XSD and verify `SchemaModel` contains all named elements/types, facet metadata, and annotation documentation.

- [ ] T036 [US2] Create `src/Services/Implementations/XSDParserService.cs` implementing `IXSDParser`
- [ ] T037 [P] [US2] Create `src/Parsers/XsdElementParser.cs` for shared XSD parsing helpers
- [ ] T038 [US2] Implement global element parsing for named XSD elements in `XSDParserService`
- [ ] T039 [US2] Implement complex type parsing in `XSDParserService`
- [ ] T040 [US2] Implement simple type parsing in `XSDParserService`
- [ ] T041 [US2] Implement restriction/facet parsing for length, minLength, maxLength, pattern, enumeration, totalDigits, fractionDigits, minInclusive, maxInclusive, minExclusive, maxExclusive
- [ ] T042 [US2] Implement sequence/choice parsing and cardinality handling in `XSDParserService`
- [ ] T043 [US2] Implement attribute parsing for global and local attributes in `XSDParserService`
- [ ] T044 [P] [US2] Implement xs:annotation/xs:documentation parsing and attach metadata to models
- [ ] T045 [US2] Implement import/include schema resolution and cross-file type lookup
- [ ] T046 [US2] Implement namespace-aware type resolution in `TypeRegistry`
- [ ] T047 [US2] Enforce canonical ID assignment and duplicate-definition elimination in `TypeRegistry`
- [ ] T048 [US2] Implement extension/restriction inheritance support in `XSDParserService`
- [ ] T049 [US2] Implement recursive type detection and safe graph traversal state tracking
- [ ] T050 [US2] Implement strict error handling with `InvalidXsdException`
- [ ] T051 [US2] Create one independent `ElementParser` class per named XSD element in `src/Parsers/ElementParsers/`
- [ ] T052 [US2] Create unit tests in `tests/Unit/Services/XSDParserServiceTests.cs`
- [ ] T053 [P] [US2] Create facet validator classes in `src/Validators/` for length, pattern, enumeration, and range constraints

**Checkpoint**: US2 can be validated independently with unit tests.

---

## Phase 5: User Story 3 - Generate JSON metadata (Priority: P1)

**Goal**: Generate JSON metadata from `SchemaModel` with schema graph serialization, facets, and documentation.

**Independent Test Criteria**: Generate JSON for a parsed schema and verify the output contains elements, types, facets, and documentation metadata.

- [ ] T054 [US3] Create `src/Services/Implementations/JSONMetadataGeneratorService.cs` implementing `IJSONGenerator`
- [ ] T055 [US3] Implement schema graph traversal for JSON serialization
- [ ] T056 [US3] Serialize `SchemaModel` metadata using `System.Text.Json`
- [ ] T057 [US3] Serialize `ElementModel` details with type references and cardinality
- [ ] T058 [US3] Serialize `TypeModel` details with base type, facets, inheritance, and canonical registry IDs
- [ ] T059 [US3] Serialize facet constraints as structured JSON objects
- [ ] T060 [US3] Serialize documentation metadata with schema annotations
- [ ] T061 [P] [US3] Apply camelCase property naming in JSON output
- [ ] T062 [US3] Include namespace mappings and registry IDs in JSON metadata output
- [ ] T063 [US3] Create unit tests in `tests/Unit/Services/JSONMetadataGeneratorServiceTests.cs`

**Checkpoint**: US3 can be validated independently with unit tests.

---

## Phase 6: User Story 4 - Generate XML output (Priority: P1)

**Goal**: Generate valid XML from `SchemaModel` and sample data with schema-aware ordering, namespaces, and validation.

**Independent Test Criteria**: Generate XML from schema and sample data, verify the output is well-formed and respects schema constraints.

- [ ] T064 [US4] Create `src/Services/Implementations/XMLGeneratorService.cs` implementing `IXMLGenerator`
- [ ] T065 [US4] Implement object-to-XML mapping for schema elements
- [ ] T066 [US4] Enforce element ordering based on sequence/choice definitions
- [ ] T067 [US4] Enforce cardinality rules and raise `InvalidDataException` for violations
- [ ] T068 [US4] Apply facet validation during XML generation
- [ ] T069 [P] [US4] Implement XML namespace declaration and prefix handling
- [ ] T070 [US4] Implement recursive complex type element generation
- [ ] T071 [US4] Implement attribute generation from data objects
- [ ] T072 [US4] Implement XML instance validation against `SchemaModel`
- [ ] T073 [US4] Create unit tests in `tests/Unit/Services/XMLGeneratorServiceTests.cs`

**Checkpoint**: US4 can be validated independently with unit tests.

---

## Phase 7: User Story 5 - Provide public API with DI integration (Priority: P1)

**Goal**: Expose library services via `AddXsdXmlParser()` and verify DI registration/resolution.

**Independent Test Criteria**: Register services with DI, resolve parser/generator services, and verify `TypeRegistry` is singleton and reusable.

- [ ] T074 [US5] Implement `AddXsdXmlParser()` in `src/Extensions/ServiceCollectionExtensions.cs`
- [ ] T075 [US5] Register `IWSDLParser` → `WSDLParserService` as scoped
- [ ] T076 [US5] Register `IXSDParser` → `XSDParserService` as scoped
- [ ] T077 [US5] Register `IJSONGenerator` → `JSONMetadataGeneratorService` as scoped
- [ ] T078 [US5] Register `IXMLGenerator` → `XMLGeneratorService` as scoped
- [ ] T079 [US5] Register `TypeRegistry` as singleton with initialization support
- [ ] T080 [US5] Implement factory-based `TypeRegistry` initialization in DI registration
- [ ] T081 [US5] Ensure public API namespaces are exposed via root project namespace
- [ ] T082 [P] [US5] Create integration test in `tests/Integration/DIRegistrationTests.cs`
- [ ] T083 [US5] Create integration test for WSDL parse + JSON generation scenario
- [ ] T084 [US5] Create integration test for XSD parse + XML generation scenario

**Checkpoint**: US5 can be validated independently with integration tests.

---

## Final Phase: Polish & Cross-Cutting Concerns

- [ ] T085 Configure CI pipeline to run StyleCop, FxCop, SonarAnalyzer, and test suites
- [ ] T086 Verify all public methods and parser helpers comply with SRP and remain under 30 lines where reasonable
- [ ] T087 Add XML documentation comments to all public classes and methods
- [ ] T088 Add `<summary>`, `<param>`, `<returns>`, and `<exception>` XML tags for public API members
- [ ] T089 Update `README.md` with installation, quickstart, DI setup, and examples
- [ ] T090 Add `ARCHITECTURE.md` documenting Clean Architecture, DI patterns, async execution, C# 10 compatibility, and TypeRegistry behavior
- [ ] T091 Add `API.md` documenting all public types, interfaces, and extension methods
- [ ] T092 Update `specs/001-wsdl-xml-parser/quickstart.md` with final API usage examples
- [ ] T093 Run final performance benchmark validation for parsing 10,000-element schemas
- [ ] T094 Add memory profiling or footprint verification for large schema parsing
- [ ] T095 Add `CHANGELOG.md` summarizing v1.0.0 release features
- [ ] T096 Verify build and test against `net6.0`, `net7.0`, and `net8.0`
- [ ] T097 Run full test suite with coverage reporting for core parsing logic
- [ ] T098 Perform final static analysis pass and address findings

## Dependencies

**Story order**:
- US1 and US2 can be implemented in parallel after Phase 2
- US3 and US4 depend on US1/US2 and can be parallel with each other
- US5 depends on US1-US4

**Task dependencies**:
- Phase 2 (T008-T024) blocks all story phases
- US1 tasks depend on WSDL parsing service + shared parser base
- US2 tasks depend on XSD parsing service + shared parser base
- US3 tasks depend on parsed schema graph from US1/US2
- US4 tasks depend on parsed schema graph from US1/US2
- US5 tasks depend on completed parser and generator services

## Parallel Execution Examples

**Per story**:
- US1: T026, T032, T035 can run in parallel with service implementation tasks
- US2: T037, T044, T045, T053 can run in parallel with XSD service implementation
- US3: T055-T062 are parallelizable once generator service scaffold exists
- US4: T066-T071 are parallelizable once generator service scaffold exists
- US5: T075-T080 are parallelizable once registration API is defined

**Across stories**:
- Phase 2: T008-T024 are strongly parallelizable
- US1 and US2: parallel after Phase 2, with the benchmark harness available for early validation
- US3 and US4: parallel after US1/US2
- US5: parallel after parser/generator services stabilize

## Implementation Strategy

**MVP First**: Complete Phase 2, then US1 (WSDL parsing), US2 (XSD parsing), US3 (JSON generation), and US5 (DI API) for a minimal library delivery.

**Incremental Delivery**: Each story is independently testable and deployable.

**Parallel Development**: Multiple developers can implement separate stories simultaneously after Phase 2.
</content>
<parameter name="filePath">/Users/sndjones/Documents/SUBHADEEP/PROJECTS/GitHub/XsdXmlParser/specs/001-wsdl-xml-parser/tasks.md
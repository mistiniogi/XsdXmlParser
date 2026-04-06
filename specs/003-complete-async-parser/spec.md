# Feature Specification: Complete WSDL/XSD Parsing Workflows

**Feature Branch**: `003-complete-async-parser`  
**Created**: 2026-04-06  
**Status**: Draft  
**Input**: User description: "Create a new spec. The current c# code is missing the main parsing logic to parse the loaded wsdl / xsd files. Add parsing logic with seperation of concern. CReate new classes for every different type of item passed. Use inheritence and polymorphism. Modify the existing code to add async await patterns to all methods. Also define the input service (s) and method(s) for the library. Create documentation on how to use it. Remove creating any kind of test or test logic. We will concentrate only on main logic in this spec."

## Clarifications

### Session 2026-04-06

- Q: How should the public parsing API be organized for consumers? → A: One consumer-facing orchestration service with asynchronous single-source and multi-source parsing methods.
- Q: Which input forms should the consumer-facing orchestration service support? → A: File path, stream, read-only memory, and batch source requests.
- Q: How should the library handle invalid sources within a parse request? → A: Fail the entire parse request and return no partial metadata output.
- Q: How should parsing failures be surfaced to callers? → A: Throw exceptions for all parse failures.
- Q: How should the library determine whether a supplied source is WSDL or XSD? → A: Require the caller to specify the source kind on every request.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Parse Supplied Schema Content End To End (Priority: P1)

As a library consumer, I want the parser to fully process supplied WSDL and XSD content into the library's metadata output so that I can use the package for real parsing work instead of only loading source files.

**Why this priority**: Without end-to-end parsing, the library does not deliver its primary value and downstream consumers cannot rely on it.

**Independent Validation**: Submit valid WSDL and XSD inputs through the public library entry points and confirm that the parser returns populated metadata output that reflects definitions, relationships, and diagnostics from the supplied sources.

**Acceptance Scenarios**:

1. **Given** a valid single-source XSD input, **When** the consumer invokes the library parse workflow, **Then** the library returns metadata that includes the discovered schema items and their relationships.
2. **Given** a valid WSDL input that references schema content, **When** the consumer invokes the library parse workflow, **Then** the library returns metadata that includes both service-level and schema-derived information needed by downstream consumers.

---

### User Story 2 - Use Clear Input Workflows (Priority: P1)

As a library consumer, I want one clear consumer-facing parsing service with dedicated request methods for file-path, stream, read-only-memory, and batch-source parsing workflows so that I can choose the correct workflow without guessing how the library should be called.

**Why this priority**: Even correct parsing logic is difficult to adopt if the public entry points are ambiguous or incomplete.

**Independent Validation**: Review the public library contract and execute the documented input workflows for a single source and a multi-source request, confirming that each workflow accepts the intended request shape and returns a consistent parse result.

**Acceptance Scenarios**:

1. **Given** a consumer with one logical source to parse from a file path, stream, or read-only memory buffer, **When** the consumer uses the consumer-facing orchestration service's supported single-source workflow, **Then** the library accepts the request and returns a parse result without requiring internal knowledge of parser components.
2. **Given** a consumer with multiple related sources, **When** the consumer uses the same consumer-facing orchestration service's batch-source workflow, **Then** the library accepts the batch request and resolves it through the same public contract family used for other parsing operations.
3. **Given** a consumer supplies a parsing request, **When** the request is created, **Then** the consumer specifies whether each supplied source is WSDL or XSD as part of the public contract.

---

### User Story 3 - Process Different Parsed Item Categories Independently (Priority: P2)

As a maintainer, I want different categories of parsed items to be processed through dedicated responsibilities with a shared behavioral contract so that item-specific logic can evolve without destabilizing unrelated parsing behavior.

**Why this priority**: The missing parser logic spans multiple schema item categories, and maintainability depends on isolating their responsibilities while preserving consistent outcomes.

**Independent Validation**: Parse sources that contain multiple item categories, then confirm that each category contributes the expected metadata and diagnostics without requiring a single monolithic parsing path.

**Acceptance Scenarios**:

1. **Given** source material containing elements, attributes, complex types, and simple types, **When** the parser processes the source, **Then** each item category is handled through its own dedicated parsing responsibility while contributing to one unified result.
2. **Given** new item-specific rules for one parsed category, **When** that category is updated, **Then** unrelated item categories continue to behave consistently through the shared parsing contract.

---

### User Story 4 - Run Parsing Through Non-Blocking Workflows (Priority: P2)

As a library consumer, I want parsing operations to be available through asynchronous workflows so that the library can be integrated into applications that depend on non-blocking execution and cancellation-aware orchestration.

**Why this priority**: The library already handles external content and multi-source processing, so blocking-only workflows create unnecessary integration friction.

**Independent Validation**: Invoke each public parse workflow from an asynchronous caller and confirm that the operation completes through the non-blocking contract while still returning the expected metadata and diagnostics.

**Acceptance Scenarios**:

1. **Given** a consumer calling a supported parse workflow from an asynchronous application path, **When** the parse begins, **Then** the consumer can await completion through the public library contract.
2. **Given** an asynchronous parse request that fails because of invalid input, **When** the operation completes, **Then** the consumer receives the same actionable error information through the asynchronous workflow.

---

### User Story 5 - Follow Published Usage Guidance (Priority: P3)

As a new adopter, I want documentation that explains how to construct requests, invoke the library, and interpret parse results so that I can integrate the package without reverse-engineering the source code.

**Why this priority**: Documentation does not create parsing capability by itself, but it is required for adoption and reduces incorrect usage.

**Independent Validation**: Ask a consumer unfamiliar with the codebase to follow the published usage guide and complete a sample parsing flow successfully without consulting internal implementation details.

**Acceptance Scenarios**:

1. **Given** a new consumer reading the usage guide, **When** the consumer follows the documented steps for a supported input workflow, **Then** the consumer can execute a successful parse request and understand the resulting output.
2. **Given** a consumer troubleshooting an invalid parse request, **When** the consumer consults the documentation, **Then** the guide explains the expected inputs, result shape, and common failure conditions clearly enough to correct the request.

### Edge Cases

- When a supplied source loads successfully but contains no parseable WSDL or XSD definitions, the library throws a parse failure exception that makes the absence of parseable items explicit.
- When a batch-source request contains a mix of valid and invalid sources, the library fails the entire request, reports which source failed, and returns no partial metadata output.
- When a caller omits the source kind for a supplied input or supplies a source kind that does not match the content contract, the library fails the request with a parse failure that identifies the offending source.
- When two parsed item categories depend on each other across source boundaries, the library preserves the relationship in the final metadata result instead of dropping one side of the link.
- When an asynchronous parse is canceled or interrupted by caller intent, the library stops work predictably and reports the outcome without leaving consumers with an ambiguous result state.
- When documentation examples are followed against supported sample inputs, the described request and result flow matches the actual library behavior.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The library MUST parse supplied WSDL sources into the library's metadata output instead of stopping after source loading or discovery.
- **FR-002**: The library MUST parse supplied XSD sources into the library's metadata output instead of stopping after source loading or discovery.
- **FR-003**: The library MUST expose one consumer-facing orchestration service as the primary public entry point for parsing operations.
- **FR-004**: The consumer-facing orchestration service MUST provide clearly defined asynchronous methods for file-path, stream, read-only-memory, and batch-source parsing requests.
- **FR-005**: Each public parsing workflow MUST define the required input information, the resulting output shape, and the exception-based failure contract in a way that consumers can use without knowledge of internal parser components.
- **FR-005e**: Every parsing request MUST require the caller to declare the source kind for each supplied input as WSDL or XSD.
- **FR-005f**: If a request omits the declared source kind or supplies an unsupported source kind, the library MUST fail the request before parsing begins.
- **FR-005a**: File-path parsing MUST accept a logical file location supplied by the consumer.
- **FR-005b**: Stream parsing MUST accept stream content supplied by the consumer.
- **FR-005c**: Read-only-memory parsing MUST accept an in-memory binary payload supplied by the consumer.
- **FR-005d**: Batch-source parsing MUST accept a coordinated collection of source requests supplied by the consumer.
- **FR-006**: The library MUST return one consistent successful parse result model across supported parsing workflows.
- **FR-006a**: If any supplied source is invalid for the active parse request, the library MUST fail the entire request rather than returning partial metadata output.
- **FR-006b**: The library MUST throw exceptions for parse failures instead of returning a failure-state parse result.
- **FR-007**: The parsing domain MUST separate item-category responsibilities so that different parsed item types can be processed through dedicated parsing behavior.
- **FR-008**: The parsing domain MUST provide one shared behavioral contract for supported parsed item categories so that item-specific workflows contribute to a unified parse result consistently.
- **FR-009**: Item-category parsing behavior MUST preserve the relationships required to connect parsed definitions across WSDL and XSD sources.
- **FR-010**: The parser MUST support the schema item categories required by the current metadata model, including elements, attributes, simple types, complex types, and service-related definitions that originate from supported WSDL content.
- **FR-011**: All public parsing workflows MUST be available through asynchronous library operations.
- **FR-012**: Asynchronous parsing workflows MUST preserve the same functional outcomes and error reporting semantics across every supported public parse workflow.
- **FR-013**: The library MUST support caller-controlled cancellation for long-running or multi-source parsing workflows.
- **FR-014**: The library MUST throw parse failure exceptions that identify the logical source and parsing stage that caused the failure when that information is available.
- **FR-015**: The library MUST distinguish source-loading failures from parser-processing failures in its thrown failures so consumers can identify whether a request failed before or during schema interpretation.
- **FR-016**: The library MUST document every supported input workflow, including how to construct requests, invoke the parsing entry point, and interpret successful and failed results.
- **FR-017**: The library MUST include at least one usage example for single-source parsing and one usage example for multi-source parsing in its published guidance.
- **FR-018**: Documentation MUST describe the minimum input data required for each supported workflow and the meaning of the main output fields and diagnostics.
- **FR-019**: C# types and members introduced or changed by the feature MUST define XML documentation requirements where they affect the shared production surface.
- **FR-020**: Complex logic paths MUST identify hotspots that require inline Why comments when implementation work begins.

### Key Entities *(include if feature involves data)*

- **Parse Request**: The consumer-supplied instruction that identifies the source content, source context, and parsing mode to execute.
- **Source Kind**: The caller-supplied classification that identifies each source as WSDL or XSD before parsing begins.
- **Batch Source Request**: The consumer-supplied collection of related source requests that the orchestration service parses as one coordinated operation.
- **Parse Workflow**: A supported library entry path for handling one source or a coordinated set of related sources.
- **Parsing Orchestration Service**: The primary consumer-facing service that accepts parse requests and routes them through the appropriate internal parsing responsibilities.
- **Parse Result**: The consumer-visible successful outcome of a parse request, containing produced metadata when parsing completes without failure.
- **Parse Failure Exception**: The consumer-visible failure signal raised when a parsing operation cannot complete successfully.
- **Parsed Item Category**: A distinct class of schema or service definition that requires dedicated parsing behavior while still participating in the shared result model.
- **Source Diagnostic**: The structured failure detail carried by a thrown parsing exception to identify a parse problem, the affected source, and the stage where the issue occurred.
- **Usage Guide**: The published consumer documentation that explains how to invoke supported parse workflows and interpret results.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: During feature validation, 100% of supported public parse workflows produce populated metadata output for valid representative WSDL and XSD inputs.
- **SC-002**: During feature validation, consumers can execute both the single-source and multi-source parsing workflows using only the published public contract and documentation.
- **SC-003**: For representative mixed-content schema sets, 100% of required parsed item categories appear in the resulting metadata output with their expected relationships preserved.
- **SC-004**: During feature validation, 100% of public parsing workflows are invokable through asynchronous application code without requiring blocking wrappers.
- **SC-005**: For representative invalid inputs, 100% of observed failures identify whether the request failed during source loading or during parser processing.
- **SC-005a**: For representative requests containing at least one invalid source, 100% of failed operations return no partial metadata output.
- **SC-005b**: For representative invalid inputs, 100% of failed operations surface through thrown parse failures instead of failure-state result objects.
- **SC-006**: A new consumer can complete the documented sample parsing flows and correctly interpret the result and diagnostics on the first attempt in at least 90% of onboarding trials.

## Assumptions

- The existing metadata model remains the target output contract and this feature fills in the missing parser behavior needed to populate it.
- The initial release of this feature focuses on completing parsing for the item categories already implied by the current library domain rather than inventing entirely new output concepts.
- Single-source and multi-source parsing remain first-class workflows and should share one coherent result contract even if their request shapes differ.
- File path, stream, read-only memory, and batch source requests are all first-class supported input forms for the consumer-facing orchestration service.
- Callers explicitly declare the source kind for every supplied input rather than relying on library-side auto-detection.
- Any invalid source in a request causes the full operation to fail rather than producing partial metadata.
- Parse failures are surfaced through exceptions rather than failure-state result objects.
- One consumer-facing orchestration service is the intended public API shape, while WSDL/XSD and item-specific parsing responsibilities remain internal separations of concern.
- Non-blocking parsing is required across the public library surface, but the feature does not require separate synchronous-only entry points if the asynchronous contract is sufficient for consumers.
- The feature scope excludes automated tests, test harnesses, and other test-only logic so implementation effort stays focused on the main parsing behavior and consumer-facing documentation.
- Consumers need documentation that is maintained within the repository and versioned alongside the library code.
- Usage guidance should reflect the real public contract delivered by the feature and is considered incomplete if it only describes internal services or source code structure.

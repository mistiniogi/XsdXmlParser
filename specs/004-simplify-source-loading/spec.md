# Feature Specification: Simplify Source Loading Inputs

**Feature Branch**: `004-simplify-source-loading`  
**Created**: 2026-04-06  
**Status**: Draft  
**Input**: User description: "Create new spec to remove different support for SourceLoaderService as I do not need it. Provide only support to load from string or from file for either WSDL or XSD. No other logic changes are required except this change in the solution."

## Clarifications

### Session 2026-04-06

- Q: How should the library determine whether a supplied source is WSDL or XSD for the retained file and string workflows? → A: Require the caller to explicitly declare WSDL or XSD for the primary file and string request-model workflows; typed parser adapters may imply document kind from the abstraction.
- Q: How should the library handle the removed stream, memory, and batch source-loading APIs? → A: Remove the stream, memory, and batch source-loading APIs entirely.
- Q: How should the library treat logical paths for retained string inputs? → A: Require every string input to provide a logical path.
- Q: How should retained string inputs handle relative imports and includes? → A: String inputs may reference related files from the filesystem using the required logical path as the resolution base.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Load Schema Content From File (Priority: P1)

As a library consumer, I want to load a WSDL or XSD source directly from a file path so that local schema files remain a first-class input workflow.

**Why this priority**: File-backed loading is already part of the current library behavior and must remain available after the surface is simplified.

**Independent Test**: Submit representative WSDL and XSD file paths through the supported file-loading entry point and confirm that the library accepts them as valid source inputs without requiring any alternate source form.

**Acceptance Scenarios**:

1. **Given** a valid XSD file path, **When** the consumer invokes the supported file-loading workflow, **Then** the library accepts the file as a schema source and passes it into the existing parsing flow.
2. **Given** a valid WSDL file path, **When** the consumer invokes the supported file-loading workflow, **Then** the library accepts the file as a service-definition source and passes it into the existing parsing flow.

---

### User Story 2 - Load Schema Content From String (Priority: P1)

As a library consumer, I want to supply WSDL or XSD content as a string so that I can parse generated or dynamically assembled schema text without writing it to disk first.

**Why this priority**: The requested simplification still needs a non-file workflow, and string-based input is the only retained in-memory path.

**Independent Test**: Submit representative WSDL and XSD content strings through the supported string-loading entry point and confirm that the library accepts them as valid source inputs only when the caller supplies the logical path needed for resolution, including relative references to related files on disk.

**Acceptance Scenarios**:

1. **Given** a valid XSD document supplied as a string, **When** the consumer invokes the supported string-loading workflow, **Then** the library accepts the content and passes it into the existing parsing flow.
2. **Given** a valid WSDL document supplied as a string, **When** the consumer invokes the supported string-loading workflow, **Then** the library accepts the content and passes it into the existing parsing flow.

---

### User Story 3 - Work Against One Reduced Input Surface (Priority: P2)

As a maintainer, I want the source-loading contract reduced to only file and string inputs so that the library surface is easier to understand and maintain.

The primary supported contract is request-model-based orchestration, while typed parser adapters remain narrower convenience seams.

**Why this priority**: The user explicitly wants the extra loading modes removed, but this is secondary to preserving the two retained workflows.

**Independent Test**: Review the supported loading contracts and confirm that only file-path and string-content workflows remain available while previous stream, memory-buffer, and multi-source loading paths are no longer supported.

**Acceptance Scenarios**:

1. **Given** a consumer reviewing the supported source-loading workflows, **When** the public contract is inspected, **Then** only file and string inputs are described and exposed.
2. **Given** an existing caller attempts to use a removed non-file, non-string loading workflow, **When** the caller upgrades to this feature, **Then** the unsupported workflow is no longer available as part of the supported contract.

### Edge Cases

- When a caller supplies an empty string or whitespace-only schema content, the library rejects the request before parsing begins.
- When a caller supplies string content without a logical path, the library rejects the request before parsing begins.
- When a caller supplies a file path that does not exist or cannot be resolved, the library rejects the request before parsing begins.
- When a caller omits the declared document kind or declares the wrong document kind for a file or string input, the library fails the request with a clear source-loading error.
- When a retained string input references a related file that cannot be resolved from the supplied logical path, the library fails the request with a clear source-loading or resolution error rather than silently ignoring the reference.
- When the same schema content is provided through file and string workflows, the downstream parsing behavior remains materially consistent.
- When an existing caller upgrades and still depends on a removed stream, memory, or batch input path, that workflow is absent from the public contract rather than silently rerouted through another input type.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The library MUST support loading WSDL sources from a file-backed input workflow.
- **FR-002**: The library MUST support loading XSD sources from a file-backed input workflow.
- **FR-003**: The library MUST support loading WSDL sources from a string-backed input workflow.
- **FR-004**: The library MUST support loading XSD sources from a string-backed input workflow.
- **FR-005**: The supported source-loading contract MUST be limited to file-backed and string-backed workflows only.
- **FR-006**: Stream-backed, memory-buffer-backed, batch-source, and any other non-file, non-string source-loading workflows MUST be removed from the public source-loading contract entirely.
- **FR-007**: The file-backed workflow MUST preserve the existing ability to identify the supplied source and pass it into the current parsing flow.
- **FR-008**: The string-backed workflow MUST require the caller to provide the source content together with the logical path needed by downstream resolution behavior.
- **FR-009**: The primary retained file and string request-model workflows MUST require the caller to explicitly declare whether the supplied source is WSDL or XSD.
- **FR-010**: Retained string inputs MUST be allowed to resolve related files from the filesystem using the caller-supplied logical path as the resolution base.
- **FR-011**: Invalid file requests, invalid string requests, requests missing the required string logical path, unresolved related-file references for retained string inputs, and requests with missing or incorrect declared document kinds MUST fail during source loading with clear validation feedback.
- **FR-012**: This feature MUST not introduce unrelated parsing behavior changes outside the source-loading contract adjustments required to support only file and string inputs.
- **FR-013**: The library MUST continue to support both WSDL and XSD documents through each retained input workflow.
- **FR-014**: Public-facing guidance and API descriptions MUST reflect that only file and string loading workflows are supported after this feature.
- **FR-015**: Public-facing guidance and API descriptions MUST reflect that `IParserOrchestrationService` and request-model workflows require explicit document-kind declaration, while typed parser adapters imply document kind from their abstraction.
- **FR-016**: Public-facing guidance and API descriptions MUST explain how retained string inputs use the required logical path to resolve related files.
- **FR-017**: C# types and members introduced or changed by the feature MUST define XML documentation requirements where they affect the shared production surface.
- **FR-018**: Complex logic paths MUST identify hotspots that require inline Why comments when implementation work begins.

### Key Entities *(include if feature involves data)*

- **File Source Input**: A consumer-supplied file location representing one WSDL or XSD document to load.
- **String Source Input**: A consumer-supplied text document representing one WSDL or XSD source together with its required logical path.
- **Logical Path**: The caller-supplied base path used to identify a retained string input and resolve its relative file references.
- **Source-Loading Contract**: The supported set of input workflows that callers can use to hand source material to the parser.
- **Source Descriptor**: The normalized representation produced by source loading and consumed by the existing parsing flow.
- **Source-Loading Failure**: A validation or resolution failure that occurs before schema parsing begins.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: During feature validation, 100% of supported source-loading entry points are file-backed or string-backed, with no remaining supported stream, memory, or batch loading paths.
- **SC-002**: During feature validation, representative WSDL and XSD documents load successfully through the file-backed workflow in 100% of test runs.
- **SC-003**: During feature validation, representative WSDL and XSD documents load successfully through the string-backed workflow in 100% of test runs.
- **SC-004**: For representative invalid file paths and empty string inputs, 100% of failures are reported before parsing begins.
- **SC-005**: For the same representative WSDL or XSD content provided through file and string inputs, 100% of validation runs reach equivalent downstream parsing readiness.
- **SC-006**: Documentation and public contract review show only the two retained workflows in 100% of reviewed source-loading references.

## Assumptions

- The existing downstream parsing pipeline remains in place and only the source-loading surface is being simplified.
- String input means textual WSDL or XSD content supplied directly by the caller rather than binary buffers or streams.
- Callers explicitly declare whether each retained file or string input is WSDL or XSD when using the primary request-model workflow; typed parser adapters supply that distinction through the interface type.
- Callers always provide the logical path needed for resolution when loading from string content.
- Retained string inputs continue to participate in existing relative import and include resolution against filesystem-backed related files using the supplied logical path as the base.
- Multi-source coordination, stream input, and raw memory input are intentionally removed from scope rather than preserved behind compatibility shims.
- Removed source-loading workflows are deleted from the supported public contract rather than retained as compatibility wrappers or obsoleted placeholders.
- Supporting WSDL and XSD remains mandatory for both retained input workflows.
- Any required public API or documentation updates are limited to reflecting the reduced source-loading scope and do not imply broader parser redesign.

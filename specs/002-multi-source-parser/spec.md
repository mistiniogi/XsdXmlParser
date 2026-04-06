# Feature Specification: Multi-Source XSD/WSDL Parser Library

**Feature Branch**: `002-multi-source-parser`  
**Created**: 2026-04-05  
**Status**: Draft  
**Input**: User description: "Define the specification for a multi-source XSD/WSDL parser library with support for FilePath, Stream, ReadOnlyMemory<byte>, and IEnumerable<Stream> inputs; main-file based root resolution for multi-file loads; normalized JSON metadata graph output; dictionary-based de-duplication using RefId references; and enough metadata to validate XML and later generate random XML without the original XSD."

## Clarifications

### Session 2026-04-05

- Q: How should circular schema imports across different memory streams be handled? → A: Allow circular import/include chains, detect them by logical source identity, and resolve them through cycle-safe graph linking with one canonical registry entry per source.
- Q: How should unique RefIds be generated for anonymous complex types? → A: Use a deterministic structural key based on logical source identity, parent RefId, schema path, and local ordinal.
- Q: How should the JSON metadata graph represent `xs:choice` versus `xs:sequence` for UI rendering? → A: Flatten both into one child list and expose ordering or occurrence hints on members instead of explicit compositor nodes.
- Q: How should conflicting duplicate definitions across supplied sources be handled? → A: Fail the parse when the same logical schema item appears with conflicting content.
- Q: How should non-seekable stream inputs be handled? → A: Reject non-seekable streams with actionable diagnostics.
- Q: How should batch sources missing unique logical source identities be handled? → A: Fail the parse request immediately with actionable diagnostics.
- Q: How should invalid main-source designation be handled in batch parsing? → A: Fail the parse request before import/include resolution begins.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Parse From Any Supported Source (Priority: P1)

As a library consumer, I want to load schema content from a single file path, a stream, or an in-memory byte buffer so that I can parse schemas regardless of where the content originates.

**Why this priority**: Source flexibility is the minimum usable value for the library. Without it, downstream normalization and validation workflows cannot start.

**Independent Test**: Provide equivalent schema content through a file path, a stream, and a memory buffer, then verify that each input path produces the same normalized metadata graph for the same source material.

**Acceptance Scenarios**:

1. **Given** a valid schema available as a file path, **When** the consumer parses it, **Then** the library returns a normalized metadata graph for that schema.
2. **Given** a valid schema available as a stream or memory buffer, **When** the consumer parses it, **Then** the library returns the same logical metadata graph that would be produced from the original file.

---

### User Story 2 - Resolve Multi-File Schema Sets (Priority: P1)

As a library consumer, I want to load multiple related schema files together and optionally designate a main file so that imports and includes resolve correctly across a schema set.

**Why this priority**: Real-world XSD and WSDL assets are commonly split across multiple files. Correct root resolution is essential to produce a complete graph.

**Independent Test**: Load a multi-file schema set with imports/includes, designate one file as the main document, and verify that cross-file references resolve into one complete metadata graph without duplicate stored definitions.

**Acceptance Scenarios**:

1. **Given** multiple related schema files and a designated main file, **When** the consumer parses the batch, **Then** relative imports and includes are resolved from that main file context using the supplied logical source identities.
2. **Given** multiple files with shared type definitions, **When** the consumer parses the batch, **Then** the output graph stores each unique definition once and references it consistently.

---

### User Story 3 - Consume A Normalized Metadata Graph (Priority: P2)

As a downstream consumer, I want the parser to return a normalized JSON metadata graph so that I can validate XML documents and later generate rule-based XML without requiring the original schema files.

**Why this priority**: The parser’s strategic value is not just parsing input files, but producing reusable metadata that powers later workflows.

**Independent Test**: Parse a representative schema set, export the metadata graph, and verify that the graph contains enough structural and constraint metadata to validate a sample XML document without consulting the original schema files.

**Acceptance Scenarios**:

1. **Given** a parsed schema set, **When** the metadata graph is exported, **Then** all complex types, simple types, elements, and attributes appear in unique top-level dictionaries and child relationships use reference IDs instead of nested definitions.
2. **Given** a metadata graph and a candidate XML document, **When** a downstream validator uses only that graph, **Then** the validator has access to occurrence rules, inheritance details, and pattern constraints required to determine conformance.

---

### User Story 4 - Prepare Metadata For Rule-Based XML Generation (Priority: P3)

As a future generator consumer, I want the metadata graph to retain generation-relevant constraints and relationships so that random XML can later be generated from the graph alone.

**Why this priority**: This extends the library’s usefulness beyond validation and avoids future rework in the metadata model.

**Independent Test**: Parse a schema containing inheritance, occurrences, and pattern constraints, then confirm that the exported graph preserves enough normalized metadata for a separate generation component to derive valid candidate values and tree structure.

**Acceptance Scenarios**:

1. **Given** schema definitions that include base types, occurrence bounds, and value constraints, **When** the graph is exported, **Then** those rules are preserved in a normalized form that another component can interpret later.

### Edge Cases

- When a batch input includes multiple candidate root files and no main file is specified, the library fails the request with actionable diagnostics that identify the candidate roots and require an explicit main-source designation.
- When a designated main source does not match any supplied batch source, the library fails the parse request before import/include resolution begins and reports actionable diagnostics for the invalid designation.
- When a batch source is supplied without a unique logical source identity, the library fails the parse request immediately with actionable diagnostics instead of deriving or guessing an identity.
- When duplicate definitions appear across supplied sources for the same logical schema item and their content conflicts, the library fails the parse request with actionable diagnostics rather than choosing or merging a definition implicitly.
- When import/include chains remain incomplete because a referenced source is missing from the supplied batch after cycle-safe resolution, the library fails the parse request with actionable diagnostics that identify the unresolved logical source identity or path.
- When `xs:choice` is flattened for UI-oriented JSON output, the library preserves exclusivity semantics through member-level grouping metadata, compositor kind, and branch ordering hints so downstream consumers can reconstruct mutually exclusive branches.
- When one canonical child definition is referenced under multiple parents, the metadata graph preserves one canonical registry entry and emits parent-to-child relationships through `RefId` links rather than duplicating the child definition under each parent.
- When a supplied stream is non-seekable, cannot be re-read, or its content is empty, the library fails the parse request with actionable diagnostics instead of buffering or retrying implicitly.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The library MUST accept schema input from a file path.
- **FR-002**: The library MUST accept schema input from a stream.
- **FR-002a**: Stream inputs MUST be readable and seekable; non-seekable stream inputs MUST fail the parse request with actionable diagnostics.
- **FR-003**: The library MUST accept schema input from a read-only memory buffer of bytes.
- **FR-004**: The library MUST accept batch schema input from a collection of streams.
- **FR-004a**: Each stream in a batch input MUST be accompanied by a unique logical source identity that the library can use for import/include matching, main-file designation, diagnostics, and cycle detection.
- **FR-004b**: If any batch source is missing a unique logical source identity, the library MUST fail the parse request before import/include resolution begins and report actionable diagnostics for the offending source.
- **FR-005**: The library MUST support both WSDL and XSD source material within the supported input modes.
- **FR-006**: When multiple files are provided, the library MUST allow the consumer to designate one supplied file as the main file for resolving relative imports and includes.
- **FR-006a**: When multiple supplied sources could act as root documents and no main file is designated, the library MUST fail the parse request with actionable diagnostics instead of choosing an implicit main source.
- **FR-006b**: If a designated main source does not match one of the supplied batch sources, the library MUST fail the parse request before import/include resolution begins and report actionable diagnostics for the invalid designation.
- **FR-007**: When a main file is designated, the library MUST resolve relative import/include paths from that main file context.
- **FR-007a**: The library MUST allow circular import/include chains across supplied sources, detect them by logical source identity, and resolve them through cycle-safe graph linking.
- **FR-007b**: For non-file inputs, relative import/include resolution MUST use the consumer-supplied logical source identity and logical path metadata rather than runtime stream object identity.
- **FR-008**: The library MUST produce a normalized JSON metadata graph as its output representation.
- **FR-009**: The metadata graph MUST store complex types, simple types, elements, and attributes in unique top-level dictionaries.
- **FR-010**: Child nodes in the metadata graph MUST reference other nodes by `RefId` keys instead of embedding nested object definitions.
- **FR-011**: The library MUST de-duplicate equivalent type and member definitions across all parsed sources so that each logical definition is stored once.
- **FR-011a**: Definitions for the same logical schema item MAY share one canonical registry entry only when their content is equivalent; conflicting duplicates MUST fail the parse request with actionable diagnostics.
- **FR-012**: The library MUST maintain a centralized registry of canonical schema definitions and identifiers while building the metadata graph.
- **FR-012a**: Anonymous complex types MUST receive deterministic `RefId` values derived from logical source identity, parent `RefId`, schema path, and local ordinal so that repeated parses yield stable graph identities.
- **FR-012b**: The schema path used for anonymous complex type `RefId` generation MUST be the normalized declaration path from the owning source root to the anonymous type, and the local ordinal MUST be the zero-based ordinal of anonymous complex type declarations under the same parent in source order before graph normalization.
- **FR-013**: The metadata graph MUST preserve occurrence constraints, including minimum and maximum occurrences, for all applicable elements and attributes.
- **FR-013a**: For UI-oriented JSON output, `xs:choice` and `xs:sequence` constructs MUST be flattened into child-member lists, with ordering and occurrence metadata preserved on the participating members instead of separate compositor nodes.
- **FR-013b**: When `xs:choice` is flattened, the metadata graph MUST still preserve reconstructable exclusivity semantics by emitting member-level grouping metadata such as a shared choice-group identifier, compositor kind, and branch ordering hints.
- **FR-014**: The metadata graph MUST preserve base type relationships and inheritance information needed to interpret derived definitions.
- **FR-015**: The metadata graph MUST preserve value constraints needed for downstream validation, including pattern-style restrictions and comparable rule metadata.
- **FR-016**: The metadata graph MUST preserve enough structural and constraint information for downstream XML validation without requiring access to the original XSD files.
- **FR-017**: The metadata graph MUST preserve enough structural and constraint information for a future rule-based XML generation workflow.
- **FR-018**: Parsing and batch-loading operations MUST be available through asynchronous library workflows.
- **FR-019**: The library MUST report unresolved imports, includes, invalid main-file selection, and duplicate-definition conflicts with actionable error details.
- **FR-019a**: The library MUST report unreadable, empty, non-seekable, and invalid schema sources with actionable error details that include the logical source identity when available.

### Key Entities *(include if feature involves data)*

- **Source Descriptor**: A consumer-supplied schema source entry that identifies the incoming content and, when applicable, the logical file identity used for multi-file resolution.
- **Logical Source Identity**: The stable, consumer-visible identifier assigned to each source so imports/includes, diagnostics, cycle detection, and main-source selection remain deterministic across files, streams, and memory-backed inputs.
- **Main Source**: The designated root document within a multi-file load that anchors relative import/include resolution.
- **Metadata Graph**: The normalized output artifact that contains all canonical schema dictionaries, reference IDs, relationships, and validation-oriented constraints.
- **Registry Entry**: The canonical stored form of a complex type, simple type, element, or attribute in the centralized registry, referenced by a stable ID.
- **Reference Link**: A child-to-parent or consumer-to-definition link that uses a `RefId` to point to another registry entry instead of duplicating a nested object.
- **Constraint Set**: The collection of validation and generation-oriented rules attached to a registry entry, such as occurrences, base-type lineage, and pattern restrictions.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Equivalent schema content provided through file path, stream, and memory-buffer inputs yields the same logical metadata graph in 100% of acceptance tests.
- **SC-002**: For representative multi-file schema sets used in acceptance testing, 100% of valid relative imports and includes resolve correctly when a main file is designated.
- **SC-003**: In acceptance datasets containing repeated definitions, every logical complex type, simple type, element, and attribute appears exactly once in the corresponding top-level dictionary.
- **SC-004**: In exported graphs used for acceptance testing, 100% of child-definition relationships are represented through `RefId` links rather than nested duplicate objects.
- **SC-005**: A downstream validator can determine pass/fail results for the acceptance XML set using only the exported metadata graph and no original schema access.
- **SC-006**: The exported graph retains all occurrence, inheritance, and rule metadata required by the acceptance scenarios for later XML generation.
- **SC-007**: In acceptance datasets containing flattened `xs:choice` structures, downstream consumers can still distinguish mutually exclusive branches from ordered sequence members using only the exported metadata graph.

## Assumptions

- The designated main file, when provided, refers to one of the supplied files in the current parse request.
- Invalid main-source designations are rejected during request validation rather than deferred until import/include resolution.
- When more than one supplied source can serve as a root document, callers are expected to designate a main source or the parse request will fail with diagnostics that list the candidate roots.
- Relative import/include resolution is limited to content explicitly supplied to the library for the parse operation.
- For stream and memory-backed batch inputs, callers supply logical source identities and logical path metadata that remain stable for the duration of the parse request.
- Batch requests with missing or non-unique logical source identities are invalid and are rejected before schema resolution begins.
- Stream inputs are expected to remain readable and seekable for the duration of the parse request; non-seekable streams are rejected rather than buffered implicitly.
- Circular import/include chains are valid when the referenced sources can be matched to supplied logical source identities and resolved without conflicting canonical definitions.
- The normalized schema path used for anonymous complex type `RefId` generation is derived from declaration ancestry within the owning source before de-duplication and graph flattening are applied.
- The first release focuses on normalized metadata generation; actual XML validation and random XML generation are downstream consumers of the graph rather than part of this feature.
- Equivalent definitions can be treated as duplicates only when their identity and content are consistent enough to represent one canonical registry entry; conflicting duplicates are treated as parse failures rather than merge candidates.

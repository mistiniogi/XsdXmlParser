# Research: Multi-Source XSD/WSDL Parser Library

## Source Identity Strategy

- Decision: Use a unified source descriptor abstraction for file paths, streams, memory buffers, and batch members.
- Rationale: Multi-source resolution, cycle detection, and deterministic RefId generation all need a logical identity that survives across input forms.
- Alternatives considered: Separate loader pipelines per input type. Rejected because it duplicates resolution logic and weakens consistency across inputs.

## Circular Import Resolution

- Decision: Allow circular import/include chains and resolve them with cycle-safe graph linking keyed by logical source identity.
- Rationale: Real schema sets can contain valid cycles, especially across modularized XSD packages. The parser needs to avoid infinite recursion while still producing a complete normalized graph.
- Alternatives considered: Reject all cycles. Rejected because it would block valid real-world schemas and conflict with the chosen clarification.

## Virtual File System Abstraction

- Decision: Introduce `IVirtualFileSystem` to abstract physical files, logical paths, in-memory buffers, and caller-supplied streams behind one schema-resolution boundary.
- Rationale: `XmlSchemaSet` integration and import/include traversal need a common access layer that works equally for disk-backed and memory-backed schema sets.
- Alternatives considered: direct file-system access and bespoke stream lookup dictionaries. Rejected because they break testability and leave non-file inputs under-specified.

## Two-Pass Parsing Strategy

- Decision: Parse in two passes: Pass 1 performs type discovery and populates the registry; Pass 2 assigns links, references, and flattened child metadata.
- Rationale: The graph cannot safely link forward references, derived types, and cyclic imports until all canonical definitions exist.
- Alternatives considered: single-pass build-and-link traversal. Rejected because it makes duplicate detection and stable reference assignment brittle.

## Component Responsibilities

- Decision: `SchemaRegistryService` owns canonical registration and lookup, `XsdGraphBuilder` owns XSD discovery and graph-shell construction, and `WsdlDiscoveryService` owns WSDL-level schema and service discovery.
- Rationale: these are the cleanest seams for SRP, DI, and testability.
- Alternatives considered: one large parser service. Rejected because it merges source loading, discovery, registry logic, and linkage into one unit.

## JSON Serialization Strategy

- Decision: Use `System.Text.Json` for the normalized graph and add custom converters only where the default serializer does not produce the required contract shape.
- Rationale: this keeps the serialization stack standard while allowing control over dictionary-backed graph details and special values.
- Alternatives considered: custom handwritten serialization for the whole graph. Rejected because it adds unnecessary complexity.

## Anonymous Complex Type RefIds

- Decision: Generate anonymous complex type RefIds deterministically from logical source identity, parent RefId, schema path, and local ordinal.
- Rationale: Stable graph identity is required for reproducible outputs, regression testing, and de-duplication.
- Alternatives considered: Random GUIDs and content-only hashes. Rejected because GUIDs are unstable and content-only hashes lose parent-location context.

## Flattened JSON Representation

- Decision: Flatten `xs:choice` and `xs:sequence` into member lists for the exported JSON while preserving ordering and occurrence hints on members.
- Rationale: The output is intended for UI rendering as well as later validation and generation workflows; flattening reduces consumer traversal complexity.
- Alternatives considered: Explicit compositor nodes. Rejected because the feature clarification explicitly chose flattened UI output.

## Canonical Registry Design

- Decision: Store complex types, simple types, elements, and attributes in dedicated top-level dictionaries, with child relationships expressed only through RefIds.
- Rationale: This enforces DRY, supports downstream validation without original schemas, and aligns with the project constitution.
- Alternatives considered: Partially nested JSON output. Rejected because it duplicates definitions and complicates identity management.

## Validation-Oriented Metadata

- Decision: Preserve occurrence constraints, base-type lineage, pattern-like rules, and structural relationships in the normalized graph.
- Rationale: The graph must support later XML validation and random XML generation without source XSD access.
- Alternatives considered: Minimal structural metadata only. Rejected because it would force later consumers to re-open original schema files.

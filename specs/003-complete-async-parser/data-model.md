# Data Model: Complete WSDL/XSD Parsing Workflows

## Entities

### ParseRequestModel
- Purpose: Abstract base model for one consumer-supplied parsing request.
- Fields:
  - `DocumentKind`: Caller-supplied `ESchemaDocumentKind` value identifying the input as WSDL or XSD.
  - `DisplayName`: Consumer-facing label used in diagnostics and documentation.
  - `LogicalPath`: Logical path or file path used for source normalization.
- Relationships:
  - Base type for all supported request-shape models.
  - Normalized by the orchestration service into one or more `SourceDescriptorModel` entries.

### FilePathParseRequestModel
- Purpose: Represents a file-backed parse request.
- Fields:
  - Inherits base request fields.
  - `FilePath`: Caller-supplied file system path.
- Validation Rules:
  - `FilePath` must be non-empty.
  - `DocumentKind` must be supplied.

### StreamParseRequestModel
- Purpose: Represents a stream-backed parse request.
- Fields:
  - Inherits base request fields.
  - `Content`: Caller-supplied readable and seekable stream.
- Validation Rules:
  - `Content` must be non-null, readable, and seekable.
  - `LogicalPath` must be non-empty.
  - `DocumentKind` must be supplied.

### MemoryParseRequestModel
- Purpose: Represents an in-memory binary parse request.
- Fields:
  - Inherits base request fields.
  - `Buffer`: Caller-supplied `ReadOnlyMemory<byte>` payload.
- Validation Rules:
  - `Buffer` must be non-empty.
  - `LogicalPath` must be non-empty.
  - `DocumentKind` must be supplied.

### BatchParseRequestModel
- Purpose: Represents one coordinated multi-source parse request.
- Fields:
  - `Sources`: Collection of `BatchSourceRequestModel` entries.
- Validation Rules:
  - `Sources` must contain at least one item.
  - Every source must declare `DocumentKind`.
  - Any invalid source fails the full request.

### BatchSourceRequestModel
- Purpose: Represents one source entry within a batch request.
- Fields:
  - Existing fields: `LogicalName`, `LogicalPath`, `Content`, `IsMain`.
  - Planned field: `DocumentKind` of type `ESchemaDocumentKind`.
- Relationships:
  - Belongs to one `BatchParseRequestModel`.
  - Produces one normalized `SourceDescriptorModel` during request loading.

### ESchemaDocumentKind
- Purpose: Distinguishes the caller-declared document kind for a source.
- Values:
  - `Wsdl`
  - `Xsd`

### Parsing Orchestration Service
- Purpose: Primary consumer-facing service that accepts request models and coordinates source loading, discovery, graph building, and exception translation.
- Responsibilities:
  - Validate request models.
  - Delegate to source loading and parser/discovery collaborators.
  - Return `MetadataGraphModel` on success.
  - Throw `ParseFailureException` on failure.

### SourceDescriptorModel
- Purpose: Normalized internal representation of one source after request loading.
- Existing Fields:
  - `SourceId`, `SourceKind`, `DisplayName`, `VirtualPath`, `RelativePath`, `IsMainSource`, `ContentFingerprint`, `LogicalName`.
- Planned Additions:
  - `DocumentKind` to preserve the caller-declared WSDL/XSD classification through discovery and graph building.

### ParsedItemHandler Contract
- Purpose: Shared polymorphic contract for item-category parsing.
- Implementations:
  - Complex type handler.
  - Simple type handler.
  - Element handler.
  - Attribute handler.
  - WSDL-derived service or schema artifact handler.
- Relationships:
  - Consumes normalized schema nodes from discovery/graph-building stages.
  - Writes canonical entries into registry services.

### MetadataGraphModel
- Purpose: Successful parse output returned to callers.
- Existing Fields:
  - `Sources`, `ComplexTypes`, `SimpleTypes`, `Elements`, `Attributes`, `Relationships`, `ValidationRules`, `RootRefIds`, `SerializerHints`.
- Relationships:
  - Populated only when the request completes successfully.

### ParseFailureException
- Purpose: Exception type raised when request validation, discovery, or graph construction cannot complete.
- Fields:
  - `Diagnostics`: One or more `ParseDiagnosticModel` entries.
  - `Stage`: High-level failure stage such as source loading, WSDL discovery, XSD graph building, or orchestration.
  - `SourceId`: Optional logical source identifier for the primary failing source.
- Relationships:
  - Carries structured failure data derived from parsing collaborators.

### ParseDiagnosticModel
- Purpose: Structured diagnostic payload attached to parse failures.
- Existing Fields:
  - `DiagnosticId`, `SourceId`, `VirtualPath`, `Stage`, `Code`, `Message`, `Details`.
- Relationships:
  - Aggregated into `ParseFailureException`.

## State Transitions

### Parse Request Lifecycle
1. `Received`: The orchestration service receives a caller-supplied request model.
2. `Validated`: Required input fields, source kind, and stream/buffer constraints pass request validation.
3. `Normalized`: `ISourceLoader` converts the request into one or more `SourceDescriptorModel` entries.
4. `Discovered`: WSDL discovery and XSD graph-building collaborators identify the reachable parsing items.
5. `Built`: Item-category handlers populate the centralized registry and finalize `MetadataGraphModel`.
6. `Failed`: A `ParseFailureException` is thrown and no metadata graph is returned.

### Parsed Item Lifecycle
1. `Encountered`: Discovery identifies a schema or WSDL-derived item.
2. `Dispatched`: The item is routed to the matching polymorphic handler.
3. `Canonicalized`: Registry services assign or reuse canonical identifiers.
4. `Linked`: Relationships, constraints, and parent-child references are attached.
5. `Exported`: The canonical entry appears in the final `MetadataGraphModel`.

## Validation Boundaries

- Request-model validation occurs before source normalization.
- Source-loading validation covers empty paths, invalid buffers, and non-seekable or unreadable streams.
- Document-kind validation occurs before WSDL discovery or XSD graph building begins.
- Any source failure terminates the full request with `ParseFailureException`.
# Public API Contract: Multi-Source XSD/WSDL Parser Library

## Objectives

- Expose async-first parse workflows for all supported source forms.
- Keep API shapes dependency-injection friendly.
- Return a normalized metadata graph that can be serialized to JSON without re-opening original schema files.
- Register concrete `XsdParserService`, `WsdlParserService`, and the default `VirtualFileSystemService` through DI-friendly abstractions.

## Public Abstractions

### IVirtualFileSystem
- Purpose: Abstract file and memory access used by schema discovery and `XmlSchemaSet` integration.
- Required operations:
  - `Task<VirtualFileModel> OpenFileAsync(string filePath, CancellationToken cancellationToken)`
  - `Task<VirtualFileModel> OpenMemoryAsync(string logicalPath, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)`
  - `Task<VirtualFileModel> OpenStreamAsync(string logicalPath, Stream stream, CancellationToken cancellationToken)`
  - `Task<bool> ExistsAsync(string virtualPath, CancellationToken cancellationToken)`
  - `Task<Stream> OpenReadAsync(string virtualPath, CancellationToken cancellationToken)`
  - `string ResolveRelativePath(string basePath, string relativePath)`

### SchemaRegistryService
- Purpose: Own canonical registration, de-duplication, lookup, and lifecycle state for discovered schema definitions.
- Required behavior:
  - Register discovered complex types, simple types, elements, and attributes during Pass 1.
  - Assign deterministic `RefId` values for anonymous complex types.
  - Reject conflicting duplicate definitions with actionable diagnostics.

### XsdGraphBuilder
- Purpose: Perform XSD discovery and build graph shells during Pass 1.
- Required behavior:
  - Discover reachable XSD definitions through `IVirtualFileSystem` and source identity metadata.
  - Populate `SchemaRegistryService` with canonical shells before any reference linkage occurs.
  - Capture schema-path metadata required for deterministic `RefId` generation.

### WsdlDiscoveryService
- Purpose: Discover WSDL-level services, embedded schemas, imported schemas, and schema entry points.
- Required behavior:
  - Identify schema documents reachable from WSDL definitions.
  - Hand off discovered XSD artifacts to `XsdGraphBuilder`.
  - Preserve service-level discovery metadata needed by the final graph.

### IWsdlParser
- Purpose: Parse WSDL inputs and emit a normalized metadata graph.
- Required operations:
  - `Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseFromStreamAsync(Stream stream, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseFromMemoryAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)`

### IXsdParser
- Purpose: Parse XSD inputs and emit a normalized metadata graph.
- Required operations:
  - `Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseFromStreamAsync(Stream stream, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseFromMemoryAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)`
  - `Task<MetadataGraphModel> ParseBatchAsync(IEnumerable<BatchSourceRequestModel> sources, CancellationToken cancellationToken)`

### IMetadataGraphSerializer
- Purpose: Serialize the normalized graph for consumers.
- Required operations:
  - `Task<string> SerializeAsync(MetadataGraphModel graph, CancellationToken cancellationToken)`
  - `Task SerializeAsync(MetadataGraphModel graph, Stream output, CancellationToken cancellationToken)`

### ISourceLoader
- Purpose: Normalize incoming source content into source descriptors.
- Required behavior:
  - Assign logical source identities.
  - Validate main-source designation.
  - Surface actionable diagnostics when a requested source cannot be resolved.

### Batch Source Contract

### BatchSourceRequestModel
- Required fields:
  - `LogicalName`: Consumer-supplied source name used for import/include matching.
  - `Content`: Stream content for the source.
  - `LogicalPath`: Consumer-supplied logical path used for relative resolution in non-file inputs.
  - `IsMain`: Optional flag indicating the designated main source.
- Rules:
  - At most one batch source may be marked as main.
  - The main source must be one of the supplied inputs.
  - Logical names must be unique within the request.
  - Requests with multiple candidate roots and no designated main source MUST fail with actionable diagnostics.

## Metadata Graph JSON Contract

### Top-Level Shape
- `sources`: Dictionary keyed by `SourceId`
- `complexTypes`: Dictionary keyed by `RefId`
- `simpleTypes`: Dictionary keyed by `RefId`
- `elements`: Dictionary keyed by `RefId`
- `attributes`: Dictionary keyed by `RefId`
- `relationships`: Dictionary keyed by relationship ID
- `validationRules`: Dictionary keyed by rule ID
- `rootRefIds`: Array of consumer entry points

### Normalization Rules
- Child definitions MUST be represented through `RefId` links.
- Anonymous complex types MUST have deterministic `RefId` values.
- `xs:choice` and `xs:sequence` MUST be flattened into child-member lists with ordering and grouping hints.
- Duplicate canonical definitions MUST appear only once across the graph dictionaries.

### Serialization Strategy
- `System.Text.Json` is the default serializer for the graph contract.
- Custom converters MAY be used for `RefId` wrappers, occurrence values such as unbounded markers, and constraint payloads that require stable contract formatting.
- Converter usage MUST preserve the dictionary-based graph structure and must not reintroduce nested duplicated definitions.

## Errors

The public API must produce actionable failures for:
- ambiguous root selection when multiple candidate roots are supplied without a main source
- invalid main-source selection
- unresolved imports/includes
- conflicting duplicate definitions
- unreadable, non-seekable, or empty sources
- invalid schema content

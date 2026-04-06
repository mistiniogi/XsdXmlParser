# Public Source Loading Contract

## Goal

Define the consumer-visible parser and source-loader contract after support is reduced to file-path and string inputs only.

## Supported Request Models

### FilePathParseRequestModel

- Required fields:
  - `FilePath`
  - `DocumentKind`
- Optional fields:
  - `DisplayName`
  - `LogicalPath`
- Behavior:
  - `LogicalPath` defaults to `FilePath` when omitted.
  - The file must exist before parsing begins.

### StringParseRequestModel

- Required fields:
  - `Content`
  - `LogicalPath`
  - `DocumentKind`
- Optional fields:
  - `DisplayName`
- Behavior:
  - `LogicalPath` is the source identity and base path for relative imports/includes.
  - `Content` must be non-empty XML text.

## Supported Public Entry Points

The primary source-loading contract is request-model-based and requires explicit caller-declared `DocumentKind`.

### IParserOrchestrationService

```csharp
Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);
Task<MetadataGraphModel> ParseStringAsync(StringParseRequestModel request, CancellationToken cancellationToken);
```

### ISourceLoader

```csharp
Task<SourceDescriptorModel> LoadAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);
Task<SourceDescriptorModel> LoadAsync(StringParseRequestModel request, CancellationToken cancellationToken);
```

## Secondary Typed Parser Adapters

`IXsdParser` and `IWsdlParser` remain narrowed to file and string workflows, but they are secondary typed adapters rather than the primary source-loading contract. Their abstraction implies document kind, so consumer-facing guidance should center on `IParserOrchestrationService` when explicit document-kind declaration is important.

### IXsdParser

```csharp
Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken);
Task<MetadataGraphModel> ParseFromStringAsync(string content, string logicalPath, CancellationToken cancellationToken);
```

Document-kind note:

- `IXsdParser` implies `ESchemaDocumentKind.Xsd` by the chosen abstraction and is documented as a typed adapter rather than the primary explicit-kind contract.

### IWsdlParser

```csharp
Task<MetadataGraphModel> ParseFromFileAsync(string filePath, CancellationToken cancellationToken);
Task<MetadataGraphModel> ParseFromStringAsync(string content, string logicalPath, CancellationToken cancellationToken);
```

Document-kind note:

- `IWsdlParser` implies `ESchemaDocumentKind.Wsdl` by the chosen abstraction and is documented as a typed adapter rather than the primary explicit-kind contract.

## Removed Public Entry Points

The following request types and workflows are removed from the supported public contract:

- `StreamParseRequestModel`
- `MemoryParseRequestModel`
- `BatchParseRequestModel`
- `BatchSourceRequestModel`
- `ParseStreamAsync(...)`
- `ParseMemoryAsync(...)`
- `ParseBatchAsync(...)`
- `LoadFromStreamAsync(...)`
- `LoadFromMemoryAsync(...)`
- `LoadBatchAsync(...)`
- `ParseFromStreamAsync(...)`
- `ParseFromMemoryAsync(...)`
- `ParseBatchAsync(...)`

## Failure Contract

- Invalid file requests fail before parsing begins.
- Invalid string requests fail before parsing begins.
- Missing `DocumentKind` declarations fail before parsing begins.
- Missing `LogicalPath` on string requests fails before parsing begins.
- Unresolved related files referenced from string-backed inputs fail with source-loading or resolution diagnostics.
- Failures surface through `ParseFailureException` with `ParseDiagnosticModel` entries.

## Validation Expectations

- Representative valid file requests must be exercised.
- Representative valid string requests must be exercised.
- Representative invalid file and invalid string requests must be exercised.
- One equivalent-content comparison between file-backed and string-backed retained workflows must be exercised.

## Documentation And Comment Expectations

- XML documentation on all changed public contracts must describe file/string-only support.
- Consumer docs must show file and string examples only.
- Why comments should explain any intentionally preserved internal behavior that still uses memory-backed virtual-file registration under the string-based public contract.
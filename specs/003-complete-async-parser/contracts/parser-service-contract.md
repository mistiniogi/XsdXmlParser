# Contract: Primary Parser Service

## Purpose

Define the consumer-facing parsing contract for the main orchestration service introduced by this feature.

## Public Service Contract

```csharp
public interface IParserOrchestrationService
{
    Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);
    Task<MetadataGraphModel> ParseStreamAsync(StreamParseRequestModel request, CancellationToken cancellationToken);
    Task<MetadataGraphModel> ParseMemoryAsync(MemoryParseRequestModel request, CancellationToken cancellationToken);
    Task<MetadataGraphModel> ParseBatchAsync(BatchParseRequestModel request, CancellationToken cancellationToken);
}
```

## Request Contracts

### FilePathParseRequestModel

- Requires `FilePath`.
- Requires `DocumentKind` with value `Wsdl` or `Xsd`.
- Uses the supplied path as the logical path unless an explicit logical path override is introduced during implementation.

### StreamParseRequestModel

- Requires `DisplayName`.
- Requires `LogicalPath`.
- Requires `Content`.
- Requires `DocumentKind` with value `Wsdl` or `Xsd`.
- Rejects null, unreadable, or non-seekable streams.

### MemoryParseRequestModel

- Requires `DisplayName`.
- Requires `LogicalPath`.
- Requires `Buffer`.
- Requires `DocumentKind` with value `Wsdl` or `Xsd`.
- Rejects empty buffers.

### BatchParseRequestModel

- Requires one or more `BatchSourceRequestModel` entries.
- Each source requires `LogicalName`, `LogicalPath`, `Content`, and `DocumentKind`.
- Batch requests fail as a whole when any source is invalid.

## Success Contract

- All methods return `MetadataGraphModel` when parsing completes successfully.
- Successful results expose the existing normalized graph dictionaries and root reference identifiers.

## Failure Contract

- All parse failures throw `ParseFailureException`.
- Exceptions carry one or more `ParseDiagnosticModel` entries with stage, source identity, code, and message details when available.
- The service does not return partial metadata when any source in the active request is invalid.

## Composition Contract

- `IParserOrchestrationService` is the primary consumer-facing entry point registered through `ServiceCollectionExtensions`.
- Existing `IWsdlParser` and `IXsdParser` services remain available as lower-level collaborators or compatibility seams during implementation.
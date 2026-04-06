# Data Model: Simplify Source Loading Inputs

## Overview

This feature narrows the consumer-visible source-loading model from four input forms to two. The downstream metadata graph remains unchanged; the data-model impact is limited to request models, source-origin reporting, and source-loading validation.

## Entities

### ParseRequestModel

- Purpose: Shared base model for retained parser requests.
- Existing Fields:
  - `DocumentKind`: Caller-declared `ESchemaDocumentKind` and required for retained workflows.
  - `DisplayName`: Optional diagnostic display name.
  - `LogicalPath`: Optional in the base class, but required by `StringParseRequestModel` and normalized for file-based requests.
- Relationships:
  - Parent type for `FilePathParseRequestModel` and `StringParseRequestModel` after this feature.

### FilePathParseRequestModel

- Purpose: Represents a file-backed WSDL or XSD request.
- Fields:
  - `FilePath`: Required physical path to the source file.
  - Inherited `DocumentKind`: Required, explicitly declared by the caller.
  - Inherited `DisplayName`: Optional display label; defaults to file name when omitted.
  - Inherited `LogicalPath`: Optional caller override; defaults to `FilePath` when omitted.
- Validation Rules:
  - `FilePath` must be non-empty.
  - The resolved file must exist before parsing begins.
  - `DocumentKind` must be explicitly provided.

### StringParseRequestModel

- Purpose: Represents a string-backed WSDL or XSD request.
- Status: New model introduced by this feature.
- Proposed Fields:
  - `Content`: Required non-empty XML text.
  - Inherited `DocumentKind`: Required, explicitly declared by the caller.
  - Inherited `DisplayName`: Optional display label for diagnostics.
  - Inherited `LogicalPath`: Required base path for source identity and relative resolution.
- Validation Rules:
  - `Content` must not be null, empty, or whitespace-only.
  - `LogicalPath` must be provided.
  - `DocumentKind` must be explicitly provided.
- Relationships:
  - Normalized by `ISourceLoader` into a `SourceDescriptorModel`.
  - Uses `LogicalPath` as the base for relative imports and includes.

### SourceDescriptorModel

- Purpose: Represents a normalized source handed to the existing parsing pipeline.
- Affected Fields:
  - `DocumentKind`: Unchanged.
  - `SourceKind`: Must reflect only supported source origins after this feature.
  - `DisplayName`, `VirtualPath`, `RelativePath`, `IsMainSource`, `ContentFingerprint`, `LogicalName`, `SourceId`: Unchanged in role.
- Validation Rules:
  - File-backed descriptors map to a retained file source kind.
  - String-backed descriptors map to a retained string source kind.
- Relationships:
  - Produced by `ISourceLoader` and consumed by parser orchestration, discovery, and graph-building services.

### ESourceKind

- Purpose: Identifies the normalized source origin reported through `SourceDescriptorModel`.
- Current State: `FilePath`, `Stream`, `Memory`, `BatchStream`.
- Planned State:
  - Retain `FilePath`.
  - Add `String` for string-backed requests.
  - Remove `Stream`, `Memory`, and `BatchStream` from the supported public contract.
- Validation Rules:
  - Every emitted descriptor must use a supported retained source kind.

### ParseFailureException And ParseDiagnosticModel

- Purpose: Represent source-loading failures before parsing begins.
- Affected Behavior:
  - Failures now cover invalid file requests, invalid string requests, missing logical paths, unresolved related files for string-backed inputs, and missing or incorrect document kinds.
- Relationships:
  - Produced by `SourceLoaderService` and surfaced through parser entry points.

## State Transitions

### File Request Flow

1. Caller creates `FilePathParseRequestModel` with `FilePath` and explicit `DocumentKind`.
2. `ISourceLoader` validates the request and resolves the file through `IVirtualFileSystem`.
3. Loader produces a `SourceDescriptorModel` with a file-backed source kind.
4. Existing parsing services process the descriptor without broader graph-model changes.

### String Request Flow

1. Caller creates `StringParseRequestModel` with `Content`, `LogicalPath`, and explicit `DocumentKind`.
2. `ISourceLoader` validates the string content and logical path.
3. Loader normalizes the content through the virtual file system and produces a `SourceDescriptorModel` with a string-backed source kind.
4. Existing parsing services resolve any related files from the string request’s logical path and continue through the current parsing pipeline.

## Removals

- `StreamParseRequestModel`: Removed from the public contract.
- `MemoryParseRequestModel`: Removed from the public contract.
- `BatchParseRequestModel`: Removed from the public contract.
- `BatchSourceRequestModel`: Removed from the public contract.

## Comment And Documentation Impact

- XML documentation on retained and new request models must describe file/string-only support.
- Why comments are expected where string normalization deliberately reuses memory-backed virtual-file-system plumbing or where source-kind reporting changes from the pre-feature behavior.
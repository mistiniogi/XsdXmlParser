# Implementation Plan: Multi-Source XSD/WSDL Parser Library

**Branch**: `002-multi-source-parser` | **Date**: 2026-04-05 | **Spec**: `/specs/002-multi-source-parser/spec.md`
**Input**: Feature specification from `/specs/002-multi-source-parser/spec.md`

## Summary

Build a production-grade async-first NuGet library that parses XSD and WSDL inputs from file paths, streams, read-only memory buffers, and multi-source stream batches. The implementation will normalize discovered schema metadata into a centralized ID-based registry, resolve import/include chains through a virtual file system and logical source identities, and emit a dictionary-based JSON metadata graph with deterministic `RefId` references, cycle-safe linkage, and enough preserved constraints for downstream XML validation and later rule-based XML generation.

## Technical Context

**Language/Version**: C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0  
**Primary Dependencies**: `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Logging.Abstractions`  
**Storage**: In-memory centralized registry and normalized metadata graph dictionaries  
**Testing**: `dotnet test` with unit, integration, contract, compatibility, cancellation, and performance coverage under `tests/`  
**Target Platform**: Cross-platform .NET library consumers on macOS, Linux, and Windows  
**Project Type**: Multi-targeted NuGet library  
**Performance Goals**: Representative multi-file schema sets parse without duplicate canonical entries, infinite recursion, or nondeterministic graph identity; repeated parses of equivalent input yield equivalent logical graphs and stable anonymous-type `RefId` values  
**Constraints**: Async-first APIs, cancellation propagation on parsing and serialization workflows, no sync-over-async, C# 10.0 only in shared production code, no duplicate schema-definition storage, deterministic logical source identity and `RefId` generation  
**Scale/Scope**: Single-source and multi-source XSD/WSDL parsing, centralized registry population, two-pass graph construction, JSON graph serialization, and actionable diagnostics for invalid sources and schema resolution failures

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Async-first I/O boundaries are satisfied through `IXsdParser`, `IWsdlParser`, `ISourceLoader`, `IVirtualFileSystem`, and `IMetadataGraphSerializer`, all of which expose asynchronous operations and require `CancellationToken` propagation.
- Registry design uses `SchemaRegistryService`, `TypeRegistry`, `SourceGraphRegistry`, and `RefIdFactory` as the centralized ID-based source of truth for complex types, simple types, elements, attributes, and normalized relationships.
- Naming contracts are satisfied by `I*` interfaces, `*Model` models, and `E*` enums; no naming exceptions are required by the current design.
- Target framework support is `net6.0`, `net7.0`, and `net8.0`, and shared production code remains limited to C# 10.0 features.
- DI ownership is explicit through `ServiceCollectionExtensions`, with parser, registry, source-loading, virtual file system, and serializer services registered behind abstractions suitable for testing and host integration.
- Full XML documentation is required across all shared production declarations, and inline `Why` comments are required in traversal, cycle-resolution, and canonicalization hotspots such as `ImportResolutionService`, `GraphLinkingService`, `SchemaRegistryService`, `SourceGraphRegistry`, and `RefIdFactory`.

## Architecture Overview

### Request Normalization

- `SourceLoaderService` converts file, stream, memory, and batch inputs into `SourceDescriptorModel` instances.
- `SourceIdentityProviderService` validates or assigns logical source identities and logical paths needed for cycle detection, diagnostics, and relative import/include resolution.
- Batch validation fails fast for missing or non-unique logical identities, ambiguous roots, or invalid main-source designation.

### Discovery and Linking

- `WsdlDiscoveryService` discovers schema entry points from WSDL inputs and hands resolved schemas to `XsdGraphBuilder`.
- `XsdGraphBuilder` performs Pass 1 discovery, registers canonical shells, and captures schema-path metadata required for deterministic anonymous-type `RefId` generation.
- `GraphLinkingService` performs Pass 2 linkage, assigning type references, inheritance, occurrence rules, flattened compositor hints, and source-to-source import/include relationships after all canonical definitions exist.

### Canonical Registry and Serialization

- `SchemaRegistryService` owns canonical registration, duplicate-equivalence checks, and conflict detection.
- `TypeRegistry` stores exported dictionaries for complex types, simple types, elements, attributes, validation rules, and normalized relationships.
- `MetadataGraphJsonSerializer` emits the final dictionary-based graph using `System.Text.Json` and targeted converters for `RefId`, occurrence values, and constraint payload formatting.

## Project Structure

### Documentation (this feature)

```text
specs/002-multi-source-parser/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── Abstractions/
├── Models/
├── Registry/
├── Parsing/
├── Serialization/
└── Extensions/

tests/
├── Unit/
│   ├── Registry/
│   ├── Parsing/
│   ├── Serialization/
│   └── RefIds/
├── Integration/
│   ├── SingleSource/
│   ├── MultiSource/
│   └── Cycles/
└── Contract/
    └── MetadataGraph/
```

**Structure Decision**: Use a single multi-target library with feature-oriented folders for abstractions, models, registry orchestration, parsing services, serialization, and DI registration. Mirror that structure in tests with unit, integration, and contract suites aligned to the highest-risk behaviors in the specification.

## Implementation Phases

### Phase 0: Foundation

- Update the package and repository structure for multi-target library delivery.
- Establish abstractions, data models, registry shells, parser shells, serializer shells, and DI registration points.
- Preserve XML documentation coverage and plan `Why` comment hotspots before implementation expands.

### Phase 1: Single-Source Parsing

- Implement file-path, stream, and memory-buffer loading.
- Reject unreadable, empty, invalid, or non-seekable sources with actionable diagnostics.
- Prove deterministic parity across equivalent single-source inputs.

### Phase 2: Multi-Source Resolution

- Implement batch normalization, logical-path validation, virtual file resolution, and main-source enforcement.
- Traverse import/include chains with cycle-safe source tracking.
- Fail fast on ambiguous roots, invalid main sources, missing logical identities, unresolved imports, and conflicting duplicate definitions.

### Phase 3: Graph Export and Downstream Metadata

- Populate canonical graph dictionaries from the registry.
- Link type relationships, inheritance, validation rules, occurrence bounds, and flattened compositor metadata.
- Serialize the graph with stable contract formatting and validate the contract through integration and contract tests.

### Phase 4: Hardening

- Validate compatibility across `net6.0`, `net7.0`, and `net8.0`.
- Validate cancellation propagation and representative performance behavior.
- Finish documentation updates for quickstart, package guidance, and public contract alignment.

## Validation Strategy

- Unit tests validate deterministic `RefId` generation, source normalization, registry canonicalization, duplicate detection, and serializer converter behavior.
- Integration tests validate single-source parity, multi-source resolution, cycle-safe traversal, ambiguous-root failures, and invalid-main-source failures.
- Contract tests validate dictionary-based graph shape, `RefId`-only child relationships, preserved validation metadata, and generation-oriented compositor hints.
- Compatibility validation confirms the feature builds and test assets remain green across `net6.0`, `net7.0`, and `net8.0`.

## Risks and Mitigations

- Risk: Cyclic import/include traversal can produce recursion bugs or partial graphs.
  Mitigation: Track traversal state by logical source identity in `SourceGraphRegistry` and defer linkage to Pass 2.
- Risk: Equivalent and conflicting duplicates may be hard to distinguish reliably.
  Mitigation: Canonicalize entries through `SchemaRegistryService` with explicit duplicate-equivalence comparison and conflict diagnostics.
- Risk: Non-file inputs can lose stable identity or relative path context.
  Mitigation: Require caller-supplied logical identity and logical path metadata for batch inputs and normalize all source types through `SourceDescriptorModel`.
- Risk: Flattening compositor structures can lose downstream meaning.
  Mitigation: Preserve choice-group identifiers, compositor kind, and ordering hints at the member level and cover them in contract tests.

## Complexity Tracking

No constitution violations or exceptional complexity justifications are required for the current design. The implementation remains within the expected bounds of a single multi-target library with explicit registry, parsing, and serialization seams.

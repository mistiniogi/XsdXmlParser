# Implementation Plan: Complete WSDL/XSD Parsing Workflows

**Branch**: `003-complete-async-parser` | **Date**: 2026-04-06 | **Spec**: `/specs/003-complete-async-parser/spec.md`
**Input**: Feature specification from `/specs/003-complete-async-parser/spec.md`

## Summary

Complete the library's missing parsing pipeline by introducing one consumer-facing orchestration service that accepts file path, stream, read-only memory, and batch source requests with explicit WSDL/XSD source kind declarations. The implementation will preserve the existing DI-friendly architecture by keeping source loading, WSDL discovery, XSD graph construction, registry canonicalization, and item-category parsing responsibilities separated behind asynchronous, cancellation-aware collaborators. Parsing failures will be surfaced through exceptions, successful operations will return the existing metadata graph contract, and repository documentation will be updated to show the new primary entry points without adding feature-scoped test logic.

## Technical Context

**Language/Version**: C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0  
**Primary Dependencies**: `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`  
**Storage**: In-memory source descriptors, centralized registry services, and normalized metadata graph dictionaries  
**Testing**: Existing `dotnet test` structure under `tests/` remains available for future validation, but this feature explicitly excludes creating new test logic and instead preserves DI seams, exception contracts, and async boundaries for later verification  
**Target Platform**: Cross-platform .NET library consumers on macOS, Linux, and Windows  
**Project Type**: Multi-targeted NuGet library  
**Performance Goals**: Parse representative WSDL/XSD inputs asynchronously without sync-over-async, avoid partial graph emission on failure, and keep repeated orchestration overhead bounded to one source-normalization plus one discovery/build pipeline per request  
**Constraints**: Async-first public APIs, `CancellationToken` propagation across all parsing stages, exception-based failure contract, explicit caller-supplied source kind, no partial metadata on invalid requests, C# 10.0 only in shared production code, complete XML documentation, and no feature-scoped test implementation  
**Scale/Scope**: One new primary orchestration service, request-model expansion for four input forms, item-specific parsing collaborators for WSDL/XSD graph population, DI updates, and consumer documentation updates across library guidance

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Async-first I/O boundaries are defined through the new orchestration service, `ISourceLoader`, existing parser/discovery services, and serializer seams; all affected methods will remain asynchronous with explicit cancellation propagation and no sync-over-async adapters.
- Registry design remains centralized through `SchemaRegistryService`, `TypeRegistry`, `SourceGraphRegistry`, and `RefIdFactory`; the feature adds orchestration and parsing logic around that registry rather than duplicating schema state.
- Naming contracts are satisfied by `I*` interfaces, `*Model` request/graph types, and `E*` enums; one new source-document-kind enum and new request models will follow the same pattern with no exceptions planned.
- Target framework support remains `net6.0`, `net7.0`, and `net8.0`, and all shared production code will continue to use C# 10.0-compatible language features only.
- DI registration and lifetime ownership are explicit in `ServiceCollectionExtensions`; the new orchestration service will become the primary consumer-facing registration while existing WSDL/XSD parser services remain internal collaborators or compatibility seams to preserve design stability.
- Full XML documentation is required for every new or changed shared production declaration, and `Why` comments are required where parsing-stage selection, item-category dispatch, WSDL-to-XSD handoff, exception translation, or registry canonicalization would otherwise be non-obvious.

## Architecture Overview

### Public API Consolidation

- Add one consumer-facing orchestration abstraction and implementation as the primary parse entry point.
- Keep existing `IWsdlParser` and `IXsdParser` responsibilities available as internal collaborators or compatibility adapters rather than removing established parser boundaries.
- Use request models to separate input-form concerns from parsing-stage concerns while keeping one successful return type: `MetadataGraphModel`.

### Parsing Pipeline Separation

- `SourceLoaderService` continues to normalize file, stream, memory, and batch inputs into `SourceDescriptorModel` instances.
- A WSDL discovery layer resolves WSDL-level artifacts and hands schema entry points to the XSD graph construction layer.
- XSD graph construction delegates item-category parsing to dedicated handlers behind one shared contract so elements, attributes, simple types, complex types, and WSDL service-derived items are processed independently but contribute to one registry-backed result.

### Failure and Documentation Strategy

- Invalid requests and parse-stage failures throw parsing exceptions that carry structured source diagnostics.
- Successful parse operations return `MetadataGraphModel` only when the full request completes.
- Repository guidance is updated to document the orchestration service, request models, explicit source-kind requirement, and exception behavior.

## Project Structure

### Documentation (this feature)

```text
specs/003-complete-async-parser/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── parser-service-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── Abstractions/
├── Extensions/
├── Models/
├── Parsing/
├── Registry/
└── Serialization/

tests/
├── Contract/
│   └── MetadataGraph/
├── Integration/
│   ├── Cycles/
│   ├── MultiSource/
│   └── SingleSource/
└── Unit/
    ├── Parsing/
    ├── RefIds/
    ├── Registry/
    └── Serialization/
```

**Structure Decision**: Continue using the single multi-target library layout already established in `src/`, with orchestration, source loading, discovery, graph building, registry services, and serialization staying in their current feature-aligned folders. Existing `tests/` directories remain part of the repository structure for future validation work, but this feature plan does not include creating new test logic.

## Implementation Phases

### Phase 0: Research and Contract Decisions

- Confirm how the primary orchestration service coexists with `IWsdlParser` and `IXsdParser` without breaking established design constraints.
- Define request-model boundaries for file path, stream, read-only memory, and batch-source workflows with explicit source-kind declarations.
- Define the exception contract for request-validation failures, discovery failures, and graph-construction failures.

### Phase 1: Design and Documentation

- Introduce the public API contract for the orchestration service and its request models.
- Define item-category parser contracts and registry interaction points needed to fill the missing graph-building logic.
- Update quickstart and consumer documentation to center on the orchestration service while documenting explicit source kind and exception behavior.

### Phase 2: Implementation Planning for Main Logic

- Establish foundational registry canonicalization seams before user-story graph population work begins.
- Establish multi-target compatibility validation checkpoints for `net6.0`, `net7.0`, and `net8.0` before feature work is considered implementation-ready.
- Add orchestration-service registration and compatibility mapping in DI.
- Expand source normalization and request validation to honor explicit source kind across all supported input forms.
- Implement WSDL discovery handoff, XSD item-category dispatch, and metadata graph population with separated responsibilities and async cancellation propagation.

### Phase 3: Hardening and Compatibility Review

- Recheck multi-target compatibility, XML documentation coverage, and `Why` comment hotspots after implementation.
- Reconcile README and docs updates with the final public contract shape.
- Validate async/cancellation behavior, exception-based failure semantics, and documentation walkthrough completion against the success criteria without adding new test logic.
- Preserve repository testing seams without expanding the feature scope into new test logic.

## Risks and Mitigations

- Risk: Introducing one public orchestration service could conflict with existing `IWsdlParser` and `IXsdParser` abstractions.
  Mitigation: Add the orchestration service as the new primary public entry point while treating existing parser interfaces as compatibility or internal-collaboration seams.
- Risk: Exception-only failure handling can obscure source-stage context if failures are thrown too early or inconsistently.
  Mitigation: Standardize a parsing exception type that carries `ParseDiagnosticModel` details, source identifiers, and stage information.
- Risk: Item-category polymorphism can devolve into another monolithic switch if handler boundaries are unclear.
  Mitigation: Define one shared item-parser contract and keep category-specific logic in dedicated handlers for complex types, simple types, elements, attributes, and WSDL-derived definitions.
- Risk: Explicit source-kind requirements may drift across input forms.
  Mitigation: Put source-kind validation in shared request normalization and document it consistently in the public contract and quickstart.

## Post-Design Constitution Re-Check

- Async orchestration, source loading, discovery, graph building, and serialization remain cancellation-aware and async-first.
- Centralized registry ownership is preserved; no design artifact introduces duplicate schema-definition storage.
- New planned types follow the `I*`, `*Model`, and `E*` naming rules with no exceptions.
- The design remains within `net6.0`, `net7.0`, `net8.0`, and C# 10.0 compatibility boundaries.
- DI ownership is explicit through one primary orchestration service plus internal collaborators; testing seams remain documented even though new test logic is out of scope.
- XML documentation and `Why` comment hotspots are explicitly identified for orchestration, handler dispatch, registry canonicalization, and exception translation.

## Complexity Tracking

No constitution violations or exceptional complexity justifications are required for the current design.

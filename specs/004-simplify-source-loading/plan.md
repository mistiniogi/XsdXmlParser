# Implementation Plan: Simplify Source Loading Inputs

**Branch**: `004-simplify-source-loading` | **Date**: 2026-04-06 | **Spec**: `/specs/004-simplify-source-loading/spec.md`
**Input**: Feature specification from `/specs/004-simplify-source-loading/spec.md`

## Summary

Reduce the library's supported source-loading surface to file-path and string-content workflows only, with explicit caller-declared document kind enforced by the primary request-model-based contract. The implementation will remove stream, memory, and batch request paths from the public parser and loader contracts, add a dedicated string request model, preserve the existing async parsing pipeline and registry-backed graph generation, explicitly re-check DI registration and centralized registry flow after the contract reduction, and update XML documentation comments plus repository guidance so the code comments and consumer docs reflect the modified spec without introducing unrelated parser logic changes.

## Technical Context

**Language/Version**: C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0  
**Primary Dependencies**: `System.Xml`, `System.Text`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`  
**Storage**: In-memory source descriptors, virtual file metadata, centralized registry services, and normalized metadata graph dictionaries  
**Testing**: Multi-target `dotnet build` validation plus representative quickstart scenario execution for valid and invalid file/string workflows and API/documentation contract review; existing `tests/` scaffolding remains available but this feature does not add new test logic  
**Target Platform**: Cross-platform .NET library consumers on macOS, Linux, and Windows  
**Project Type**: Multi-targeted NuGet library  
**Performance Goals**: Preserve current async parsing readiness with no sync-over-async regressions, keep one normalization step per request, and avoid adding extra file reads beyond those required for retained file and string resolution paths  
**Constraints**: No unrelated parsing behavior changes, explicit caller-declared `ESchemaDocumentKind` for file and string requests, required logical path for string inputs, compile compatibility with `net6.0`/`net7.0`/`net8.0`, C# 10.0 only, full XML documentation on changed shared declarations, and targeted Why comments where source-kind or resolution behavior is non-obvious  
**Scale/Scope**: Contract reduction across parser and loader abstractions, one new string request model, request/model cleanup for removed source forms, DI/documentation updates, and source-loading comment refresh limited to the affected workflow

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Async-first I/O boundaries remain intact: retained file and string loading continue to use asynchronous methods with `CancellationToken` propagation, and no sync-over-async wrappers are introduced.
- Registry design stays centralized through existing source identity, registry, and graph-building services; the feature only narrows how source descriptors are produced.
- Naming contracts are satisfied by preserving existing `I*`, `*Model`, and `E*` patterns and introducing `StringParseRequestModel` only if needed by the implementation.
- Target framework and language-version constraints remain `net6.0`, `net7.0`, `net8.0`, and C# 10.0 for all shared production changes.
- DI registration and ownership remain explicit in `ServiceCollectionExtensions`, with the same service graph and a narrowed public contract rather than a new architecture; the plan includes an explicit DI verification pass after the contract reduction.
- XML documentation and comment updates are required on every changed shared production declaration, and Why comments are expected around string normalization, logical-path-based resolution, and any source-kind translation that would otherwise be unclear.

## Architecture Overview

### Public Contract Reduction

- Narrow the primary request-model-based source-loading contract in `ISourceLoader` and `IParserOrchestrationService` to the two supported input forms only: file path and string content with explicit `DocumentKind`.
- Keep `IXsdParser` and `IWsdlParser` as typed parser adapters whose abstraction implies document kind, while ensuring consumer-facing guidance centers on the explicit request-model contract.
- Remove stream, memory, and batch request models plus their public entry points instead of keeping compatibility shims.
- Keep explicit document-kind declaration on retained request models and document the reduced surface in code comments and repository guidance.

### String Normalization Strategy

- Add a dedicated `StringParseRequestModel` so textual schema input is represented explicitly instead of reusing the removed memory-buffer contract.
- Reuse the existing virtual-file-system memory-backed normalization path internally by converting string content to bytes before registration, which keeps downstream resolution behavior stable while the public contract changes from memory to string.
- Require logical paths for all string inputs so relative imports and includes continue to resolve through the existing filesystem-backed resolution logic.

### Documentation And Comment Synchronization

- Update XML documentation comments on all changed interfaces, models, enums, and services so they describe file/string support only.
- Update repository-facing docs in `README.md` and `docs/getting-started.md` to remove stream, memory, and batch examples and replace them with file and string examples.
- Add implementation-time validation of representative file/string success and failure scenarios so success criteria are checked against real retained workflows instead of build-only evidence.
- Keep parser internals, registry ownership, and discovery/build flows otherwise unchanged so the implementation remains a surface reduction rather than a parser redesign.

## Project Structure

### Documentation (this feature)

```text
specs/004-simplify-source-loading/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-source-loading-contract.md
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

docs/
├── getting-started.md
├── introduction.md
└── toc.yml

tests/
├── Contract/
├── Integration/
└── Unit/
```

**Structure Decision**: Continue using the existing single-library structure. Feature work is concentrated in `src/Abstractions`, `src/Models`, and `src/Parsing`, with supporting DI and consumer documentation updates in `src/Extensions`, `README.md`, and `docs/getting-started.md`. The repository `tests/` tree remains part of the project context for future validation, but this plan does not add new test logic.

## Implementation Phases

### Phase 0: Research And Scope Lock

- Confirm which public contracts currently expose stream, memory, and batch source forms and treat their removal as an intentional breaking change aligned to the spec.
- Decide how string input is represented without widening internal parser changes; the preferred direction is a dedicated string request model layered over existing virtual file registration.
- Confirm that explicit document-kind declaration is enforced by the primary request-model contract while typed parser adapters remain secondary convenience seams.
- Identify all XML documentation comments and consumer docs that currently mention removed source forms.

### Phase 1: Design And Contract Updates

- Define the retained request contract around `FilePathParseRequestModel` and a new `StringParseRequestModel`, both with explicit `DocumentKind` and required logical path semantics where applicable.
- Define how `ESourceKind` and `SourceDescriptorModel` reflect the reduced public source-loading surface without leaking removed source types through consumer-visible metadata.
- Define the DI registration and centralized registry verification points needed to prove the narrowed contract still routes through the existing service graph and canonical registry flow.
- Document the revised parser/loader contracts and the quickstart flow for file and string parsing, including failure expectations.

### Phase 2: Implementation Planning

- Remove stream, memory, and batch methods from `ISourceLoader`, `IParserOrchestrationService`, `IXsdParser`, and `IWsdlParser`, and update implementations accordingly.
- Add string-loading support in `SourceLoaderService` and orchestration/parser services while reusing existing virtual-file-system registration and source identity logic.
- Re-check `ServiceCollectionExtensions` and the registry-backed source-descriptor flow after contract changes so DI ownership and canonical registry usage stay explicit and compliant.
- Update XML docs/comments, README examples, and getting-started guidance to match the new contract and ensure build validation across `net6.0`, `net7.0`, and `net8.0` remains part of the execution plan.
- Validate representative retained workflows after implementation using the quickstart scenarios: valid file input, valid string input, invalid file/string input, and file-vs-string readiness comparison.

## Risks And Mitigations

- Risk: Removing public stream, memory, and batch methods is a compile-time breaking change for existing consumers.
  Mitigation: Make the removal explicit in contracts and documentation, and keep the feature scope otherwise narrow so the breaking surface is predictable.
- Risk: Replacing memory input with string input could accidentally alter import/include resolution semantics.
  Mitigation: Require logical paths on string requests and normalize string content through the existing virtual file system pipeline used for non-file content.
- Risk: `SourceDescriptorModel` and `ESourceKind` could leak removed source-form terminology after the public contract is narrowed.
  Mitigation: Update the model contract and XML docs alongside parser/loader changes so consumer-visible metadata reflects the new file/string-only support.
- Risk: Documentation drift between code comments, README examples, and feature spec could leave unsupported workflows discoverable.
  Mitigation: Treat XML docs and markdown documentation as first-class implementation deliverables in the plan, not cleanup work.

## Post-Design Constitution Re-Check

- Async boundaries and cancellation propagation remain explicit for retained file and string workflows.
- No duplicate registry or graph state is introduced; the feature only changes source normalization inputs and contract exposure.
- Planned types and enums continue to follow naming rules, with no C# 10.0 violations expected.
- DI ownership remains explicit through existing service registrations and parser/loader seams.
- XML documentation and Why comment hotspots are explicitly identified for loader normalization, logical-path-based resolution, source-kind reporting, and consumer-facing contract updates.
- DI verification, registry-flow verification, and representative runtime validation are explicitly part of the post-design execution plan.

## Complexity Tracking

No constitution violations or exceptional complexity justifications are required for the current design.

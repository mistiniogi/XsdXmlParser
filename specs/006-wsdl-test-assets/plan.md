# Implementation Plan: WSDL Test Assets

**Branch**: `006-wsdl-test-assets` | **Date**: 2026-04-06 | **Spec**: `/specs/006-wsdl-test-assets/spec.md`
**Input**: Feature specification from `/specs/006-wsdl-test-assets/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add a reusable WSDL fixture catalog under `tests/Integration/wsdl-fixtures` for future testing without changing source code or runtime behavior. The implementation will create six category folders, use lowercase ASCII kebab-case names that are safe across macOS, Windows, and Linux, organize each fixture as a self-contained fixture-set folder, and include representative WSDL and companion XSD files where required so future integration and negative-path tests can reference stable file-system assets.

## Technical Context

**Language/Version**: C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` for the host repository; WSDL 1.1 and XSD fixture files only for this feature  
**Primary Dependencies**: Existing library dependencies remain unchanged: `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, analyzer packages configured in the project file  
**Storage**: Filesystem-backed test fixture assets under `tests/Integration/wsdl-fixtures`  
**Testing**: Existing test projects remain unchanged; feature validation uses fixture tree inspection, asset readability checks, and `dotnet build` to confirm no collateral breakage  
**Target Platform**: Cross-platform .NET repository workflows on macOS, Windows, and Linux
**Project Type**: Multi-targeted library repository with integration test assets  
**Performance Goals**: Preserve existing runtime and build behavior because the implementation adds only static fixture files; keep fixture sets small enough for fast repository navigation and future test execution  
**Constraints**: No source-code changes, no public API changes, no project-file changes, no documentation changes outside feature artifacts, ASCII path-safe names, six required categories, structurally wrong WSDL only for invalid fixtures, companion XSDs required for import-based fixtures  
**Scale/Scope**: One dedicated fixture root, six category folders, at least one fixture set per category, and companion XSD files only where the import-based category requires them

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Async-first I/O boundaries, cancellation propagation, and absence of sync-over-async are defined.
- Registry design uses a centralized, ID-based source of truth for shared type definitions.
- Naming contracts for enums, models, interfaces, and implementations are explicitly satisfied or justified.
- Target framework support and C# 10.0-only language-version constraints comply with the active constitution.
- DI registration, lifetime ownership, and integration verification boundaries are documented.
- Full C# XML documentation expectations for types and members, and rationale-comment hotspots for complex logic, are documented.

Result: PASS. This feature adds only test assets under `tests/Integration/wsdl-fixtures` and does not modify async APIs, registry behavior, naming contracts for production code, target framework support, DI registration, or C# XML documentation surfaces. The plan keeps implementation scope explicitly asset-only so it remains compatible with the constitution and the spec's no-code-change boundary.

## Project Structure

### Documentation (this feature)

```text
specs/006-wsdl-test-assets/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
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
└── Integration/
│   ├── Cycles/
│   ├── MultiSource/
│   ├── SingleSource/
│   └── wsdl-fixtures/
│       ├── simplest-wsdls/
│       ├── simple-wsdls/
│       ├── complex-wsdls/
│       ├── very-complex-wsdls/
│       ├── very-complex-wsdls-with-xsd-imports/
│       └── invalid-wsdls/
```

**Structure Decision**: Use the existing single-library repository layout rooted at `src/` and `tests/Integration`, and place the new assets inside `tests/Integration/wsdl-fixtures` because the repository already groups integration scenarios by dedicated directories. Each category folder will contain one or more self-contained fixture-set subfolders so future tests can reference stable paths without mixing files from unrelated scenarios.

## Complexity Tracking

No constitution violations are required for this feature.


# Implementation Plan: WSDL Fixture Integration Coverage

**Branch**: `007-wsdl-integration-tests` | **Date**: 2026-04-07 | **Spec**: `/specs/007-wsdl-integration-tests/spec.md`
**Input**: Feature specification from `/specs/007-wsdl-integration-tests/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add a dedicated integration-only test project that covers every in-scope WSDL fixture under `tests/Integration/wsdl-fixtures`, prioritizes valid single-document fixtures as the baseline MVP, preserves one clearly named test method per WSDL file, applies a shared success baseline plus richer fixture-specific assertions, and validates invalid or multi-document scenarios through stable failure-classification and explicit network-entry-point coverage.

## Technical Context

**Language/Version**: C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0`  
**Primary Dependencies**: Existing library package graph plus `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` in a dedicated integration test project; DI usage continues through `Microsoft.Extensions.DependencyInjection` and repository parser abstractions such as `IParserOrchestrationService` and `IWsdlParser`  
**Storage**: Filesystem-backed WSDL/XSD fixtures under `tests/Integration/wsdl-fixtures` and source-controlled integration test code under `tests/Integration`  
**Testing**: Dedicated integration tests under `tests/Integration` only, with one method per WSDL file, grouped category or fixture-set classes, scenario-level network tests for multi-document fixture sets, and stable failure-category assertions for invalid WSDLs  
**Target Platform**: Cross-platform `dotnet test` workflows on macOS, Windows, and Linux
**Project Type**: Multi-targeted library repository with a dedicated integration-only test project  
**Performance Goals**: Keep the full fixture suite suitable for routine local and CI `dotnet test` execution, avoid external network access, and reuse shared helpers so one test method does not perform redundant fixture orchestration beyond what the scenario requires  
**Constraints**: No unit-test or contract-test suites, no public production API changes, XML documentation comments required for all new or changed test classes, test methods, and reusable test abstractions, grouped classes with one method per WSDL file, standalone plus scenario-level coverage for valid multi-document WSDLs, and inline `Why` comments for non-obvious fixture-role or scenario-entry logic  
**Scale/Scope**: 19 in-scope WSDL files, 7 active fixture roots, 2 multi-document import networks, 1 invalid WSDL family, 1 new integration test project, and shared infrastructure for grouped fixture coverage

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Async-first I/O boundaries, cancellation propagation, and absence of sync-over-async are defined.
- Registry design uses a centralized, ID-based source of truth for shared type definitions.
- Naming contracts for enums, models, interfaces, and implementations are explicitly satisfied or justified.
- Target framework support and C# 10.0-only language-version constraints comply with the active constitution.
- DI registration, lifetime ownership, and integration verification boundaries are documented.
- Automated verification, if requested, is planned under `tests/Integration` only with no unit-test or contract-test suites.
- Full C# XML documentation expectations for types and members, and rationale-comment hotspots for complex logic, are documented.

Result: PASS. The feature remains limited to integration-test-side artifacts, exercises the DI-composed parser surface, preserves the existing library API, and aligns with the integration-only testing policy captured in constitution v1.4.0 and repository testing notes.

## Project Structure

### Documentation (this feature)

```text
specs/007-wsdl-integration-tests/
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
  ├── XsdXmlParser.Core.IntegrationTests.csproj
  ├── Infrastructure/
  │   ├── AssertionProfiles/
  │   ├── FixtureCatalog.cs
  │   ├── FixturePathResolver.cs
  │   ├── FixtureServiceProviderFactory.cs
  │   └── WsdlFixtureIntegrationTestBase.cs
  ├── FixtureCategories/
  │   ├── ComplexWsdlFixtureTests.cs
  │   ├── DownloadWebFixtureTests.cs
  │   ├── InvalidWsdlFixtureTests.cs
  │   ├── SimpleWsdlFixtureTests.cs
  │   ├── SimplestWsdlFixtureTests.cs
  │   └── VeryComplexWsdlFixtureTests.cs
  ├── FixtureSets/
  │   ├── ImportChainNetworkFixtureTests.cs
  │   └── ShippingNetworkFixtureTests.cs
  └── wsdl-fixtures/
    ├── complex-wsdls/
    ├── download-web/
    ├── invalid-wsdls/
    ├── simple-wsdls/
    ├── simplest-wsdls/
    ├── very-complex-wsdls/
    └── very-complex-wsdls-with-xsd-imports/
```

**Structure Decision**: Keep the existing library source layout unchanged and add one dedicated integration test project under `tests/Integration`. Group valid single-document fixtures by category, isolate invalid and multi-document networks into explicit category or fixture-set classes, and centralize repeated orchestration and assertion behavior in `Infrastructure/` only where it improves clarity.

## Phase 0 Research Summary

- A dedicated integration test project is required because `XsdXmlParser.csproj` removes `tests/**/*.cs` from compilation.
- Grouped classes with one test method per WSDL file provide the best balance of traceability and maintainability.
- The DI-resolved `IParserOrchestrationService` is the default integration entry point because it exercises the composed registration path exposed by `AddXsdXmlParser()`.
- Valid fixtures should use a shared baseline assertion profile plus category- or fixture-specific assertions.
- Invalid fixtures should assert stable failure classifications or diagnostic families rather than exact payloads.
- Multi-document fixture sets require both standalone WSDL coverage, where independently valid, and explicit scenario-level entry-point tests.

## Phase 1 Design Summary

- The core design entities are the fixture catalog, categories, fixture sets, fixture documents, grouped integration classes, fixture test methods, reusable test base classes, and success or failure assertion profiles.
- The primary contributor-facing contract is the coverage mapping from each WSDL file to one named test method, plus discoverable grouped class organization and explicit scenario-level network coverage.
- User Story 1 is intentionally narrowed to valid single-document fixtures so baseline coverage can be delivered independently before invalid and multi-document scenarios are layered in.
- Quickstart validation focuses on discoverability, one-method-per-WSDL traceability, shared baseline assertions, invalid classification assertions, explicit network entry points, and runnable `dotnet test` execution.

## Post-Design Constitution Re-Check

- Async orchestration remains verified through composed service calls that honor the existing async-first parser surface.
- Central registry ownership remains unchanged; the feature validates outcomes through public parser graphs rather than duplicating registry structures.
- Naming contracts remain satisfied by grouped test classes and focused helper abstractions with single-responsibility names.
- The design remains within `net6.0`, `net7.0`, `net8.0`, and C# 10.0 compatibility boundaries.
- DI ownership is explicit through a dedicated integration project that resolves `IParserOrchestrationService` and related parser abstractions from `AddXsdXmlParser()`.
- Automated verification remains fully within `tests/Integration` and introduces no unit-test or contract-test suites.
- XML documentation and `Why` comment hotspots are identified for shared fixture-loading abstractions, assertion profiles, fixture-role mapping, and multi-document scenario entry-point orchestration.

## Complexity Tracking

No constitution violations or special complexity justifications are required for this feature at plan time.

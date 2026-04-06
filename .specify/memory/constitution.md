<!--
Sync Impact Report
Version change: 1.2.0 -> 1.3.0
Modified principles:
- V. Fully Documented C# Surface
Added sections:
None
Removed sections:
None
Templates requiring updates:
- None
- ⚠ pending .specify/templates/commands/*.md (directory absent in this repository)
- ✅ .github/copilot-instructions.md
Follow-up TODOs:
- TODO(RATIFICATION_DATE): original ratification date is not recorded in repository history.
-->

# XsdXmlParser.Core Constitution

## Core Principles

### I. Async-First Library Architecture
All external I/O, schema loading, parsing, metadata generation, and XML generation MUST be exposed through asynchronous APIs. Public methods that can block on file, stream, network, or large-document processing MUST accept a `CancellationToken` and MUST NOT use sync-over-async patterns such as `.Result`, `.Wait()`, or hidden blocking wrappers. Rationale: the library is intended for hosted and tooling scenarios where scalable concurrency and responsive cancellation are mandatory.

### II. Central Registry, No Duplicate Types
Type definitions MUST be stored exactly once in a centralized, ID-based registry that acts as the canonical source of truth. Models, parsers, validators, and generators MUST reference type identities through registry IDs or resolvable references instead of duplicating schema definitions in multiple object graphs. Rationale: strict DRY for type metadata reduces divergence, simplifies cross-schema resolution, and keeps large schemas maintainable.

### III. Stable Naming Contracts
Enums MUST use the `E` prefix, models MUST use the `Model` suffix, and interfaces MUST start with `I`. Public abstractions and implementations MUST use names that reflect their single responsibility, such as `IXsdParser`, `TypeRegistry`, or `XmlGeneratorService`, and naming exceptions MUST be documented in the plan artifact. Rationale: stable naming is part of the public contract for a NuGet library and directly affects discoverability, consistency, and review quality.

### IV. Multi-Target NuGet Compatibility
The repository MUST be governed as a multi-targeted NuGet library targeting `net6.0`, `net7.0`, and `net8.0`; console-app-only assumptions, tooling, or release rules MUST NOT define the default engineering workflow. Shared production code MUST remain compatible with C# 10.0 only, and features that require newer language versions, including `required` members, primary constructors, list patterns, collection expressions, and file-local types, MUST NOT appear in shared production code. Rationale: NuGet consumers need a stable compatibility matrix, and governance must optimize for reusable package delivery rather than executable-host assumptions.

### V. Fully Documented C# Surface
Core services MUST be reachable through dependency-injection-friendly abstractions and registration extensions suitable for host applications and tests. Every C# class, struct, enum, interface, property, method, constructor, delegate, event, field, and other declaration in shared production code MUST include XML documentation comments across all declaration visibility levels, using `<summary>` universally and adding `<param>`, `<typeparam>`, `<returns>`, `<value>`, `<remarks>`, `<exception>`, `<example>`, `<paramref>`, `<typeparamref>`, `<see>`, or `<seealso>` wherever applicable. Existing accurate XML documentation intent MUST be preserved and enhanced rather than replaced indiscriminately, examples MUST be added selectively when they materially improve understanding, and inline reference tags SHOULD be preferred over plain text when they improve precision or generated reference quality. Public async methods MUST use the `Async` suffix. Lifetimes, registry ownership, and parser composition MUST be explicit, mockable, and reviewable, with static access patterns avoided unless they are internal implementation details behind DI-managed services. Rationale: production-grade library integration depends on composability, testability, complete IntelliSense discoverability, consistent source-level maintainability, and generated package documentation that remains useful for both consumer-facing and lower-visibility declarations.

## Engineering Standards

- Published package identity and documentation MUST refer to the library as `XsdXmlParser.Core`.
- The codebase MUST retain Clean Architecture boundaries between models, parsing, validation, registry management, and output generation.
- Graph traversal remains the required mechanism for recursion, cross-reference resolution, and cycle-safe navigation.
- All C# declarations in shared production code, regardless of visibility, MUST carry XML documentation comments that accurately describe intent, inputs, outputs, values, references, and usage constraints using the applicable XML tag set.
- XML documentation updates MUST remain documentation-only unless a separate approved feature explicitly authorizes runtime, structural, signature, or behavioral code changes.
- Accurate existing XML documentation MUST be preserved and expanded in place, while placeholder, redundant, or low-information comment text MUST be replaced with meaningful explanation.
- Complex logic, including XSD traversal, schema-linking heuristics, cycle handling, and canonicalization rules, MUST include inline `Why` comments where the intent would not otherwise be obvious from the code alone.
- Compatibility validation MUST run against `net6.0`, `net7.0`, and `net8.0` for every release candidate.

## Delivery Workflow

- Every feature plan MUST include a constitution check covering async I/O, registry canonicalization, naming contracts, target framework support, DI boundaries, all-visibility C# XML documentation expectations, selective example policy, inline reference-tag usage, documentation-only scope constraints when applicable, and rationale-comment hotspots for complex logic.
- Every task plan MUST include foundational work for async service contracts, centralized registry implementation, DI registration, XML documentation coverage for C# declarations across all relevant visibility levels, preservation of existing accurate comment intent, and multi-target validation.
- Reviews MUST reject duplicated type-definition storage, undocumented naming exceptions, missing XML documentation on any in-scope shared-production declaration, missing required applicable XML tags, speculative or misleading XML documentation text, missing required `Why` comments in complex logic, and any use of C# features newer than 10.0 in shared code.
- Performance and cancellation validation MUST occur before feature work is considered implementation-complete for parsing pipelines.
- Release notes and package metadata MUST describe the supported target framework matrix and async-first API expectations.

## Governance

- This constitution supersedes conflicting repository guidance for architecture, naming, compatibility, and delivery rules.
- Amendments MUST be delivered in a pull request that includes the rationale, a Sync Impact Report, and updates to affected templates, prompts, or runtime guidance files.
- Constitution versioning follows semantic versioning: MAJOR for incompatible principle changes or removals, MINOR for new principles or materially expanded sections, PATCH for clarifications and wording-only corrections.
- Compliance reviews are required at spec, plan, task-generation, and pull-request time. Reviewers MUST block changes that violate the constitution without an approved amendment.
- Ratification history before this document is incomplete; `TODO(RATIFICATION_DATE): original adoption date not recorded in repository history.`

**Version**: 1.3.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date not recorded in repository history. | **Last Amended**: 2026-04-06
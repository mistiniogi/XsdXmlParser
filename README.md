# XsdXmlParser.Core

Production-grade NuGet library for parsing WSDL 1.1 and XSD schemas, building a centralized ID-based type registry, and generating JSON metadata and XML output.

## Engineering Direction

- Targets `net6.0`, `net7.0`, and `net8.0`
- Shared production code remains compatible with C# 10.0
- Public parsing and generation APIs are async-first
- All C# classes, structs, enums, properties, methods, and other shared production declarations carry XML documentation tags appropriate to their shape
- Core services are designed for dependency injection and testability
- Complex traversal and normalization logic uses inline `Why` comments where intent is non-obvious
- Type definitions are canonicalized through a centralized registry to enforce DRY

## Planning Tooling

This repository uses Spec Kit artifacts under `.specify/` and `specs/` to manage feature planning, constitution checks, and implementation tasks.

## Planned Source Layout

- `src/Abstractions`: parser, loader, VFS, and serializer contracts
- `src/Models`: normalized graph, source, registry, and diagnostic models
- `src/Registry`: canonical registration, source traversal state, and deterministic `RefId` helpers
- `src/Parsing`: source normalization, schema discovery, and two-pass linkage services
- `src/Serialization`: `System.Text.Json` graph serialization and targeted converters
- `src/Extensions`: dependency injection registration

## Planned Parsing Flow

1. Normalize input sources into stable logical identities and paths.
2. Discover reachable schemas and register canonical shells in Pass 1.
3. Link references, constraints, and flattened compositor metadata in Pass 2.
4. Serialize the normalized graph for downstream validation and generation workflows.
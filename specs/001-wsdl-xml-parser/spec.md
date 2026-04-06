# WSDL XML Parser Feature Specification

## Overview
Develop the `XsdXmlParser.Core` library to parse WSDL and XSD schemas, generate JSON metadata and XML, and enforce a centralized ID-based type registry. The project emphasizes schema-first design, graph-based processing, clean separation of concerns, and async-first APIs. It is multi-targeted to support .NET 6.0, .NET 7.0, and .NET 8.0 while keeping shared production code compatible with C# 10.0.

## Clarifications
### Session 2026-04-05
- Q: Which WSDL version should be supported? → A: WSDL 1.1 only.
- Q: Should the project deliver a library API, CLI tool, or both? → A: Library API only (no CLI).
- Q: What performance and scale targets should the parser meet? → A: Handle schemas up to 10,000 elements with sub-second parsing (<1 second).
- Q: How should the parser handle schema errors—strict or lenient? → A: Strict (fail fast with exceptions on parsing/validation errors).
- Q: Which XSD constraint features should be supported? → A: Full XSD facets (length, pattern, enumeration, precision) plus documentation annotations (parse documentation elements and attach to corresponding types/elements).
- Q: Which XSD elements require element-to-class mapping? → A: All named elements (top-level and locally-scoped), excluding anonymous inline elements.
- Q: What naming convention for element-parsing classes? → A: Use "ElementParser" suffix for element-parsing classes (e.g., PersonElementParser); use "Model" suffix for data POCOs (e.g., PersonModel).
- Q: How are element parsers instantiated and managed? → A: Stateless singletons registered in TypeRegistry at schema load time; reused across multiple parse operations, with parse state passed as method parameters.

## Requirements
- FR-001: The library shall parse WSDL 1.1 files.
- FR-002: The library shall parse XSD (XML Schema Definition) schemas.
- FR-003: The library shall generate JSON metadata from parsed schemas.
- FR-004: The library shall generate XML output based on parsed schemas and input data.
- FR-005: The implementation shall follow schema-first design principles.
- FR-006: The implementation shall use graph-based processing for handling complex relationships, recursion, and cross-schema references.
- FR-007: The architecture shall maintain clean separation of concerns across parsing, validation, metadata generation, and XML generation.
- FR-008: The library shall support multi-targeting for .NET 6.0, .NET 7.0, and .NET 8.0.
- FR-009: The codebase shall remain compatible with C# 10.0 across shared production code and shall not depend on newer language features.
- FR-010: Every named XSD element shall map to its own independent `ElementParser` class, ensuring strict one-to-one mapping between schema elements and generated parser classes; anonymous inline elements are excluded.
- FR-011: The library shall expose a clean, well-documented public API for parsing and generation operations.
- FR-012: The library shall use an async-first architecture. Public parsing and generation APIs shall be asynchronous, propagate `CancellationToken` where appropriate, and avoid sync-over-async behavior.
- FR-013: The library shall maintain a centralized, ID-based registry as the single source of truth for type definitions and parser resolution to enforce DRY across schemas.

## Technical Approach
- Use Clean Architecture as per constitution
- Implement graph traversal for navigation and recursion handling
- Use a centralized, ID-based Type Registry for canonical storage and reference resolution
- Use an async-first service architecture for all public parsing and generation workflows
- Propagate `CancellationToken` through service and parser call chains
- Follow naming conventions:
  - Enums: prefix with 'E' (e.g., EElementType)
  - Data POCOs (models): suffix with 'Model' (e.g., PersonModel, OrderModel)
  - Interfaces: start with 'I' (e.g., IWSDLParser, IXSDParser)
  - Element-parsing classes: suffix with 'ElementParser' (e.g., PersonElementParser, OrderElementParser)
  - Service implementations: suffix with responsibility (e.g., WSDLParserService, XSDParserService)
- Support multi-targeting with conditional compilation and target-specific optimizations
- Keep shared production code compatible with C# 10.0 for all supported targets (.NET 6.0, .NET 7.0, and .NET 8.0)
- Avoid C# features newer than 10.0, including primary constructors and collection expressions, in shared production code
- Implement strict element-to-class mapping for each named XSD element parsed from the schema

## Detailed Specification
- Parse WSDL 1.1 files and extract service definitions, port types, bindings, messages, and embedded schema references.
- Parse XSD schemas and build an in-memory schema graph containing elements, complex/simple types, attributes, sequences, choices, imports, and includes.
- **Element-to-Class Mapping (Strict)**: Create an independent parsing class for each named XSD element (both top-level and locally-scoped) encountered during parsing; exclude anonymous inline elements. Ensure one-to-one correspondence between named schema elements and parser class instances; maintain this mapping in the schema graph for reference and validation.
- **Element Parser Lifecycle**: Register element parsers as stateless singletons in the TypeRegistry at schema load time, keyed by stable registry IDs, element name, and namespace. Reuse parsers across multiple parse operations; pass parse state (position, depth, context) as method parameters rather than instance fields to maintain statelessness.
- Expose asynchronous `ParseAsync` and `GenerateAsync` entry points as the primary public API surface.
- Ensure schema loading, import/include resolution, validation, metadata generation, and XML generation all participate in the asynchronous execution model.
- Do not block asynchronous flows with `.Result`, `.Wait()`, or equivalent sync-over-async patterns.
- Support XSD facet constraints: length, minLength, maxLength, pattern (regex), enumeration values, totalDigits, fractionDigits, minInclusive, maxInclusive, minExclusive, maxExclusive.
- Parse and record XSD annotation documentation (xs:documentation) for each type, element, and attribute; attach documentation text as metadata accessible via the public API.
- Support schema references across multiple files via import/include resolution and namespace-aware type lookup.
- Use a centralized `TypeRegistry` to store canonical type definitions by stable IDs, resolve schema types, and prevent duplicate type definitions.
- Produce JSON metadata that serializes the parsed schema graph, including element names, types, cardinality, namespaces, and relationships.
- Generate XML output from the parsed schema and sample data, ensuring element order, namespaces, and cardinality rules are respected.
- Validate parsed schema structures during parsing, reporting missing type references, invalid cardinality, and namespace mismatches.
- **Error Handling (Strict Mode)**: Throw detailed, actionable exceptions on any parsing or validation error; include line/column information and schema location when available to aid consumer debugging.
- Handle recursive type definitions and cycles safely using graph traversal state tracking.
- Keep parsing, validation, metadata generation, and XML generation separated into distinct services and implementation layers.
- **Performance & Scale**: Parse schemas containing up to 10,000 elements in under 1 second; keep memory footprint reasonable for typical business WSDL/XSD files (target <500 MB for 10k-element schemas).

## Success Criteria
- SC-001: Successfully parse WSDL 1.1 files with embedded XSD schemas and complex type definitions.
- SC-002: Successfully parse standalone XSD schemas up to 10,000 elements within 1 second under the defined benchmark workload.
- SC-003: Generate well-formed, schema-valid JSON metadata from parsed schemas.
- SC-004: Generate valid XML output that conforms to parsed schema constraints.
- SC-005: Demonstrate graph-based type resolution handling circular references safely.
- SC-006: Adhere to Clean Architecture with clear separation between models, services, and implementations.
- SC-007: Provide comprehensive public API documentation with code examples for each major function.
- SC-008: Document all public classes and public methods with XML comments suitable for library consumers.
- SC-009: Complete asynchronous parsing and generation workflows without sync-over-async blocking and with cancellation support.
- SC-010: Build and test successfully against `net6.0`, `net7.0`, and `net8.0` without requiring language features newer than C# 10.0.
# XsdXmlParser Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-04-05

## Active Technologies
- C# 10.0 with .NET 6.0, .NET 7.0, and .NET 8.0 + System.Xml, System.Text.Json, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging.Abstractions (002-multi-source-parser)
- In-memory centralized ID-based registry and normalized metadata graph dictionaries (002-multi-source-parser)

- C# 10.0 with .NET 6, .NET 7, and .NET 8 (001-wsdl-xml-parser)

## Project Structure

```text
src/
tests/
```

## Commands

# Add commands for C# 10.0 with .NET 6, .NET 7, and .NET 8

## Code Style

C# 10.0 across shared code: follow standard conventions and avoid newer language features in shared production files

All C# classes, structs, enums, interfaces, properties, methods, constructors, delegates, events, and other shared production declarations must include XML documentation tags (`<summary>`, `<param>`, `<returns>`, `<value>` where applicable)

Complex logic such as XSD traversal, canonicalization, and cycle handling should include succinct inline Why comments when intent is not obvious from the code

## Recent Changes
- 002-multi-source-parser: Added C# 10.0 with .NET 6.0, .NET 7.0, and .NET 8.0 + System.Xml, System.Text.Json, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging.Abstractions

- 001-wsdl-xml-parser: Adopted async-first DI-friendly library rules for C# 10.0 on .NET 6, .NET 7, and .NET 8

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->

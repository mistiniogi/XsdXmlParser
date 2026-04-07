# XsdXmlParser Development Guidelines
Auto-generated from all feature plans. Last updated: 2026-04-07

## ⚠️ Design Preservation Rules (Apply to ALL New Features)

When adding or modifying code for any new speckit feature, Copilot MUST:

1. **Preserve existing class design** — Do not rename, restructure, or alter the signature of any existing public class, interface, struct, enum, method, property, constructor, or delegate unless the feature plan explicitly requires it.
2. **Respect existing design principles** — All new code must follow the same architectural patterns already in use:
   - Async-first, DI-friendly library design
   - Centralized ID-based registry and normalized metadata graph dictionaries
   - Separation of concerns between parsing, resolution, and metadata layers
3. **Extend, don't replace** — Prefer adding new classes/interfaces alongside existing ones 
rather than modifying them. If a modification to an existing class is genuinely required 
by the feature, make the smallest possible change that satisfies the requirement:
- Do not alter existing method signatures — add overloads instead
- Do not remove or rename existing members
- Do not change existing behavior — only add to it
- Flag any such modification with a `// MODIFIED: <feature-name> — <reason>` 
  inline comment so it is visible in review and traceable to the feature plan.
4. **Match existing naming conventions** — Follow the exact same naming patterns (PascalCase for types/members, camelCase for locals/parameters, `I` prefix for interfaces, `Base` suffix for abstract classes where applicable).
5. **Keep multi-target compatibility** — All new shared production code must compile and run correctly on .NET 6.0, .NET 7.0, and .NET 8.0. Do not use APIs or language features exclusive to a single target.
6. **Maintain XML documentation** — Every new declaration in shared production code, regardless of visibility, must include XML documentation comments with `<summary>` and all other applicable tags such as `<param>`, `<typeparam>`, `<returns>`, `<value>`, `<remarks>`, `<exception>`, `<example>`, `<paramref>`, `<typeparamref>`, `<see>`, and `<seealso>`. Preserve accurate existing intent when enhancing comments.
7. **Do not remove inline Why comments** — Preserve all existing inline comments explaining non-obvious logic (XSD traversal, canonicalization, cycle handling). Add equivalent comments to any new complex logic.

## Active Technologies
- C# 10.0 with .NET 6.0, .NET 7.0, and .NET 8.0 + System.Xml, System.Text.Json, Microsoft.Extensions.DependencyInjection, Microsoft.Extensions.Logging.Abstractions (002-multi-source-parser)
- In-memory centralized ID-based registry and normalized metadata graph dictionaries (002-multi-source-parser)
- C# 10.0 with .NET 6, .NET 7, and .NET 8 (001-wsdl-xml-parser)
- C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0 + `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions` (003-complete-async-parser)
- In-memory source descriptors, centralized registry services, and normalized metadata graph dictionaries (003-complete-async-parser)
- C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0 + `System.Xml`, `System.Text`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions` (004-simplify-source-loading)
- In-memory source descriptors, virtual file metadata, centralized registry services, and normalized metadata graph dictionaries (004-simplify-source-loading)
- C# 10.0 on .NET 6.0, .NET 7.0, and .NET 8.0 + `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, analyzer packages configured in the project file (005-enhance-xml-docs)
- N/A for this feature; the work is limited to source-file XML documentation comments in existing `.cs` files (005-enhance-xml-docs)
- C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` + `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, analyzer packages configured in the project file (005-enhance-xml-docs)
- C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` for the host repository; WSDL 1.1 and XSD fixture files only for this feature + Existing library dependencies remain unchanged: `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, analyzer packages configured in the project file (006-wsdl-test-assets)
- Filesystem-backed test fixture assets under `tests/Integration/wsdl-fixtures` (006-wsdl-test-assets)
- C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` + Existing library package graph plus `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` in a dedicated integration test project; DI usage continues through `Microsoft.Extensions.DependencyInjection` and repository parser abstractions (007-wsdl-integration-tests)
- Filesystem-backed WSDL/XSD fixtures under `tests/Integration/wsdl-fixtures` and source-controlled integration test code under `tests/Integration` (007-wsdl-integration-tests)
- C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` + Existing library package graph plus `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` in a dedicated integration test project; DI usage continues through `Microsoft.Extensions.DependencyInjection` and repository parser abstractions such as `IParserOrchestrationService` and `IWsdlParser` (007-wsdl-integration-tests)

## Project Structure
```text
src/
tests/
```

## Commands
# Add commands for C# 10.0 with .NET 6, .NET 7, and .NET 8

## Code Style
- C# 10.0 across shared code: follow standard conventions and avoid newer language features in shared production files
- All C# declarations in shared production code, regardless of visibility, must include XML documentation comments with the applicable XML tag set (`<summary>`, `<param>`, `<typeparam>`, `<returns>`, `<value>`, `<remarks>`, `<exception>`, `<example>`, `<paramref>`, `<typeparamref>`, `<see>`, and `<seealso>` where relevant)
- Complex logic such as XSD traversal, canonicalization, and cycle handling should include succinct inline Why comments when intent is not obvious from the code

## New Feature Checklist (Copilot must verify before generating code)

Before generating any new file or modifying an existing one for a speckit feature, confirm:

- [ ] No existing public API surface is broken or altered without explicit instruction
- [ ] New types follow the established class hierarchy and abstraction layers
- [ ] All new public members have XML doc comments
- [ ] Code compiles on .NET 6, .NET 7, and .NET 8 (no single-target APIs)
- [ ] New files are placed in the correct `src/` or `tests/` subfolder consistent with existing structure
- [ ] DI registrations (if any) follow the existing `IServiceCollection` extension pattern
- [ ] No hardcoded strings — use constants or resource identifiers consistent with existing patterns

## Recent Changes
- 007-wsdl-integration-tests: Added C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` + Existing library package graph plus `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` in a dedicated integration test project; DI usage continues through `Microsoft.Extensions.DependencyInjection` and repository parser abstractions such as `IParserOrchestrationService` and `IWsdlParser`
- 007-wsdl-integration-tests: Added C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` + Existing library package graph plus `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` in a dedicated integration test project; DI usage continues through `Microsoft.Extensions.DependencyInjection` and repository parser abstractions
- 006-wsdl-test-assets: Added C# 10.0 on .NET `net6.0`, `net7.0`, and `net8.0` for the host repository; WSDL 1.1 and XSD fixture files only for this feature + Existing library dependencies remain unchanged: `System.Xml`, `System.Text.Json`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`, analyzer packages configured in the project file

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->

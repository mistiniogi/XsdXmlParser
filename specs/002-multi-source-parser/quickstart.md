# Quickstart: Multi-Source XSD/WSDL Parser Library

## Goal

Parse XSD or WSDL content from any supported source, resolve multi-file schema sets with a designated main source, and serialize a normalized metadata graph.

## Core Components

- `SchemaRegistryService`: canonical registry for discovered definitions and deterministic `RefId` assignment.
- `XsdGraphBuilder`: Pass 1 XSD discovery and registry population.
- `WsdlDiscoveryService`: WSDL discovery and schema entry-point resolution.
- `IVirtualFileSystem`: abstraction that exposes file-backed and memory-backed sources to the parsing pipeline.

## Planned Source Layout

- `src/Abstractions`: public parser, loader, serializer, and virtual file system contracts.
- `src/Models`: source descriptors, registry entries, metadata graph models, and diagnostics.
- `src/Registry`: canonical registration and deterministic `RefId` helpers.
- `src/Parsing`: request normalization, discovery, import resolution, and graph linkage.
- `src/Serialization`: JSON graph serializer and targeted converters.
- `src/Extensions`: DI registration for parser and serialization services.

## Prerequisites

- .NET 6.0, .NET 7.0, or .NET 8.0 SDK
- `XsdXmlParser.Core` package reference
- Async-capable calling code with cancellation support

## Parse From A File Path

```csharp
using var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var parser = serviceProvider.GetRequiredService<IWsdlParser>();
var graph = await parser.ParseFromFileAsync("schemas/service.wsdl", cancellationToken);
```

## Parse From A Stream

```csharp
await using var stream = File.OpenRead("schemas/schema.xsd");
var parser = serviceProvider.GetRequiredService<IXsdParser>();
var graph = await parser.ParseFromStreamAsync(stream, cancellationToken);
```

## Parse From Memory

```csharp
ReadOnlyMemory<byte> buffer = await File.ReadAllBytesAsync("schemas/schema.xsd", cancellationToken);
var parser = serviceProvider.GetRequiredService<IXsdParser>();
var graph = await parser.ParseFromMemoryAsync(buffer, cancellationToken);
```

## Parse A Multi-File Batch

```csharp
var parser = serviceProvider.GetRequiredService<IXsdParser>();
var sources = new[]
{
    new BatchSourceRequest("main.xsd", File.OpenRead("schemas/main.xsd"), isMain: true),
    new BatchSourceRequest("common/types.xsd", File.OpenRead("schemas/common/types.xsd")),
    new BatchSourceRequest("shared/attributes.xsd", File.OpenRead("schemas/shared/attributes.xsd"))
};

var graph = await parser.ParseBatchAsync(sources, cancellationToken);
```

## Recommended Parsing Flow

1. `WsdlDiscoveryService` or `XsdGraphBuilder` discovers reachable schema artifacts through `IVirtualFileSystem`.
2. Pass 1 populates `SchemaRegistryService` with canonical definition shells.
3. Pass 2 assigns references, inheritance, occurrence rules, and flattened compositor metadata.
4. `MetadataGraphJsonSerializer` serializes the final dictionary-based graph with `System.Text.Json`.

## Implementation Notes

- Source normalization and identity validation happen before discovery begins.
- Main-source validation and unresolved batch-source failures are treated as request-validation concerns, not late linkage errors.
- Non-seekable streams are rejected with actionable diagnostics instead of buffered implicitly.

## Inspect The Normalized Graph

```csharp
var serializer = serviceProvider.GetRequiredService<IMetadataGraphSerializer>();
string json = await serializer.SerializeAsync(graph, cancellationToken);
Console.WriteLine(json);
```

## Expected Output Characteristics

- Complex types, simple types, elements, and attributes are emitted in separate top-level dictionaries.
- Child relationships are expressed through stable `RefId` values.
- Circular imports are resolved through cycle-safe linking rather than recursive duplication.
- Validation-relevant metadata such as occurrence bounds, base-type lineage, and pattern-style rules are preserved.
- `System.Text.Json` serialization preserves the dictionary-based graph contract and can use targeted converters for special value shapes.

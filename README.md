# XsdXmlParser.Core

`XsdXmlParser.Core` is a multi-targeted .NET library for parsing WSDL 1.1 and XSD sources into a normalized metadata graph with canonical reference identifiers, relationship metadata, and serializer hints.

## Targets

- `net6.0`
- `net7.0`
- `net8.0`
- C# 10.0-compatible shared production code

## Main Entry Point

The primary consumer-facing API is `IParserOrchestrationService`, registered through `AddXsdXmlParser()`.

```csharp
var services = new ServiceCollection();
services.AddXsdXmlParser();

using var serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetRequiredService<IParserOrchestrationService>();
```

## Supported Request Shapes

- `FilePathParseRequestModel`
- `StreamParseRequestModel`
- `MemoryParseRequestModel`
- `BatchParseRequestModel`

Every request must declare `DocumentKind` explicitly as `ESchemaDocumentKind.Wsdl` or `ESchemaDocumentKind.Xsd`.

## Parse A WSDL File

```csharp
var graph = await parser.ParseFileAsync(
	new FilePathParseRequestModel
	{
		FilePath = "schemas/service.wsdl",
		DisplayName = "service.wsdl",
		LogicalPath = "schemas/service.wsdl",
		DocumentKind = ESchemaDocumentKind.Wsdl,
	},
	cancellationToken);
```

## Parse An XSD Stream

```csharp
await using var stream = File.OpenRead("schemas/types.xsd");

var graph = await parser.ParseStreamAsync(
	new StreamParseRequestModel
	{
		Content = stream,
		DisplayName = "types.xsd",
		LogicalPath = "schemas/types.xsd",
		DocumentKind = ESchemaDocumentKind.Xsd,
	},
	cancellationToken);
```

## Parse A Batch

```csharp
var graph = await parser.ParseBatchAsync(
	new BatchParseRequestModel
	{
		Sources = new[]
		{
			new BatchSourceRequestModel
			{
				LogicalName = "main.wsdl",
				LogicalPath = "schemas/main.wsdl",
				Content = File.OpenRead("schemas/main.wsdl"),
				DocumentKind = ESchemaDocumentKind.Wsdl,
				IsMain = true,
			},
			new BatchSourceRequestModel
			{
				LogicalName = "types.xsd",
				LogicalPath = "schemas/types.xsd",
				Content = File.OpenRead("schemas/types.xsd"),
				DocumentKind = ESchemaDocumentKind.Xsd,
			},
		},
	},
	cancellationToken);
```

## Failure Contract

Invalid requests and parse-stage failures throw `ParseFailureException`. The exception exposes structured `ParseDiagnosticModel` entries, the failure stage, and the primary failing source when available.

```csharp
try
{
	await parser.ParseFileAsync(request, cancellationToken);
}
catch (ParseFailureException ex)
{
	foreach (var diagnostic in ex.Diagnostics)
	{
		Console.WriteLine($"{diagnostic.Stage}: {diagnostic.Code} - {diagnostic.Message}");
	}
}
```

## Parsing Pipeline

1. `ISourceLoader` validates and normalizes the request into `SourceDescriptorModel` entries.
2. `WsdlDiscoveryService` expands WSDL imports and schema references before graph construction.
3. `XsdGraphBuilder` walks WSDL inline schemas and XSD documents, dispatching category-specific items to dedicated handlers.
4. Registry services canonicalize types, elements, attributes, relationships, and constraint metadata into `MetadataGraphModel`.

## Repository Structure

- `src/Abstractions`: public contracts and internal parsing seams
- `src/Models`: request models, graph models, diagnostics, and exceptions
- `src/Registry`: canonical registration and reference identifier helpers
- `src/Parsing`: source loading, WSDL discovery, orchestration, and item handlers
- `src/Serialization`: graph serialization support
- `src/Extensions`: dependency injection registration
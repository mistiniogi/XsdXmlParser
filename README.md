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
- `StringParseRequestModel`

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

## Parse An XSD String

```csharp
const string schemaText =
	"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" +
	"<xs:element name=\"customer\" type=\"xs:string\" />" +
	"</xs:schema>";

var graph = await parser.ParseStringAsync(
	new StringParseRequestModel
	{
		Content = schemaText,
		DisplayName = "types.xsd",
		LogicalPath = "schemas/types.xsd",
		DocumentKind = ESchemaDocumentKind.Xsd,
	},
	cancellationToken);
```

## Use Typed Parser Adapters

```csharp
var xsdParser = serviceProvider.GetRequiredService<IXsdParser>();
var graph = await xsdParser.ParseFromStringAsync(schemaText, "schemas/types.xsd", cancellationToken);
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
# Getting Started

## Register The Parser

```csharp
var services = new ServiceCollection();
services.AddXsdXmlParser();

using var serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetRequiredService<IParserOrchestrationService>();
```

## Choose A Request Model

Use the request model that matches your input source:

- `FilePathParseRequestModel` for file-backed content
- `StreamParseRequestModel` for readable, seekable streams
- `MemoryParseRequestModel` for `ReadOnlyMemory<byte>` buffers
- `BatchParseRequestModel` for coordinated multi-source parsing

Each request or batch source must declare `DocumentKind` explicitly.

## Parse A File

```csharp
var graph = await parser.ParseFileAsync(
	new FilePathParseRequestModel
	{
		FilePath = "schemas/customer.xsd",
		DisplayName = "customer.xsd",
		LogicalPath = "schemas/customer.xsd",
		DocumentKind = ESchemaDocumentKind.Xsd,
	},
	cancellationToken);
```

## Parse A Stream

```csharp
await using var stream = File.OpenRead("schemas/service.wsdl");

var graph = await parser.ParseStreamAsync(
	new StreamParseRequestModel
	{
		Content = stream,
		DisplayName = "service.wsdl",
		LogicalPath = "schemas/service.wsdl",
		DocumentKind = ESchemaDocumentKind.Wsdl,
	},
	cancellationToken);
```

## Parse Memory Content

```csharp
ReadOnlyMemory<byte> buffer = await File.ReadAllBytesAsync("schemas/common.xsd", cancellationToken);

var graph = await parser.ParseMemoryAsync(
	new MemoryParseRequestModel
	{
		Buffer = buffer,
		DisplayName = "common.xsd",
		LogicalPath = "schemas/common.xsd",
		DocumentKind = ESchemaDocumentKind.Xsd,
	},
	cancellationToken);
```

## Handle Failures

```csharp
try
{
	await parser.ParseBatchAsync(batchRequest, cancellationToken);
}
catch (ParseFailureException ex)
{
	foreach (var diagnostic in ex.Diagnostics)
	{
		Console.WriteLine($"{diagnostic.Stage}: {diagnostic.Message}");
	}
}
```

## Output Shape

Successful parses return `MetadataGraphModel`, which contains:

- `Sources`
- `ComplexTypes`
- `SimpleTypes`
- `Elements`
- `Attributes`
- `Relationships`
- `ValidationRules`
- `RootRefIds`
- `SerializerHints`
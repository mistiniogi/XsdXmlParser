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
- `StringParseRequestModel` for string-backed XML content

Each request must declare `DocumentKind` explicitly. String-backed requests must also provide `LogicalPath` so relative imports and includes can resolve consistently.

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

## Parse A String

```csharp
const string wsdlText =
	"<wsdl:definitions xmlns:wsdl=\"http://schemas.xmlsoap.org/wsdl/\" " +
	"xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" +
	"<wsdl:types><xs:schema /></wsdl:types></wsdl:definitions>";

var graph = await parser.ParseStringAsync(
	new StringParseRequestModel
	{
		Content = wsdlText,
		DisplayName = "service.wsdl",
		LogicalPath = "schemas/service.wsdl",
		DocumentKind = ESchemaDocumentKind.Wsdl,
	},
	cancellationToken);
```

## Use The Typed Parser Seams

```csharp
var xsdParser = serviceProvider.GetRequiredService<IXsdParser>();

var graph = await xsdParser.ParseFromStringAsync(
	"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" />",
	"schemas/common.xsd",
	cancellationToken);
```

## Handle Failures

```csharp
var invalidRequest = new StringParseRequestModel
{
	Content = string.Empty,
	DisplayName = "invalid.xsd",
	LogicalPath = "schemas/invalid.xsd",
	DocumentKind = ESchemaDocumentKind.Xsd,
};

try
{
	await parser.ParseStringAsync(invalidRequest, cancellationToken);
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
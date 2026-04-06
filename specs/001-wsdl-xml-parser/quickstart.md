# Quickstart: WSDL XML Parser

## Installation

```bash
dotnet add package XsdXmlParser.Core
```

## Basic Usage

### Parse WSDL and Generate JSON

```csharp
using XsdXmlParser.Core;

using var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var parser = provider.GetRequiredService<IWSDLParser>();
var generator = provider.GetRequiredService<IJSONGenerator>();

string wsdlContent = await File.ReadAllTextAsync("service.wsdl", cancellationToken);
var schema = await parser.ParseAsync(wsdlContent, cancellationToken);
string json = await generator.GenerateAsync(schema, cancellationToken);

Console.WriteLine(json);
```

### Parse XSD and Generate XML

```csharp
using XsdXmlParser.Core;

using var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var parser = provider.GetRequiredService<IXsdParser>();
var xmlGenerator = provider.GetRequiredService<IXmlGenerator>();

string xsdContent = await File.ReadAllTextAsync("schema.xsd", cancellationToken);
var schema = await parser.ParseAsync(xsdContent, cancellationToken);

// Sample data matching the schema
var data = new { name = "example", value = 123 };

string xml = await xmlGenerator.GenerateAsync(schema, data, cancellationToken);
Console.WriteLine(xml);
```

## Configuration

Use dependency injection for services:

```csharp
var services = new ServiceCollection();
services.AddXsdXmlParser(); // Extension method

var provider = services.BuildServiceProvider();
var parser = provider.GetRequiredService<IWSDLParser>();
```

## Error Handling

```csharp
try
{
    var schema = await parser.ParseAsync(wsdlContent, cancellationToken);
}
catch (InvalidWsdlException ex)
{
    Console.WriteLine($"Parsing failed: {ex.Message}");
}
```

## Advanced Features

- Graph-based processing for complex relationships
- Centralized ID-based type registry for canonical type management
- Async operations for large schemas
- Validation support
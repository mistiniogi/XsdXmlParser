# Quickstart: Complete WSDL/XSD Parsing Workflows

## Goal

Use one consumer-facing orchestration service to parse WSDL or XSD content from file paths, streams, read-only memory, or batch source requests.

## Prerequisites

- .NET 6.0, .NET 7.0, or .NET 8.0 SDK
- Dependency injection configured with `AddXsdXmlParser()`
- Async-capable calling code with a `CancellationToken`

## Register The Library Services

```csharp
var services = new ServiceCollection();
services.AddXsdXmlParser();

using var serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetRequiredService<IParserOrchestrationService>();
```

## Parse A File Path

```csharp
var request = new FilePathParseRequestModel
{
    FilePath = "schemas/service.wsdl",
    DocumentKind = ESchemaDocumentKind.Wsdl,
    DisplayName = "service.wsdl",
    LogicalPath = "schemas/service.wsdl",
};

var graph = await parser.ParseFileAsync(request, cancellationToken);
```

## Parse A Stream

```csharp
await using var stream = File.OpenRead("schemas/schema.xsd");

var request = new StreamParseRequestModel
{
    DisplayName = "schema.xsd",
    LogicalPath = "schemas/schema.xsd",
    Content = stream,
    DocumentKind = ESchemaDocumentKind.Xsd,
};

var graph = await parser.ParseStreamAsync(request, cancellationToken);
```

## Parse From Memory

```csharp
ReadOnlyMemory<byte> buffer = await File.ReadAllBytesAsync("schemas/schema.xsd", cancellationToken);

var request = new MemoryParseRequestModel
{
    DisplayName = "schema.xsd",
    LogicalPath = "schemas/schema.xsd",
    Buffer = buffer,
    DocumentKind = ESchemaDocumentKind.Xsd,
};

var graph = await parser.ParseMemoryAsync(request, cancellationToken);
```

## Parse A Batch

```csharp
var request = new BatchParseRequestModel
{
    Sources = new[]
    {
        new BatchSourceRequestModel
        {
            LogicalName = "main.wsdl",
            LogicalPath = "schemas/main.wsdl",
            Content = File.OpenRead("schemas/main.wsdl"),
            IsMain = true,
            DocumentKind = ESchemaDocumentKind.Wsdl,
        },
        new BatchSourceRequestModel
        {
            LogicalName = "types.xsd",
            LogicalPath = "schemas/types.xsd",
            Content = File.OpenRead("schemas/types.xsd"),
            DocumentKind = ESchemaDocumentKind.Xsd,
        },
    },
};

var graph = await parser.ParseBatchAsync(request, cancellationToken);
```

## Handle Failures

```csharp
try
{
    var graph = await parser.ParseFileAsync(request, cancellationToken);
}
catch (ParseFailureException ex)
{
    foreach (var diagnostic in ex.Diagnostics)
    {
        Console.WriteLine($"{diagnostic.Stage}: {diagnostic.Code} - {diagnostic.Message}");
    }
}
```

## Planned Internal Flow

1. The orchestration service validates the request model and required `DocumentKind`.
2. `ISourceLoader` normalizes the request into one or more `SourceDescriptorModel` entries.
3. `WsdlDiscoveryService` expands WSDL imports and schema references before graph construction.
4. WSDL inline schemas and XSD documents route discovered items to category-specific handlers.
4. Registry services canonicalize entries and build `MetadataGraphModel`.
5. Successful requests return the metadata graph; invalid requests throw `ParseFailureException`.

## Notes

- The orchestration service is the primary consumer-facing API for this feature.
- Existing `IWsdlParser` and `IXsdParser` services remain lower-level seams during the transition.
- This feature plan focuses on the main parsing logic and documentation, not on adding new test logic.
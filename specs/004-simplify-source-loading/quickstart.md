# Quickstart: Simplify Source Loading Inputs

## Purpose

Validate the planned contract reduction by walking through the retained file and string workflows only.

## 1. Register The Library

```csharp
var services = new ServiceCollection();
services.AddXsdXmlParser();

using var serviceProvider = services.BuildServiceProvider();
var parser = serviceProvider.GetRequiredService<IParserOrchestrationService>();
```

## 2. Parse From A File

```csharp
var fileGraph = await parser.ParseFileAsync(
    new FilePathParseRequestModel
    {
        FilePath = "schemas/service.wsdl",
        DisplayName = "service.wsdl",
        LogicalPath = "schemas/service.wsdl",
        DocumentKind = ESchemaDocumentKind.Wsdl,
    },
    cancellationToken);
```

Expected result:

- The request is accepted only when the file exists.
- `DocumentKind` is explicitly declared by the caller.
- The existing parsing pipeline receives a normalized file-backed source descriptor.
- Run this scenario with both representative WSDL and representative XSD file inputs during implementation validation.

## 3. Parse From A String

```csharp
const string schemaText =
        "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">" +
        "<xs:element name=\"customer\" type=\"xs:string\" />" +
        "</xs:schema>";

var stringGraph = await parser.ParseStringAsync(
    new StringParseRequestModel
    {
        Content = schemaText,
        DisplayName = "customer.xsd",
        LogicalPath = "schemas/customer.xsd",
        DocumentKind = ESchemaDocumentKind.Xsd,
    },
    cancellationToken);
```

Expected result:

- The request is accepted only when `Content` is non-empty.
- `LogicalPath` is required and acts as the base for any relative imports/includes.
- The existing parsing pipeline receives a normalized string-backed source descriptor.
- Run this scenario with both representative WSDL and representative XSD string inputs during implementation validation.

## 4. Verify Removed Workflows Are Gone

Review the public contract after implementation and confirm these workflows are no longer present:

- Stream-based parsing
- Memory-buffer parsing
- Batch parsing

## 5. Verify Failure Behavior

```csharp
try
{
    await parser.ParseStringAsync(
        new StringParseRequestModel
        {
            Content = string.Empty,
            LogicalPath = "schemas/invalid.xsd",
            DocumentKind = ESchemaDocumentKind.Xsd,
        },
        cancellationToken);
}
catch (ParseFailureException ex)
{
    foreach (var diagnostic in ex.Diagnostics)
    {
        Console.WriteLine($"{diagnostic.Stage}: {diagnostic.Message}");
    }
}
```

Expected result:

- Validation failures occur before parsing begins.
- Diagnostics identify the failing stage and offending input details.

Also validate:

- A missing file path or unresolved file-backed request fails before parsing begins.
- A string-backed request with missing logical path fails before parsing begins.
- A string-backed request with an unresolved related file fails with source-loading or resolution diagnostics.

## 6. Compare File And String Readiness

Use equivalent WSDL or XSD content once from disk and once from string input, then confirm both retained workflows reach the same downstream parsing readiness.

## 7. Build Validation

Run the standard multi-target build validation after implementation work begins:

```bash
dotnet build
```

Success means:

- The narrowed public contract compiles for `net6.0`, `net7.0`, and `net8.0`.
- XML documentation updates and comment changes stay consistent with the modified spec.
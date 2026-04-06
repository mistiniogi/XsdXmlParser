# Public API Contracts

## IWSDLParser

```csharp
public interface IWSDLParser
{
    Task<SchemaModel> ParseAsync(string wsdlContent, CancellationToken cancellationToken = default);
    Task<bool> ValidateAsync(string wsdlContent, CancellationToken cancellationToken = default);
}
```

**Contract**:
- Input: Valid WSDL XML string
- Output: Parsed SchemaModel or validation result
- Exceptions: InvalidWsdlException for malformed input
- Threading: Async operations

## IXSDParser

```csharp
public interface IXSDParser
{
    Task<SchemaModel> ParseAsync(string xsdContent, CancellationToken cancellationToken = default);
    Task<bool> ValidateAsync(string xsdContent, CancellationToken cancellationToken = default);
}
```

**Contract**:
- Input: Valid XSD XML string
- Output: Parsed SchemaModel or validation result
- Exceptions: InvalidXsdException for malformed input
- Threading: Async operations

## IJSONGenerator

```csharp
public interface IJSONGenerator
{
    Task<string> GenerateAsync(SchemaModel schema, CancellationToken cancellationToken = default);
    Task<JSONMetadata> GenerateMetadataAsync(SchemaModel schema, CancellationToken cancellationToken = default);
}
```

**Contract**:
- Input: Valid SchemaModel
- Output: JSON string or JSONMetadata object
- Format: Standard JSON with schema metadata
- Threading: Async operations

## IXMLGenerator

```csharp
public interface IXMLGenerator
{
    Task<string> GenerateAsync(SchemaModel schema, object data, CancellationToken cancellationToken = default);
    Task<bool> ValidateInstanceAsync(string xmlContent, SchemaModel schema, CancellationToken cancellationToken = default);
}
```

**Contract**:
- Input: SchemaModel and data object
- Output: Valid XML string or validation result
- Exceptions: InvalidDataException for data not matching schema
- Threading: Async operations
using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents the normalized metadata graph returned by the parser.
/// </summary>
public sealed class MetadataGraphModel
{
    /// <summary>
    /// Gets the source dictionary keyed by source identifier.
    /// </summary>
    public Dictionary<string, SourceDescriptorModel> Sources { get; } = new();

    /// <summary>
    /// Gets the complex type dictionary keyed by reference identifier.
    /// </summary>
    public Dictionary<string, ComplexTypeModel> ComplexTypes { get; } = new();

    /// <summary>
    /// Gets the simple type dictionary keyed by reference identifier.
    /// </summary>
    public Dictionary<string, SimpleTypeModel> SimpleTypes { get; } = new();

    /// <summary>
    /// Gets the element dictionary keyed by reference identifier.
    /// </summary>
    public Dictionary<string, ElementModel> Elements { get; } = new();

    /// <summary>
    /// Gets the attribute dictionary keyed by reference identifier.
    /// </summary>
    public Dictionary<string, AttributeModel> Attributes { get; } = new();

    /// <summary>
    /// Gets the relationship dictionary keyed by relationship identifier.
    /// </summary>
    public Dictionary<string, RelationshipModel> Relationships { get; } = new();

    /// <summary>
    /// Gets the validation rule dictionary keyed by rule identifier.
    /// </summary>
    public Dictionary<string, ConstraintSetModel> ValidationRules { get; } = new();

    /// <summary>
    /// Gets the exported root reference identifiers.
    /// </summary>
    public List<string> RootRefIds { get; } = new();

    /// <summary>
    /// Gets the serializer hints for downstream consumers.
    /// </summary>
    public Dictionary<string, string> SerializerHints { get; } = new();
}
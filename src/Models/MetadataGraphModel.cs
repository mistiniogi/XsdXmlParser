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
    /// <value>The normalized sources that participated in graph construction, keyed by canonical source identifier.</value>
    public Dictionary<string, SourceDescriptorModel> Sources { get; } = new();

    /// <summary>
    /// Gets the complex type dictionary keyed by reference identifier.
    /// </summary>
    /// <value>The canonical complex type entries keyed by reference identifier.</value>
    public Dictionary<string, ComplexTypeModel> ComplexTypes { get; } = new();

    /// <summary>
    /// Gets the simple type dictionary keyed by reference identifier.
    /// </summary>
    /// <value>The canonical simple type entries keyed by reference identifier.</value>
    public Dictionary<string, SimpleTypeModel> SimpleTypes { get; } = new();

    /// <summary>
    /// Gets the element dictionary keyed by reference identifier.
    /// </summary>
    /// <value>The canonical element entries keyed by reference identifier.</value>
    public Dictionary<string, ElementModel> Elements { get; } = new();

    /// <summary>
    /// Gets the attribute dictionary keyed by reference identifier.
    /// </summary>
    /// <value>The canonical attribute entries keyed by reference identifier.</value>
    public Dictionary<string, AttributeModel> Attributes { get; } = new();

    /// <summary>
    /// Gets the relationship dictionary keyed by relationship identifier.
    /// </summary>
    /// <value>The normalized relationships that connect sources, types, elements, and attributes.</value>
    public Dictionary<string, RelationshipModel> Relationships { get; } = new();

    /// <summary>
    /// Gets the validation rule dictionary keyed by rule identifier.
    /// </summary>
    /// <value>The normalized validation and constraint entries keyed by rule identifier.</value>
    public Dictionary<string, ConstraintSetModel> ValidationRules { get; } = new();

    /// <summary>
    /// Gets the exported root reference identifiers.
    /// </summary>
    /// <value>The reference identifiers that represent the primary exported roots in the graph.</value>
    public List<string> RootRefIds { get; } = new();

    /// <summary>
    /// Gets the serializer hints for downstream consumers.
    /// </summary>
    /// <value>The serializer hints used by downstream consumers when projecting the graph into other formats.</value>
    public Dictionary<string, string> SerializerHints { get; } = new();
}
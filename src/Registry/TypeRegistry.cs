using System.Collections.Generic;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Registry;

/// <summary>
/// Stores canonical graph dictionaries for exported parser metadata.
/// </summary>
public sealed class TypeRegistry
{
    /// <summary>
    /// Gets the complex type dictionary.
    /// </summary>
    public Dictionary<string, ComplexTypeModel> ComplexTypes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the simple type dictionary.
    /// </summary>
    public Dictionary<string, SimpleTypeModel> SimpleTypes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the element dictionary.
    /// </summary>
    public Dictionary<string, ElementModel> Elements { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the attribute dictionary.
    /// </summary>
    public Dictionary<string, AttributeModel> Attributes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the validation rule dictionary.
    /// </summary>
    public Dictionary<string, ConstraintSetModel> ValidationRules { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the relationship dictionary.
    /// </summary>
    public Dictionary<string, RelationshipModel> Relationships { get; } = new(StringComparer.Ordinal);
}
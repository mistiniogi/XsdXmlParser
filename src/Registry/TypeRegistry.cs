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
    /// <value>The canonical complex type dictionary keyed by reference identifier.</value>
    public Dictionary<string, ComplexTypeModel> ComplexTypes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the simple type dictionary.
    /// </summary>
    /// <value>The canonical simple type dictionary keyed by reference identifier.</value>
    public Dictionary<string, SimpleTypeModel> SimpleTypes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the element dictionary.
    /// </summary>
    /// <value>The canonical element dictionary keyed by reference identifier.</value>
    public Dictionary<string, ElementModel> Elements { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the attribute dictionary.
    /// </summary>
    /// <value>The canonical attribute dictionary keyed by reference identifier.</value>
    public Dictionary<string, AttributeModel> Attributes { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the validation rule dictionary.
    /// </summary>
    /// <value>The normalized validation rule dictionary keyed by rule identifier.</value>
    public Dictionary<string, ConstraintSetModel> ValidationRules { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the relationship dictionary.
    /// </summary>
    /// <value>The normalized relationship dictionary keyed by relationship identifier.</value>
    public Dictionary<string, RelationshipModel> Relationships { get; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Stores a canonical complex type.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(ComplexTypeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ComplexTypes[model.RefId] = model;
    }

    /// <summary>
    /// Stores a canonical simple type.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(SimpleTypeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        SimpleTypes[model.RefId] = model;
    }

    /// <summary>
    /// Stores a canonical element.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(ElementModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Elements[model.RefId] = model;
    }

    /// <summary>
    /// Stores a canonical attribute.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(AttributeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Attributes[model.RefId] = model;
    }

    /// <summary>
    /// Stores a normalized validation rule.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(ConstraintSetModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        ValidationRules[model.RuleId] = model;
    }

    /// <summary>
    /// Stores a normalized relationship.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void Store(RelationshipModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        Relationships[model.RelationshipId] = model;
    }

    /// <summary>
    /// Attempts to retrieve a complex type by reference identifier.
    /// </summary>
    /// <param name="refId">The reference identifier to inspect.</param>
    /// <param name="model">The matching model when present.</param>
    /// <returns><see langword="true"/> when the model is present.</returns>
    public bool TryGetComplexType(string refId, out ComplexTypeModel model)
    {
        return ComplexTypes.TryGetValue(refId, out model!);
    }
}
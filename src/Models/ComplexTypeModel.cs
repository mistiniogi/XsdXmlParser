using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical complex type definition in the normalized metadata graph.
/// </summary>
public sealed class ComplexTypeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the base type reference identifier.
    /// </summary>
    /// <value>The canonical base type reference identifier when the complex type extends or restricts another type.</value>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the child member reference identifiers in order.
    /// </summary>
    /// <value>The ordered child member reference identifiers that define the complex type content model.</value>
    public List<string> ChildMemberRefIds { get; } = new();

    /// <summary>
    /// Gets the attached attribute reference identifiers.
    /// </summary>
    /// <value>The attribute reference identifiers associated with the complex type.</value>
    public List<string> AttributeRefIds { get; } = new();

    /// <summary>
    /// Gets the flattened compositor hints for UI and validation consumers.
    /// </summary>
    /// <value>The compositor hints that help downstream consumers interpret sequence, choice, or related structural metadata.</value>
    public Dictionary<string, string> CompositorHints { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the type is anonymous.
    /// </summary>
    /// <value><see langword="true"/> when the complex type originated as an anonymous inline definition.</value>
    public bool IsAnonymous { get; set; }
}
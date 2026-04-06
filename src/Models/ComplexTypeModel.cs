using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical complex type definition.
/// </summary>
public sealed class ComplexTypeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the base type reference identifier.
    /// </summary>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the child member reference identifiers in order.
    /// </summary>
    public List<string> ChildMemberRefIds { get; } = new();

    /// <summary>
    /// Gets the attached attribute reference identifiers.
    /// </summary>
    public List<string> AttributeRefIds { get; } = new();

    /// <summary>
    /// Gets the flattened compositor hints for UI and validation consumers.
    /// </summary>
    public Dictionary<string, string> CompositorHints { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the type is anonymous.
    /// </summary>
    public bool IsAnonymous { get; set; }
}
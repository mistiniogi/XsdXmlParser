using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical simple type definition in the normalized metadata graph.
/// </summary>
public sealed class SimpleTypeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the base type reference identifier.
    /// </summary>
    /// <value>The canonical base type reference identifier when the simple type derives from another type.</value>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the restriction rule identifiers.
    /// </summary>
    /// <value>The normalized restriction rule identifiers applied to the simple type.</value>
    public List<string> RestrictionRuleIds { get; } = new();

    /// <summary>
    /// Gets the explicit enumeration values.
    /// </summary>
    /// <value>The explicit enumeration values declared for the simple type.</value>
    public List<string> EnumerationValues { get; } = new();
}
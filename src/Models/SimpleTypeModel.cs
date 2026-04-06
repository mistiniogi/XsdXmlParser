using System.Collections.Generic;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical simple type definition.
/// </summary>
public sealed class SimpleTypeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the base type reference identifier.
    /// </summary>
    public string? BaseTypeRefId { get; set; }

    /// <summary>
    /// Gets the restriction rule identifiers.
    /// </summary>
    public List<string> RestrictionRuleIds { get; } = new();

    /// <summary>
    /// Gets the explicit enumeration values.
    /// </summary>
    public List<string> EnumerationValues { get; } = new();
}
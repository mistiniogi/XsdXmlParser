namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical attribute definition in the normalized metadata graph.
/// </summary>
public sealed class AttributeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the referenced type identifier.
    /// </summary>
    /// <value>The canonical type reference identifier that describes the attribute value.</value>
    public string TypeRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the usage kind.
    /// </summary>
    /// <value>The usage classification, such as required or optional, derived from the source schema.</value>
    public string UseKind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional default value.
    /// </summary>
    /// <value>The default value declared for the attribute, when one is available.</value>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the optional fixed value.
    /// </summary>
    /// <value>The fixed value declared for the attribute, when one is available.</value>
    public string? FixedValue { get; set; }
}
namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical attribute definition.
/// </summary>
public sealed class AttributeModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the referenced type identifier.
    /// </summary>
    public string TypeRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the usage kind.
    /// </summary>
    public string UseKind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional default value.
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the optional fixed value.
    /// </summary>
    public string? FixedValue { get; set; }
}
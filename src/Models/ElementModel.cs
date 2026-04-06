namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical element definition.
/// </summary>
public sealed class ElementModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the referenced type identifier.
    /// </summary>
    public string TypeRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum occurrence.
    /// </summary>
    public int MinOccurs { get; set; }

    /// <summary>
    /// Gets or sets the maximum occurrence marker.
    /// </summary>
    public string MaxOccurs { get; set; } = "1";

    /// <summary>
    /// Gets or sets the order index.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Gets or sets the optional choice-group key.
    /// </summary>
    public string? ChoiceGroupKey { get; set; }

    /// <summary>
    /// Gets or sets the optional compositor kind used by flattened metadata.
    /// </summary>
    public string? CompositorKind { get; set; }
}
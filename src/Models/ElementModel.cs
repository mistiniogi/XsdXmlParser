namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a canonical element definition in the normalized metadata graph.
/// </summary>
public sealed class ElementModel : RegistryEntryModel
{
    /// <summary>
    /// Gets or sets the referenced type identifier.
    /// </summary>
    /// <value>The canonical type reference identifier that describes the element value.</value>
    public string TypeRefId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum occurrence.
    /// </summary>
    /// <value>The minimum occurrence constraint for the element within its owning scope.</value>
    public int MinOccurs { get; set; }

    /// <summary>
    /// Gets or sets the maximum occurrence marker.
    /// </summary>
    /// <value>The maximum occurrence marker, which may be a numeric value or <c>unbounded</c>.</value>
    public string MaxOccurs { get; set; } = "1";

    /// <summary>
    /// Gets or sets the order index.
    /// </summary>
    /// <value>The ordinal position used when the element participates in an ordered content model.</value>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Gets or sets the optional choice-group key.
    /// </summary>
    /// <value>The choice-group key used when the element participates in a flattened choice structure.</value>
    public string? ChoiceGroupKey { get; set; }

    /// <summary>
    /// Gets or sets the optional compositor kind used by flattened metadata.
    /// </summary>
    /// <value>The compositor kind used when downstream consumers need flattened sequence or choice hints.</value>
    public string? CompositorKind { get; set; }
}
using System.Collections.Generic;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Registry;

/// <summary>
/// Owns canonical registration and duplicate analysis for schema definitions.
/// </summary>
public sealed class SchemaRegistryService
{
    private readonly Dictionary<string, SchemaRegistryEntryModel> entries = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaRegistryService"/> class.
    /// </summary>
    /// <param name="typeRegistry">The canonical type registry used for exported dictionaries.</param>
    public SchemaRegistryService(TypeRegistry typeRegistry)
    {
        TypeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
    }

    /// <summary>
    /// Gets the canonical type registry used for exported graph content.
    /// </summary>
    public TypeRegistry TypeRegistry { get; }

    /// <summary>
    /// Registers lifecycle metadata for a canonical entry.
    /// </summary>
    /// <param name="entry">The entry to register.</param>
    public void RegisterEntry(SchemaRegistryEntryModel entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        entries[entry.RefId] = entry;
    }

    /// <summary>
    /// Attempts to retrieve lifecycle metadata for a canonical entry.
    /// </summary>
    /// <param name="refId">The reference identifier to inspect.</param>
    /// <param name="entry">The matching entry when present.</param>
    /// <returns><see langword="true"/> when the entry is present.</returns>
    public bool TryGetEntry(string refId, out SchemaRegistryEntryModel entry)
    {
        return entries.TryGetValue(refId, out entry!);
    }
}
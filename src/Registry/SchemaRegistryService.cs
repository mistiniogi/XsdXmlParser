using System.Collections.Generic;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Parsing;

namespace XsdXmlParser.Core.Registry;

/// <summary>
/// Owns canonical registration and duplicate analysis for schema definitions.
/// </summary>
public sealed class SchemaRegistryService
{
    /// <summary>
    /// The lifecycle entries keyed by canonical reference identifier.
    /// </summary>
    private readonly Dictionary<string, SchemaRegistryEntryModel> entries = new(StringComparer.Ordinal);

    /// <summary>
    /// The canonical reference identifiers keyed by logical entry identity.
    /// </summary>
    private readonly Dictionary<string, string> canonicalRefIds = new(StringComparer.Ordinal);

    /// <summary>
    /// The deterministic reference identifier factory used for named and anonymous entries.
    /// </summary>
    private readonly RefIdFactory refIdFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaRegistryService"/> class.
    /// </summary>
    /// <param name="typeRegistry">The canonical type registry used for exported dictionaries.</param>
    public SchemaRegistryService(TypeRegistry typeRegistry)
        : this(typeRegistry, new RefIdFactory())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaRegistryService"/> class.
    /// </summary>
    /// <param name="typeRegistry">The canonical type registry used for exported dictionaries.</param>
    /// <param name="refIdFactory">The deterministic reference identifier factory.</param>
    public SchemaRegistryService(TypeRegistry typeRegistry, RefIdFactory refIdFactory)
    {
        TypeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
        this.refIdFactory = refIdFactory ?? throw new ArgumentNullException(nameof(refIdFactory));
    }

    /// <summary>
    /// Gets the canonical type registry used for exported graph content.
    /// </summary>
    /// <value>The canonical type registry used to surface exported graph content.</value>
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

    /// <summary>
    /// Creates or retrieves a canonical type reference identifier.
    /// </summary>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <param name="qualifiedName">The qualified type name.</param>
    /// <param name="schemaPath">The canonical schema path.</param>
    /// <returns>The canonical type reference identifier.</returns>
    public string GetOrCreateTypeRefId(string sourceId, string qualifiedName, string schemaPath)
    {
        return GetOrCreateNamedRefId("type", sourceId, qualifiedName, schemaPath);
    }

    /// <summary>
    /// Creates or retrieves a canonical element reference identifier.
    /// </summary>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <param name="qualifiedName">The qualified element name.</param>
    /// <param name="schemaPath">The canonical schema path.</param>
    /// <returns>The canonical element reference identifier.</returns>
    public string GetOrCreateElementRefId(string sourceId, string qualifiedName, string schemaPath)
    {
        return GetOrCreateNamedRefId("element", sourceId, qualifiedName, schemaPath);
    }

    /// <summary>
    /// Creates or retrieves a canonical attribute reference identifier.
    /// </summary>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <param name="qualifiedName">The qualified attribute name.</param>
    /// <param name="schemaPath">The canonical schema path.</param>
    /// <returns>The canonical attribute reference identifier.</returns>
    public string GetOrCreateAttributeRefId(string sourceId, string qualifiedName, string schemaPath)
    {
        return GetOrCreateNamedRefId("attribute", sourceId, qualifiedName, schemaPath);
    }

    /// <summary>
    /// Creates or retrieves a deterministic anonymous type reference identifier.
    /// </summary>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <param name="parentRefId">The parent reference identifier.</param>
    /// <param name="schemaPath">The canonical schema path.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <returns>The anonymous type reference identifier.</returns>
    public string CreateAnonymousTypeRefId(string sourceId, string parentRefId, string schemaPath, int localOrdinal)
    {
        var refId = refIdFactory.CreateAnonymousTypeRefId(sourceId, parentRefId, schemaPath, localOrdinal);
        if (!entries.ContainsKey(refId))
        {
            RegisterEntry(new SchemaRegistryEntryModel
            {
                DiscoveryState = "anonymous-shell",
                EntryKind = "type",
                OwningSourceId = sourceId,
                RefId = refId,
            });
        }

        return refId;
    }

    /// <summary>
    /// Stores a canonical complex type.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreComplexType(ComplexTypeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
        RegisterEntry(CreateEntry(model.RefId, "complexType", model.SourceId));
    }

    /// <summary>
    /// Stores a canonical simple type.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreSimpleType(SimpleTypeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
        RegisterEntry(CreateEntry(model.RefId, "simpleType", model.SourceId));
    }

    /// <summary>
    /// Stores a canonical element.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreElement(ElementModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
        RegisterEntry(CreateEntry(model.RefId, "element", model.SourceId));
    }

    /// <summary>
    /// Stores a canonical attribute.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreAttribute(AttributeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
        RegisterEntry(CreateEntry(model.RefId, "attribute", model.SourceId));
    }

    /// <summary>
    /// Stores a normalized validation rule.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreConstraintSet(ConstraintSetModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
    }

    /// <summary>
    /// Stores a normalized relationship.
    /// </summary>
    /// <param name="model">The model to store.</param>
    public void StoreRelationship(RelationshipModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        TypeRegistry.Store(model);
    }

    /// <summary>
    /// Creates lifecycle metadata for a discovered canonical entry.
    /// </summary>
    /// <param name="refId">The canonical reference identifier.</param>
    /// <param name="entryKind">The logical entry kind.</param>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <returns>The registry lifecycle entry.</returns>
    private static SchemaRegistryEntryModel CreateEntry(string refId, string entryKind, string sourceId)
    {
        return new SchemaRegistryEntryModel
        {
            DiscoveryState = "discovered",
            EntryKind = entryKind,
            OwningSourceId = sourceId,
            RefId = refId,
        };
    }

    /// <summary>
    /// Throws when a required value is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name used in any thrown exception.</param>
    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }

    /// <summary>
    /// Creates or retrieves a canonical reference identifier for a named schema entry.
    /// </summary>
    /// <param name="kind">The logical entry kind.</param>
    /// <param name="sourceId">The owning source identifier.</param>
    /// <param name="qualifiedName">The qualified schema name.</param>
    /// <param name="schemaPath">The canonical schema path used for conflict tracking.</param>
    /// <returns>The canonical reference identifier.</returns>
    private string GetOrCreateNamedRefId(string kind, string sourceId, string qualifiedName, string schemaPath)
    {
        ThrowIfNullOrWhiteSpace(sourceId, nameof(sourceId));
        ThrowIfNullOrWhiteSpace(qualifiedName, nameof(qualifiedName));
        ThrowIfNullOrWhiteSpace(schemaPath, nameof(schemaPath));

        var canonicalKey = string.Concat(kind, "|", qualifiedName);
        if (canonicalRefIds.TryGetValue(canonicalKey, out var existingRefId))
        {
            return existingRefId;
        }

        var refId = string.Concat(kind, ":", SchemaParsingHelper.SanitizeIdentifier(qualifiedName));
        canonicalRefIds[canonicalKey] = refId;
        RegisterEntry(new SchemaRegistryEntryModel
        {
            ConflictHash = schemaPath,
            DiscoveryState = "shell",
            EntryKind = kind,
            OwningSourceId = sourceId,
            RefId = refId,
        });

        return refId;
    }
}
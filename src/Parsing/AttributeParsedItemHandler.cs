using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Handles XSD attribute items and projects them into canonical attribute entries.
/// </summary>
internal sealed class AttributeParsedItemHandler : IParsedItemHandler
{
    /// <inheritdoc/>
    public bool CanHandle(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return string.Equals(item.Name.LocalName, "attribute", StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public async Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        var declaredName = (string?)item.Attribute("name");
        var referencedName = (string?)item.Attribute("ref");
        var effectiveName = !string.IsNullOrWhiteSpace(referencedName)
            ? referencedName
            : declaredName ?? string.Concat("anonymous-attribute-", localOrdinal.ToString(CultureInfo.InvariantCulture));
        var qualifiedName = SchemaParsingHelper.ResolveQualifiedName(item, effectiveName, context.TargetNamespace);
        var schemaPath = SchemaParsingHelper.BuildSchemaPath(item);
        var refId = context.SchemaRegistryService.GetOrCreateAttributeRefId(context.Source.SourceId, qualifiedName, schemaPath);

        var inlineType = item.Elements().FirstOrDefault(candidate => string.Equals(candidate.Name.LocalName, "simpleType", StringComparison.Ordinal));
        var explicitType = (string?)item.Attribute("type");
        var typeRefId = string.Empty;
        if (inlineType is not null)
        {
            typeRefId = await context.HandleNestedItemAsync(inlineType, refId, 0, cancellationToken).ConfigureAwait(false) ?? string.Empty;
        }
        else if (!string.IsNullOrWhiteSpace(explicitType))
        {
            typeRefId = context.SchemaRegistryService.GetOrCreateTypeRefId(
                context.Source.SourceId,
                SchemaParsingHelper.ResolveQualifiedName(item, explicitType, context.TargetNamespace),
                string.Concat(schemaPath, "/@type"));
        }

        var model = new AttributeModel
        {
            DefaultValue = (string?)item.Attribute("default"),
            Documentation = SchemaParsingHelper.TryGetDocumentation(item),
            FixedValue = (string?)item.Attribute("fixed"),
            ParentRefId = parentRefId,
            QualifiedName = qualifiedName,
            RefId = refId,
            SchemaPath = schemaPath,
            SourceId = context.Source.SourceId,
            TypeRefId = typeRefId,
            UseKind = (string?)item.Attribute("use") ?? "optional",
        };

        context.SchemaRegistryService.StoreAttribute(model);

        if (!string.IsNullOrWhiteSpace(parentRefId))
        {
            context.SchemaRegistryService.StoreRelationship(new RelationshipModel
            {
                FromRefId = parentRefId,
                OrderIndex = localOrdinal,
                PassAssigned = "build",
                RelationshipId = SchemaParsingHelper.CreateRelationshipId(parentRefId, ERelationshipKind.AttributeOf.ToString(), refId),
                RelationshipKind = ERelationshipKind.AttributeOf,
                ToRefId = refId,
            });

            if (context.SchemaRegistryService.TypeRegistry.TryGetComplexType(parentRefId, out var parentComplexType) && !parentComplexType.AttributeRefIds.Contains(refId, StringComparer.Ordinal))
            {
                parentComplexType.AttributeRefIds.Add(refId);
            }
        }

        if (!string.IsNullOrWhiteSpace(typeRefId))
        {
            context.SchemaRegistryService.StoreRelationship(new RelationshipModel
            {
                FromRefId = refId,
                PassAssigned = "build",
                RelationshipId = SchemaParsingHelper.CreateRelationshipId(refId, "references", typeRefId),
                RelationshipKind = ERelationshipKind.References,
                ToRefId = typeRefId,
            });
        }

        return refId;
    }
}
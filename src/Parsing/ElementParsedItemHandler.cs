using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Handles XSD element items and projects them into canonical element, relationship, and occurrence metadata.
/// </summary>
internal sealed class ElementParsedItemHandler : IParsedItemHandler
{
    /// <inheritdoc/>
    public bool CanHandle(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return string.Equals(item.Name.LocalName, "element", StringComparison.Ordinal);
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
            : declaredName ?? string.Concat("anonymous-element-", localOrdinal.ToString(CultureInfo.InvariantCulture));
        var qualifiedName = SchemaParsingHelper.ResolveQualifiedName(item, effectiveName, context.TargetNamespace);
        var schemaPath = SchemaParsingHelper.BuildSchemaPath(item);
        var refId = context.SchemaRegistryService.GetOrCreateElementRefId(context.Source.SourceId, qualifiedName, schemaPath);

        var inlineType = item.Elements().FirstOrDefault(candidate => string.Equals(candidate.Name.LocalName, "complexType", StringComparison.Ordinal) || string.Equals(candidate.Name.LocalName, "simpleType", StringComparison.Ordinal));
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

        var model = new ElementModel
        {
            ChoiceGroupKey = item.Parent is null || !string.Equals(item.Parent.Name.LocalName, "choice", StringComparison.Ordinal)
                ? null
                : SchemaParsingHelper.BuildSchemaPath(item.Parent),
            CompositorKind = item.Parent?.Name.LocalName,
            Documentation = SchemaParsingHelper.TryGetDocumentation(item),
            MaxOccurs = (string?)item.Attribute("maxOccurs") ?? "1",
            MinOccurs = ParseInt((string?)item.Attribute("minOccurs"), 1),
            OrderIndex = localOrdinal,
            ParentRefId = parentRefId,
            QualifiedName = qualifiedName,
            RefId = refId,
            SchemaPath = schemaPath,
            SourceId = context.Source.SourceId,
            TypeRefId = typeRefId,
        };

        context.SchemaRegistryService.StoreElement(model);
        AddParentRelationship(context, parentRefId, refId, localOrdinal, ERelationshipKind.Contains);

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

        if (model.MinOccurs != 1 || !string.Equals(model.MaxOccurs, "1", StringComparison.Ordinal))
        {
            context.SchemaRegistryService.StoreConstraintSet(new ConstraintSetModel
            {
                MaxOccurs = model.MaxOccurs,
                MinOccurs = model.MinOccurs,
                OwnerRefId = refId,
                RuleId = SchemaParsingHelper.CreateRuleId(refId, "occurs"),
            });
        }

        if (!string.IsNullOrWhiteSpace(parentRefId)
            && context.SchemaRegistryService.TypeRegistry.TryGetComplexType(parentRefId, out var parentComplexType)
            && !parentComplexType.ChildMemberRefIds.Contains(refId, StringComparer.Ordinal))
        {
            parentComplexType.ChildMemberRefIds.Add(refId);
        }

        return refId;
    }

    /// <summary>
    /// Adds a parent-child relationship when the current element is nested inside another registered entry.
    /// </summary>
    /// <param name="context">The current parsing context.</param>
    /// <param name="parentRefId">The optional parent reference identifier.</param>
    /// <param name="childRefId">The child reference identifier.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <param name="relationshipKind">The relationship kind to store.</param>
    private static void AddParentRelationship(ParsedItemContext context, string? parentRefId, string childRefId, int localOrdinal, ERelationshipKind relationshipKind)
    {
        if (string.IsNullOrWhiteSpace(parentRefId))
        {
            return;
        }

        context.SchemaRegistryService.StoreRelationship(new RelationshipModel
        {
            FromRefId = parentRefId,
            OrderIndex = localOrdinal,
            PassAssigned = "build",
            RelationshipId = SchemaParsingHelper.CreateRelationshipId(parentRefId, relationshipKind.ToString(), childRefId),
            RelationshipKind = relationshipKind,
            ToRefId = childRefId,
        });
    }

    /// <summary>
    /// Parses an occurrence value while falling back to the supplied default.
    /// </summary>
    /// <param name="value">The raw occurrence value.</param>
    /// <param name="defaultValue">The default value to use when parsing fails.</param>
    /// <returns>The parsed occurrence value or the supplied default.</returns>
    private static int ParseInt(string? value, int defaultValue)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue)
            ? parsedValue
            : defaultValue;
    }
}
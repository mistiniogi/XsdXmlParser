using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Handles complex type schema items.
/// </summary>
internal sealed class ComplexTypeParsedItemHandler : IParsedItemHandler
{
    /// <inheritdoc/>
    public bool CanHandle(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return string.Equals(item.Name.LocalName, "complexType", StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public async Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        var declaredName = (string?)item.Attribute("name");
        var schemaPath = SchemaParsingHelper.BuildSchemaPath(item);
        var isAnonymous = string.IsNullOrWhiteSpace(declaredName);
        var qualifiedName = isAnonymous ? null : SchemaParsingHelper.ResolveQualifiedName(item, declaredName!, context.TargetNamespace);
        var refId = isAnonymous
            ? context.SchemaRegistryService.CreateAnonymousTypeRefId(context.Source.SourceId, parentRefId ?? context.Source.SourceId, schemaPath, localOrdinal)
            : context.SchemaRegistryService.GetOrCreateTypeRefId(context.Source.SourceId, qualifiedName!, schemaPath);

        var model = new ComplexTypeModel
        {
            Documentation = SchemaParsingHelper.TryGetDocumentation(item),
            IsAnonymous = isAnonymous,
            ParentRefId = parentRefId,
            QualifiedName = qualifiedName,
            RefId = refId,
            SchemaPath = schemaPath,
            SourceId = context.Source.SourceId,
        };

        var extensionOrRestriction = item.Descendants().FirstOrDefault(descendant => string.Equals(descendant.Name.LocalName, "extension", StringComparison.Ordinal) || string.Equals(descendant.Name.LocalName, "restriction", StringComparison.Ordinal));
        var baseTypeValue = (string?)extensionOrRestriction?.Attribute("base");
        if (!string.IsNullOrWhiteSpace(baseTypeValue))
        {
            model.BaseTypeRefId = context.SchemaRegistryService.GetOrCreateTypeRefId(
                context.Source.SourceId,
                SchemaParsingHelper.ResolveQualifiedName(extensionOrRestriction!, baseTypeValue!, context.TargetNamespace),
                string.Concat(schemaPath, "/@base"));

            context.SchemaRegistryService.StoreRelationship(new RelationshipModel
            {
                FromRefId = refId,
                PassAssigned = "build",
                RelationshipId = SchemaParsingHelper.CreateRelationshipId(refId, "derives", model.BaseTypeRefId),
                RelationshipKind = ERelationshipKind.DerivesFrom,
                ToRefId = model.BaseTypeRefId,
            });
        }

        context.SchemaRegistryService.StoreComplexType(model);

        var memberOrdinal = 0;
        foreach (var member in SchemaParsingHelper.EnumerateContainedMembers(item))
        {
            var memberRefId = await context.HandleNestedItemAsync(member, refId, memberOrdinal, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(memberRefId))
            {
                memberOrdinal++;
                continue;
            }

            if (string.Equals(member.Name.LocalName, "attribute", StringComparison.Ordinal))
            {
                if (!model.AttributeRefIds.Contains(memberRefId, StringComparer.Ordinal))
                {
                    model.AttributeRefIds.Add(memberRefId);
                }
            }
            else if (!model.ChildMemberRefIds.Contains(memberRefId, StringComparer.Ordinal))
            {
                model.ChildMemberRefIds.Add(memberRefId);
            }

            memberOrdinal++;
        }

        return refId;
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Handles XSD simple type items and projects them into canonical type and constraint metadata.
/// </summary>
internal sealed class SimpleTypeParsedItemHandler : IParsedItemHandler
{
    /// <inheritdoc/>
    public bool CanHandle(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return string.Equals(item.Name.LocalName, "simpleType", StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
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

        var model = new SimpleTypeModel
        {
            Documentation = SchemaParsingHelper.TryGetDocumentation(item),
            ParentRefId = parentRefId,
            QualifiedName = qualifiedName,
            RefId = refId,
            SchemaPath = schemaPath,
            SourceId = context.Source.SourceId,
        };

        var restriction = item.Descendants().FirstOrDefault(descendant => string.Equals(descendant.Name.LocalName, "restriction", StringComparison.Ordinal));
        var baseTypeValue = (string?)restriction?.Attribute("base");
        if (!string.IsNullOrWhiteSpace(baseTypeValue))
        {
            model.BaseTypeRefId = context.SchemaRegistryService.GetOrCreateTypeRefId(
                context.Source.SourceId,
                SchemaParsingHelper.ResolveQualifiedName(restriction!, baseTypeValue!, context.TargetNamespace),
                string.Concat(schemaPath, "/@base"));
        }

        foreach (var enumeration in item.Descendants().Where(descendant => string.Equals(descendant.Name.LocalName, "enumeration", StringComparison.Ordinal)))
        {
            var value = (string?)enumeration.Attribute("value");
            if (!string.IsNullOrWhiteSpace(value))
            {
                model.EnumerationValues.Add(value);
            }
        }

        context.SchemaRegistryService.StoreSimpleType(model);

        if (restriction is not null)
        {
            var rule = new ConstraintSetModel
            {
                BaseTypeRefId = model.BaseTypeRefId,
                OwnerRefId = refId,
                Pattern = restriction.Elements().FirstOrDefault(descendant => string.Equals(descendant.Name.LocalName, "pattern", StringComparison.Ordinal))?.Attribute("value")?.Value,
                RuleId = SchemaParsingHelper.CreateRuleId(refId, "restriction"),
            };

            foreach (var enumerationValue in model.EnumerationValues)
            {
                rule.EnumerationValues.Add(enumerationValue);
            }

            foreach (var boundName in new[] { "minInclusive", "maxInclusive", "minExclusive", "maxExclusive", "minLength", "maxLength", "length" })
            {
                var boundValue = restriction.Elements().FirstOrDefault(descendant => string.Equals(descendant.Name.LocalName, boundName, StringComparison.Ordinal))?.Attribute("value")?.Value;
                if (!string.IsNullOrWhiteSpace(boundValue))
                {
                    rule.ValueBounds[boundName] = boundValue;
                }
            }

            if (!string.IsNullOrWhiteSpace(rule.Pattern) || rule.EnumerationValues.Count > 0 || rule.ValueBounds.Count > 0 || !string.IsNullOrWhiteSpace(rule.BaseTypeRefId))
            {
                context.SchemaRegistryService.StoreConstraintSet(rule);
            }
        }

        return Task.FromResult<string?>(refId);
    }
}
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Handles WSDL service-level artifacts that do not map to dedicated registry entry models and records serializer hints for them.
/// </summary>
internal sealed class WsdlServiceParsedItemHandler : IParsedItemHandler
{
    /// <inheritdoc/>
    public bool CanHandle(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var localName = item.Name.LocalName;
        return string.Equals(localName, "binding", StringComparison.Ordinal)
            || string.Equals(localName, "message", StringComparison.Ordinal)
            || string.Equals(localName, "operation", StringComparison.Ordinal)
            || string.Equals(localName, "part", StringComparison.Ordinal)
            || string.Equals(localName, "port", StringComparison.Ordinal)
            || string.Equals(localName, "portType", StringComparison.Ordinal)
            || string.Equals(localName, "service", StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        var localName = item.Name.LocalName;
        var artifactName = (string?)item.Attribute("name") ?? (string?)item.Attribute("element") ?? (string?)item.Attribute("type") ?? string.Concat(localName, "-", localOrdinal.ToString(CultureInfo.InvariantCulture));
        var key = string.Concat("wsdl:", context.Source.SourceId, ":", localName, ":", SchemaParsingHelper.SanitizeIdentifier(artifactName));
        context.Graph.SerializerHints[key] = SchemaParsingHelper.BuildSchemaPath(item);

        var action = (string?)item.Attribute("Action") ?? (string?)item.Attribute(XName.Get("Action", "http://schemas.xmlsoap.org/wsdl/soap/"));
        if (!string.IsNullOrWhiteSpace(action))
        {
            context.Graph.SerializerHints[key + ":action"] = action;
        }

        return Task.FromResult<string?>(null);
    }
}
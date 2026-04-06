using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides shared XML parsing helpers for WSDL and XSD traversal.
/// </summary>
internal static class SchemaParsingHelper
{
    /// <summary>
    /// Builds a stable schema path for the supplied XML item.
    /// </summary>
    /// <param name="item">The XML item.</param>
    /// <returns>The stable schema path.</returns>
    public static string BuildSchemaPath(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var segments = item
            .AncestorsAndSelf()
            .Reverse()
            .Select(element => $"{element.Name.LocalName}[{GetOneBasedOrdinal(element)}]");

        return "/" + string.Join("/", segments);
    }

    /// <summary>
    /// Builds a stable synthetic identifier segment.
    /// </summary>
    /// <param name="value">The raw identifier value.</param>
    /// <returns>The sanitized segment.</returns>
    public static string SanitizeIdentifier(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            var sanitizedCharacter = char.IsLetterOrDigit(character) || character == ':' || character == '-' || character == '_' || character == '.'
                ? character
                : '-';
            _ = builder.Append(sanitizedCharacter);
        }

        return builder.ToString().Trim('-');
    }

    /// <summary>
    /// Resolves a raw QName string into a stable namespace-qualified representation.
    /// </summary>
    /// <param name="scope">The XML scope used for prefix resolution.</param>
    /// <param name="rawName">The raw QName string.</param>
    /// <param name="targetNamespace">The default namespace to use for unqualified names.</param>
    /// <returns>The stable qualified name representation.</returns>
    public static string ResolveQualifiedName(XElement scope, string rawName, string targetNamespace)
    {
        ArgumentNullException.ThrowIfNull(scope);

        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        var separatorIndex = rawName.IndexOf(':');
        if (separatorIndex < 0)
        {
            return string.IsNullOrWhiteSpace(targetNamespace)
                ? rawName
                : string.Concat("{", targetNamespace, "}", rawName);
        }

        var prefix = rawName[..separatorIndex];
        var localName = rawName.AsSpan(separatorIndex + 1).ToString();
        var resolvedNamespace = scope.GetNamespaceOfPrefix(prefix)?.NamespaceName ?? string.Empty;
        return string.IsNullOrWhiteSpace(resolvedNamespace)
            ? localName
            : string.Concat("{", resolvedNamespace, "}", localName);
    }

    /// <summary>
    /// Extracts the first documentation text block from the supplied XML item.
    /// </summary>
    /// <param name="item">The XML item.</param>
    /// <returns>The documentation text when present.</returns>
    public static string? TryGetDocumentation(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return item
            .Descendants()
            .FirstOrDefault(descendant => string.Equals(descendant.Name.LocalName, "documentation", StringComparison.Ordinal))?
            .Value
            .Trim();
    }

    /// <summary>
    /// Enumerates member items contained within a complex type-like container.
    /// </summary>
    /// <param name="container">The container to enumerate.</param>
    /// <returns>The contained schema items.</returns>
    public static IEnumerable<XElement> EnumerateContainedMembers(XElement container)
    {
        ArgumentNullException.ThrowIfNull(container);
        return EnumerateContainedMembersIterator(container);
    }

    /// <summary>
    /// Creates a deterministic rule identifier.
    /// </summary>
    /// <param name="ownerRefId">The owning entry reference identifier.</param>
    /// <param name="suffix">The rule suffix.</param>
    /// <returns>The deterministic rule identifier.</returns>
    public static string CreateRuleId(string ownerRefId, string suffix)
    {
        return string.Concat("rule:", SanitizeIdentifier(ownerRefId), ":", SanitizeIdentifier(suffix));
    }

    /// <summary>
    /// Creates a deterministic relationship identifier.
    /// </summary>
    /// <param name="fromRefId">The source reference identifier.</param>
    /// <param name="kind">The relationship kind.</param>
    /// <param name="toRefId">The target reference identifier.</param>
    /// <returns>The deterministic relationship identifier.</returns>
    public static string CreateRelationshipId(string fromRefId, string kind, string toRefId)
    {
        return string.Concat("rel:", SanitizeIdentifier(fromRefId), ":", SanitizeIdentifier(kind), ":", SanitizeIdentifier(toRefId));
    }

    private static IEnumerable<XElement> EnumerateContainedMembersIterator(XElement container)
    {
        foreach (var child in container.Elements())
        {
            var localName = child.Name.LocalName;
            if (string.Equals(localName, "element", StringComparison.Ordinal) || string.Equals(localName, "attribute", StringComparison.Ordinal))
            {
                yield return child;
                continue;
            }

            if (ShouldRecurseIntoContainer(localName))
            {
                foreach (var descendant in EnumerateContainedMembers(child))
                {
                    yield return descendant;
                }
            }
        }
    }

    private static int GetOneBasedOrdinal(XElement item)
    {
        var siblings = item.Parent?.Elements(item.Name) ?? Enumerable.Repeat(item, 1);
        return siblings.TakeWhile(candidate => !ReferenceEquals(candidate, item)).Count() + 1;
    }

    private static bool ShouldRecurseIntoContainer(string localName)
    {
        return string.Equals(localName, "all", StringComparison.Ordinal)
            || string.Equals(localName, "choice", StringComparison.Ordinal)
            || string.Equals(localName, "complexContent", StringComparison.Ordinal)
            || string.Equals(localName, "complexType", StringComparison.Ordinal)
            || string.Equals(localName, "extension", StringComparison.Ordinal)
            || string.Equals(localName, "group", StringComparison.Ordinal)
            || string.Equals(localName, "restriction", StringComparison.Ordinal)
            || string.Equals(localName, "sequence", StringComparison.Ordinal)
            || string.Equals(localName, "simpleContent", StringComparison.Ordinal);
    }
}
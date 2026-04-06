using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Parsing;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Provides a category-specific handler for one discovered schema or WSDL item.
/// </summary>
internal interface IParsedItemHandler
{
    /// <summary>
    /// Determines whether the handler supports the supplied item.
    /// </summary>
    /// <param name="item">The discovered XML item.</param>
    /// <returns><see langword="true"/> when the handler supports the supplied item.</returns>
    bool CanHandle(XElement item);

    /// <summary>
    /// Handles the supplied item and returns the canonical reference identifier when one is produced.
    /// </summary>
    /// <param name="context">The current parsing context.</param>
    /// <param name="item">The XML item to handle.</param>
    /// <param name="parentRefId">The optional parent reference identifier.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the canonical reference identifier when applicable.</returns>
    Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken);
}
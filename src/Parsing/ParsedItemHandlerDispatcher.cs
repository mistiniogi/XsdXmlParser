using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Abstractions;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Routes discovered schema items to the first compatible category-specific handler.
/// </summary>
internal sealed class ParsedItemHandlerDispatcher
{
    /// <summary>
    /// The ordered handler set used to translate discovered XML items into normalized graph state.
    /// </summary>
    private readonly IReadOnlyList<IParsedItemHandler> handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedItemHandlerDispatcher"/> class.
    /// </summary>
    /// <param name="handlers">The available category-specific handlers.</param>
    public ParsedItemHandlerDispatcher(IEnumerable<IParsedItemHandler> handlers)
    {
        this.handlers = (handlers ?? throw new ArgumentNullException(nameof(handlers))).ToArray();
    }

    /// <summary>
    /// Creates a dispatcher with the default handler set used by compatibility constructors.
    /// </summary>
    /// <returns>The default dispatcher.</returns>
    public static ParsedItemHandlerDispatcher CreateDefault()
    {
        return new ParsedItemHandlerDispatcher(
            new IParsedItemHandler[]
            {
                new ComplexTypeParsedItemHandler(),
                new SimpleTypeParsedItemHandler(),
                new ElementParsedItemHandler(),
                new AttributeParsedItemHandler(),
                new WsdlServiceParsedItemHandler(),
            });
    }

    /// <summary>
    /// Determines whether any registered handler supports the supplied item.
    /// </summary>
    /// <param name="item">The item to inspect.</param>
    /// <returns><see langword="true"/> when a registered handler supports the item.</returns>
    public bool Supports(XElement item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return handlers.Any(candidate => candidate.CanHandle(item));
    }

    /// <summary>
    /// Dispatches an item to the first compatible handler.
    /// </summary>
    /// <param name="context">The current parsing context.</param>
    /// <param name="item">The item to dispatch.</param>
    /// <param name="parentRefId">The optional parent reference identifier.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the canonical reference identifier when one is produced.</returns>
    /// <remarks>
    /// The dispatcher preserves handler ordering so more specific handlers can run before broader item categories.
    /// </remarks>
    public Task<string?> HandleAsync(ParsedItemContext context, XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(item);

        var handler = handlers.FirstOrDefault(candidate => candidate.CanHandle(item));
        return handler is null
            ? Task.FromResult<string?>(null)
            : handler.HandleAsync(context, item, parentRefId, localOrdinal, cancellationToken);
    }
}
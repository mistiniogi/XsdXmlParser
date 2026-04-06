using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Registry;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Carries shared state for category-specific parsed item handlers.
/// </summary>
internal sealed class ParsedItemContext
{
    /// <summary>
    /// The nested item dispatcher used when handlers must recursively process child items.
    /// </summary>
    private Func<XElement, string?, int, CancellationToken, Task<string?>>? nestedItemHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedItemContext"/> class.
    /// </summary>
    /// <param name="graph">The graph being populated.</param>
    /// <param name="schemaRegistryService">The canonical schema registry service.</param>
    /// <param name="source">The current source descriptor.</param>
    /// <param name="targetNamespace">The active target namespace.</param>
    public ParsedItemContext(MetadataGraphModel graph, SchemaRegistryService schemaRegistryService, SourceDescriptorModel source, string targetNamespace)
    {
        Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        SchemaRegistryService = schemaRegistryService ?? throw new ArgumentNullException(nameof(schemaRegistryService));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        TargetNamespace = targetNamespace ?? string.Empty;
    }

    /// <summary>
    /// Gets the graph being populated.
    /// </summary>
    /// <value>The normalized metadata graph that receives parsed output.</value>
    public MetadataGraphModel Graph { get; }

    /// <summary>
    /// Gets the canonical schema registry service.
    /// </summary>
    /// <value>The registry service responsible for canonical entry and relationship storage.</value>
    public SchemaRegistryService SchemaRegistryService { get; }

    /// <summary>
    /// Gets the current source descriptor.
    /// </summary>
    /// <value>The current source descriptor whose content is being processed.</value>
    public SourceDescriptorModel Source { get; }

    /// <summary>
    /// Gets the active target namespace.
    /// </summary>
    /// <value>The target namespace currently in scope for qualified name resolution.</value>
    public string TargetNamespace { get; }

    /// <summary>
    /// Sets the nested item dispatcher used for recursive category handling.
    /// </summary>
    /// <param name="handler">The nested item dispatcher.</param>
    /// <remarks>
    /// The dispatcher is supplied after construction so handlers can recurse through a shared context without circular constructor dependencies.
    /// </remarks>
    public void SetNestedItemHandler(Func<XElement, string?, int, CancellationToken, Task<string?>> handler)
    {
        nestedItemHandler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// Dispatches a nested schema item to the matching handler.
    /// </summary>
    /// <param name="item">The nested item to handle.</param>
    /// <param name="parentRefId">The optional parent reference identifier.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the canonical reference identifier when one is produced.</returns>
    /// <remarks>
    /// Nested dispatch allows category handlers to reuse the shared dispatcher when inline complex or simple types appear inside broader parent items.
    /// </remarks>
    public Task<string?> HandleNestedItemAsync(XElement item, string? parentRefId, int localOrdinal, CancellationToken cancellationToken)
    {
        return nestedItemHandler is null
            ? throw new InvalidOperationException("A nested item handler has not been configured for the current parsing context.")
            : nestedItemHandler(item, parentRefId, localOrdinal, cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Serializes a normalized metadata graph for downstream consumers and generated reference workflows.
/// </summary>
public interface IMetadataGraphSerializer
{
    /// <summary>
    /// Serializes a graph to a JSON string.
    /// </summary>
    /// <param name="graph">The graph to serialize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the serialized JSON payload.</returns>
    /// <remarks>
    /// Use this overload when the caller needs an in-memory payload for logging, snapshotting, or transport.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// string json = await serializer.SerializeAsync(graph, cancellationToken).ConfigureAwait(false);
    /// ]]></code>
    /// </example>
    Task<string> SerializeAsync(MetadataGraphModel graph, CancellationToken cancellationToken);

    /// <summary>
    /// Serializes a graph to a writable stream.
    /// </summary>
    /// <param name="graph">The graph to serialize.</param>
    /// <param name="output">The output stream that receives the serialized JSON payload.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// Use this overload when the serialized graph should be written directly to a file, network stream, or response body.
    /// </remarks>
    Task SerializeAsync(MetadataGraphModel graph, Stream output, CancellationToken cancellationToken);
}
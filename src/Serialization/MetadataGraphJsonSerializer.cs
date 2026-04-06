using System.Text.Json;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;
using XsdXmlParser.Core.Serialization.Converters;

namespace XsdXmlParser.Core.Serialization;

/// <summary>
/// Serializes normalized metadata graphs with <see cref="JsonSerializer"/>.
/// </summary>
public sealed class MetadataGraphJsonSerializer : IMetadataGraphSerializer
{
    /// <summary>
    /// The serializer options used for normalized metadata graph output.
    /// </summary>
    private readonly JsonSerializerOptions serializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataGraphJsonSerializer"/> class.
    /// </summary>
    /// <remarks>
    /// The default configuration emits indented web-style JSON and registers graph-specific converters required by the normalized metadata model.
    /// </remarks>
    public MetadataGraphJsonSerializer()
    {
        serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        serializerOptions.Converters.Add(new ConstraintSetJsonConverter());
        serializerOptions.WriteIndented = true;
    }

    /// <inheritdoc/>
    public Task<string> SerializeAsync(MetadataGraphModel graph, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(graph);
        var json = JsonSerializer.Serialize(graph, serializerOptions);
        return Task.FromResult(json);
    }

    /// <inheritdoc/>
    public async Task SerializeAsync(MetadataGraphModel graph, Stream output, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(output);
        await JsonSerializer.SerializeAsync(output, graph, serializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
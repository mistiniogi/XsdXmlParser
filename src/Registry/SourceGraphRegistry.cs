using System.Collections.Generic;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Registry;

/// <summary>
/// Tracks source-level state used during discovery and cycle-safe traversal.
/// </summary>
public sealed class SourceGraphRegistry
{
    private readonly Dictionary<string, SourceDescriptorModel> descriptors = new(StringComparer.Ordinal);
    private readonly HashSet<string> resolvingSources = new(StringComparer.Ordinal);

    /// <summary>
    /// Registers a normalized source descriptor.
    /// </summary>
    /// <param name="descriptor">The descriptor to register.</param>
    public void Register(SourceDescriptorModel descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        descriptors[descriptor.SourceId] = descriptor;
    }

    /// <summary>
    /// Attempts to retrieve a registered source descriptor.
    /// </summary>
    /// <param name="sourceId">The source identifier to retrieve.</param>
    /// <param name="descriptor">The matching descriptor when present.</param>
    /// <returns><see langword="true"/> when the descriptor is present.</returns>
    public bool TryGetDescriptor(string sourceId, out SourceDescriptorModel descriptor)
    {
        return descriptors.TryGetValue(sourceId, out descriptor!);
    }

    /// <summary>
    /// Marks a source as currently resolving.
    /// </summary>
    /// <param name="sourceId">The source identifier to mark.</param>
    /// <returns><see langword="true"/> when the source was not already resolving.</returns>
    public bool BeginResolution(string sourceId)
    {
        return resolvingSources.Add(sourceId);
    }

    /// <summary>
    /// Marks a source as no longer resolving.
    /// </summary>
    /// <param name="sourceId">The source identifier to unmark.</param>
    public void EndResolution(string sourceId)
    {
        _ = resolvingSources.Remove(sourceId);
    }

    /// <summary>
    /// Determines whether a source is currently resolving.
    /// </summary>
    /// <param name="sourceId">The source identifier to inspect.</param>
    /// <returns><see langword="true"/> when the source is currently resolving.</returns>
    public bool IsResolving(string sourceId)
    {
        return resolvingSources.Contains(sourceId);
    }
}
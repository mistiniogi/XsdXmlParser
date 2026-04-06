using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Validates and assigns logical source identities for normalized parser sources.
/// </summary>
public sealed class SourceIdentityProviderService : ISourceIdentityProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// Identity validation is performed before discovery and graph-building work begins so duplicate logical inputs cannot collide in registry state.
    /// </remarks>
    public string GetOrCreateIdentity(SourceDescriptorModel descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if (!string.IsNullOrWhiteSpace(descriptor.SourceId))
        {
            return descriptor.SourceId;
        }

        if (!string.IsNullOrWhiteSpace(descriptor.LogicalName))
        {
            return descriptor.LogicalName;
        }

        throw new InvalidOperationException("A logical source identity is required for parser inputs.");
    }

    /// <inheritdoc/>
    public void ValidateUniqueIdentities(IEnumerable<SourceDescriptorModel> descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        var identities = new HashSet<string>(StringComparer.Ordinal);
        foreach (var descriptor in descriptors)
        {
            ArgumentNullException.ThrowIfNull(descriptor);
            var sourceId = GetOrCreateIdentity(descriptor);
            if (!identities.Add(sourceId))
            {
                throw new InvalidOperationException($"Duplicate logical source identity '{sourceId}' was supplied.");
            }
        }
    }
}
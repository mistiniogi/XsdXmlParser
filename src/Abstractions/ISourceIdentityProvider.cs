using System.Collections.Generic;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Assigns and validates logical source identities for normalized parser inputs.
/// </summary>
public interface ISourceIdentityProvider
{
    /// <summary>
    /// Creates or retrieves a logical source identity for a source descriptor.
    /// </summary>
    /// <param name="descriptor">The descriptor that requires an identity.</param>
    /// <returns>The logical source identity used by registry and graph components.</returns>
    string GetOrCreateIdentity(SourceDescriptorModel descriptor);

    /// <summary>
    /// Validates that a set of descriptors has unique logical identities.
    /// </summary>
    /// <param name="descriptors">The descriptors to validate.</param>
    /// <remarks>
    /// Implementations should reject collisions before graph construction begins so canonical registration does not merge unrelated sources.
    /// </remarks>
    void ValidateUniqueIdentities(IEnumerable<SourceDescriptorModel> descriptors);
}
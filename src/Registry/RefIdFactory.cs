using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace XsdXmlParser.Core.Registry;

/// <summary>
/// Generates deterministic reference identifiers for canonical graph entries.
/// </summary>
public sealed class RefIdFactory
{
    /// <summary>
    /// Creates a deterministic anonymous-type reference identifier.
    /// </summary>
    /// <param name="sourceId">The logical source identifier.</param>
    /// <param name="parentRefId">The parent reference identifier.</param>
    /// <param name="schemaPath">The normalized schema path.</param>
    /// <param name="localOrdinal">The zero-based ordinal within the parent scope.</param>
    /// <returns>The deterministic anonymous-type reference identifier.</returns>
    /// <remarks>
    /// Anonymous identifiers incorporate source, parent scope, schema path, and ordinal so repeated builds produce stable reference identifiers.
    /// </remarks>
    public string CreateAnonymousTypeRefId(string sourceId, string parentRefId, string schemaPath, int localOrdinal)
    {
        var material = string.Create(
            CultureInfo.InvariantCulture,
            $"{sourceId}|{parentRefId}|{schemaPath}|{localOrdinal}");

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(material));
        return "anon-" + Convert.ToHexString(hash).ToLowerInvariant();
    }
}
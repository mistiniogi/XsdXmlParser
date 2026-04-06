using XsdXmlParser.Core.Abstractions;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Resolves import and include targets through the configured virtual file system.
/// </summary>
public sealed class ImportResolutionService
{
    private readonly IVirtualFileSystem virtualFileSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportResolutionService"/> class.
    /// </summary>
    /// <param name="virtualFileSystem">The virtual file system used for resolution.</param>
    public ImportResolutionService(IVirtualFileSystem virtualFileSystem)
    {
        this.virtualFileSystem = virtualFileSystem ?? throw new ArgumentNullException(nameof(virtualFileSystem));
    }

    /// <summary>
    /// Resolves a relative import or include path against a base path.
    /// </summary>
    /// <param name="basePath">The base logical path.</param>
    /// <param name="relativePath">The relative path to resolve.</param>
    /// <returns>The normalized resolved path.</returns>
    public string Resolve(string basePath, string relativePath)
    {
        // Why: path normalization must be centralized so cycle detection and diagnostics
        // operate on one canonical view of every logical source path.
        return virtualFileSystem.ResolveRelativePath(basePath, relativePath);
    }
}
namespace XsdXmlParser.Core.Tests.Integration.Infrastructure;

/// <summary>
/// Resolves repository-relative fixture paths into absolute file-system locations.
/// </summary>
public static class FixturePathResolver
{
    /// <summary>
    /// Resolves a fixture path relative to <c>tests/Integration/wsdl-fixtures</c>.
    /// </summary>
    /// <param name="relativeFixturePath">The fixture path relative to the fixture root.</param>
    /// <returns>The absolute fixture path.</returns>
    public static string ResolveFixturePath(string relativeFixturePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeFixturePath);

        string fixtureRoot = Path.Combine(ResolveRepositoryRoot(), "tests", "Integration", "wsdl-fixtures");
        string fullPath = Path.GetFullPath(Path.Combine(fixtureRoot, relativeFixturePath));

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Fixture file was not found for '{relativeFixturePath}'.", fullPath);
        }

        return fullPath;
    }

    /// <summary>
    /// Resolves the repository root by walking upward from the test output directory.
    /// </summary>
    /// <returns>The absolute repository root path.</returns>
    public static string ResolveRepositoryRoot()
    {
        string current = AppContext.BaseDirectory;

        // Why: test binaries run from deep framework-specific output folders, so we climb up
        // to the first directory that contains the repository solution file.
        while (!string.IsNullOrEmpty(current))
        {
            string solutionPath = Path.Combine(current, "XsdXmlParser.sln");
            if (File.Exists(solutionPath))
            {
                return current;
            }

            string? parent = Directory.GetParent(current)?.FullName;
            if (string.Equals(parent, current, StringComparison.Ordinal))
            {
                break;
            }

            current = parent ?? string.Empty;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root containing XsdXmlParser.sln.");
    }
}

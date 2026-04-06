using System.Collections.ObjectModel;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a structured parsing failure that preserves stage and diagnostic context.
/// </summary>
public sealed class ParseFailureException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseFailureException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="stage">The high-level stage that failed.</param>
    /// <param name="diagnostics">The structured diagnostics associated with the failure.</param>
    /// <param name="sourceId">The optional primary failing source identifier.</param>
    /// <param name="innerException">The optional inner exception.</param>
    /// <remarks>
    /// Use this exception to surface parse failures that should remain actionable to callers without exposing internal implementation details alone.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// throw new ParseFailureException("Import resolution failed.", "resolution", diagnostics, sourceId);
    /// ]]></code>
    /// </example>
    public ParseFailureException(string message, string stage, IEnumerable<ParseDiagnosticModel>? diagnostics = null, string? sourceId = null, Exception? innerException = null)
        : base(message, innerException)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));
        }

        if (string.IsNullOrWhiteSpace(stage))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(stage));
        }

        Stage = stage;
        SourceId = sourceId;
        Diagnostics = new ReadOnlyCollection<ParseDiagnosticModel>((diagnostics ?? Array.Empty<ParseDiagnosticModel>()).ToArray());
    }

    /// <summary>
    /// Gets the structured diagnostics associated with the failure.
    /// </summary>
    /// <value>The diagnostics that describe the failure in more detail.</value>
    public IReadOnlyList<ParseDiagnosticModel> Diagnostics { get; }

    /// <summary>
    /// Gets the high-level failure stage.
    /// </summary>
    /// <value>The stage label used to categorize where the failure occurred.</value>
    public string Stage { get; }

    /// <summary>
    /// Gets the optional primary failing source identifier.
    /// </summary>
    /// <value>The primary source identifier associated with the failure, when known.</value>
    public string? SourceId { get; }
}
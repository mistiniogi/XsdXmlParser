using System.Collections.ObjectModel;

namespace XsdXmlParser.Core.Models;

/// <summary>
/// Represents a structured parsing failure.
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
    public IReadOnlyList<ParseDiagnosticModel> Diagnostics { get; }

    /// <summary>
    /// Gets the high-level failure stage.
    /// </summary>
    public string Stage { get; }

    /// <summary>
    /// Gets the optional primary failing source identifier.
    /// </summary>
    public string? SourceId { get; }
}
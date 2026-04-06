using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Coordinates WSDL and XSD parsing workflows for the primary request-model-based input forms.
/// </summary>
public interface IParserOrchestrationService
{
    /// <summary>
    /// Parses a file-backed request.
    /// </summary>
    /// <param name="request">The file-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a string-backed request.
    /// </summary>
    /// <param name="request">The string-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    Task<MetadataGraphModel> ParseStringAsync(StringParseRequestModel request, CancellationToken cancellationToken);
}
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Coordinates the primary WSDL and XSD parsing workflows for request-model-based inputs.
/// </summary>
public interface IParserOrchestrationService
{
    /// <summary>
    /// Parses a file-backed request.
    /// </summary>
    /// <param name="request">The file-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    /// <remarks>
    /// This overload is intended for callers that already model input and options through <see cref="FilePathParseRequestModel"/>.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// var graph = await orchestrationService.ParseFileAsync(
    ///     new FilePathParseRequestModel { FilePath = "schemas/service.wsdl" },
    ///     cancellationToken).ConfigureAwait(false);
    /// ]]></code>
    /// </example>
    Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Parses a string-backed request.
    /// </summary>
    /// <param name="request">The string-backed parse request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the parsed metadata graph.</returns>
    /// <remarks>
    /// This overload is intended for callers that supply schema content from memory while still requiring relative import resolution.
    /// </remarks>
    Task<MetadataGraphModel> ParseStringAsync(StringParseRequestModel request, CancellationToken cancellationToken);
}
using System.Threading;
using System.Threading.Tasks;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Abstractions;

/// <summary>
/// Normalizes caller-supplied file-backed and string-backed schema inputs into source descriptors.
/// </summary>
public interface ISourceLoader
{
    /// <summary>
    /// Normalizes a file-backed parse request.
    /// </summary>
    /// <param name="request">The file-backed request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadAsync(FilePathParseRequestModel request, CancellationToken cancellationToken);

    /// <summary>
    /// Normalizes a string-backed parse request.
    /// </summary>
    /// <param name="request">The string-backed request to normalize.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized source descriptor.</returns>
    Task<SourceDescriptorModel> LoadAsync(StringParseRequestModel request, CancellationToken cancellationToken);
}
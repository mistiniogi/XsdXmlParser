using System.Collections.Generic;
using System.Linq;
using XsdXmlParser.Core.Abstractions;
using XsdXmlParser.Core.Models;

namespace XsdXmlParser.Core.Parsing;

/// <summary>
/// Provides the primary consumer-facing orchestration flow for WSDL and XSD parsing.
/// </summary>
public sealed class ParserOrchestrationService : IParserOrchestrationService
{
    /// <summary>
    /// The graph builder that assembles normalized metadata output.
    /// </summary>
    private readonly IMetadataGraphBuilder metadataGraphBuilder;

    /// <summary>
    /// The source loader that normalizes request inputs before orchestration begins.
    /// </summary>
    private readonly ISourceLoader sourceLoader;

    /// <summary>
    /// The WSDL discovery service that expands referenced WSDL and XSD sources when needed.
    /// </summary>
    private readonly WsdlDiscoveryService wsdlDiscoveryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserOrchestrationService"/> class.
    /// </summary>
    /// <param name="sourceLoader">The source loader used to normalize requests.</param>
    /// <param name="wsdlDiscoveryService">The WSDL discovery service.</param>
    /// <param name="metadataGraphBuilder">The graph builder used to construct metadata output.</param>
    public ParserOrchestrationService(ISourceLoader sourceLoader, WsdlDiscoveryService wsdlDiscoveryService, IMetadataGraphBuilder metadataGraphBuilder)
    {
        this.sourceLoader = sourceLoader ?? throw new ArgumentNullException(nameof(sourceLoader));
        this.wsdlDiscoveryService = wsdlDiscoveryService ?? throw new ArgumentNullException(nameof(wsdlDiscoveryService));
        this.metadataGraphBuilder = metadataGraphBuilder ?? throw new ArgumentNullException(nameof(metadataGraphBuilder));
    }

    /// <inheritdoc/>
    public Task<MetadataGraphModel> ParseFileAsync(FilePathParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteAsync(async () => new[] { await sourceLoader.LoadAsync(request, cancellationToken).ConfigureAwait(false) }, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<MetadataGraphModel> ParseStringAsync(StringParseRequestModel request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteAsync(async () => new[] { await sourceLoader.LoadAsync(request, cancellationToken).ConfigureAwait(false) }, cancellationToken);
    }

    /// <summary>
    /// Creates a structured orchestration failure for unexpected exceptions.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <param name="stage">The orchestration stage where the failure occurred.</param>
    /// <returns>The structured parse failure exception.</returns>
    private static ParseFailureException CreateUnexpectedFailure(Exception exception, string stage)
    {
        var diagnostics = new[]
        {
            new ParseDiagnosticModel
            {
                Code = "unexpected-failure",
                DiagnosticId = Guid.NewGuid().ToString("N"),
                Details = exception.ToString(),
                Message = exception.Message,
                Stage = stage,
            },
        };

        return new ParseFailureException(exception.Message, stage, diagnostics, null, exception);
    }

    /// <summary>
    /// Executes the shared orchestration workflow after request-specific source loading has been selected.
    /// </summary>
    /// <param name="loadDescriptorsAsync">The callback that loads the initial normalized descriptors.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A task that returns the normalized metadata graph.</returns>
    private async Task<MetadataGraphModel> ExecuteAsync(Func<Task<IReadOnlyList<SourceDescriptorModel>>> loadDescriptorsAsync, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var descriptors = await loadDescriptorsAsync().ConfigureAwait(false);

            // Why: one discovery pass keeps WSDL and XSD requests consistent and lets mixed batch
            // inputs expand referenced WSDL imports before graph construction starts.
            var discoveredDescriptors = descriptors.Any(descriptor => descriptor.DocumentKind == ESchemaDocumentKind.Wsdl)
                ? await wsdlDiscoveryService.DiscoverAsync(descriptors, cancellationToken).ConfigureAwait(false)
                : descriptors;

            // Why: source normalization ends here and the registry-backed graph builder owns all
            // downstream source registration, discovery tracking, and canonical graph assembly.
            return await metadataGraphBuilder.BuildAsync(discoveredDescriptors, cancellationToken).ConfigureAwait(false);
        }
        catch (ParseFailureException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw CreateUnexpectedFailure(exception, "orchestration");
        }
    }
}
# Research: Simplify Source Loading Inputs

## Public Contract Reduction Strategy

- Decision: Remove stream, memory, and batch source-loading methods from the public parser and loader contracts instead of keeping compatibility shims.
- Rationale: The clarified spec explicitly requires the supported contract to be limited to file and string inputs and says removed workflows should be absent rather than silently rerouted.
- Alternatives considered: Keep removed APIs as obsolete wrappers. Rejected because that preserves unsupported paths in the public contract and broadens the feature beyond the requested simplification.

## String Request Modeling Strategy

- Decision: Introduce a dedicated `StringParseRequestModel` derived from `ParseRequestModel` to represent textual schema input.
- Rationale: The current codebase has `FilePathParseRequestModel`, `StreamParseRequestModel`, and `MemoryParseRequestModel`, but no text-native request model. Reusing the removed memory contract would leak a public concept the feature is explicitly removing.
- Alternatives considered: Reuse `MemoryParseRequestModel` and reinterpret its bytes as text. Rejected because it conflicts with the new public contract and would keep memory-buffer terminology in consumer-facing APIs and XML docs.

## Internal Normalization Strategy

- Decision: Preserve the existing virtual-file-system normalization seam and route string content through it internally after converting the string to bytes.
- Rationale: This keeps import/include resolution and source-identity behavior stable while limiting the feature to a public contract change rather than a deeper parser rewrite.
- Alternatives considered: Add a separate virtual-file-system text-loading abstraction. Rejected because it adds new infrastructure without a corresponding functional requirement.

## Source Descriptor Reporting Strategy

- Decision: Update source-kind reporting so consumer-visible descriptors reflect only retained file and string source origins.
- Rationale: `SourceDescriptorModel.SourceKind` is part of the normalized output surface. Leaving `Memory`, `Stream`, or `BatchStream` as active consumer-visible kinds would contradict the simplified contract.
- Alternatives considered: Keep using the existing `Memory` source kind for string-backed content internally and externally. Rejected because it leaks removed terminology into the post-change public model.

## Parser Compatibility Strategy

- Decision: Make `IParserOrchestrationService` the primary request-model-based contract for explicit caller-declared document kind, while narrowing `IXsdParser` and `IWsdlParser` to file and string workflows as secondary typed adapters whose abstraction implies document kind.
- Rationale: The spec requires explicit document kind for retained source-loading workflows, but the repository already has type-specific parser interfaces. Treating orchestration as the primary contract keeps the explicit-kind rule intact without forcing redundant kind parameters onto type-specific adapters.
- Alternatives considered: Require explicit document kind on every `IXsdParser` and `IWsdlParser` convenience method. Rejected because the interface type already supplies that semantic distinction and would add redundant API noise.

## DI And Registry Verification Strategy

- Decision: Include an explicit verification step for `ServiceCollectionExtensions` registration and the registry-backed source-descriptor flow after the contract reduction.
- Rationale: The constitution requires task and plan coverage for DI boundaries and centralized registry behavior, even when the feature is mostly a public-surface reduction.
- Alternatives considered: Assume DI and registry behavior remain correct because parser internals are mostly unchanged. Rejected because it leaves a constitution-mandated concern untracked.

## Runtime Validation Strategy

- Decision: Validate retained workflows using representative quickstart scenarios for valid file input, valid string input, invalid file/string input, and file-vs-string readiness comparison in addition to `dotnet build`.
- Rationale: The success criteria require evidence beyond compilation, especially for failure handling and equivalence between retained input forms.
- Alternatives considered: Use build-only validation. Rejected because it cannot demonstrate runtime failure semantics or parity between retained workflows.

## Documentation And Comment Strategy

- Decision: Treat XML documentation comments, `README.md`, and `docs/getting-started.md` as part of the feature’s required deliverables.
- Rationale: The user asked to update comments with the modified spec, and the constitution requires full XML docs plus rationale comments where logic is non-obvious.
- Alternatives considered: Update implementation only and defer docs/comments. Rejected because it leaves unsupported source forms discoverable and violates the repository’s documentation expectations.
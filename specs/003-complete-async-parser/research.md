# Research: Complete WSDL/XSD Parsing Workflows

## Public Entry Point Strategy

- Decision: Add one consumer-facing orchestration service as the primary public parsing API, with async methods for file path, stream, read-only memory, and batch-source requests.
- Rationale: The clarified spec requires one consumer-facing service, and centralizing entry points simplifies adoption while still allowing internal parser specialization.
- Alternatives considered: Keep `IWsdlParser` and `IXsdParser` as the only public APIs. Rejected because it conflicts with the clarified public-contract direction and leaves input-shape guidance fragmented.

## Compatibility Preservation Strategy

- Decision: Keep `IWsdlParser` and `IXsdParser` as lower-level collaborators or compatibility adapters while shifting documentation and DI guidance toward the orchestration service.
- Rationale: Repository guidance explicitly prefers extending existing design over replacing it, and this avoids unnecessary public-surface breakage.
- Alternatives considered: Remove or rename the existing parser interfaces. Rejected because it would alter established public design more than the feature requires.

## Request Contract Design

- Decision: Model file path, stream, read-only memory, and batch-source inputs as explicit request models that all carry a caller-supplied WSDL/XSD source kind.
- Rationale: The feature needs a predictable public contract across multiple input forms, and explicit source kind avoids detection ambiguity at orchestration boundaries.
- Alternatives considered: Infer source kind automatically from content or file extension. Rejected because the clarification requires caller-supplied source kind on every request.

## Failure Contract Strategy

- Decision: Throw parsing exceptions for all parse failures and reserve `MetadataGraphModel` for successful outcomes only.
- Rationale: The clarified spec rejects partial results and requires exception-based failure signaling, so the plan must standardize one failure path with structured diagnostics.
- Alternatives considered: Return failure-state result objects with diagnostics. Rejected because it directly conflicts with the accepted clarification.

## Item-Category Parsing Strategy

- Decision: Implement item-category parsing through one shared contract with dedicated handlers for elements, attributes, simple types, complex types, and WSDL-derived service artifacts.
- Rationale: This satisfies the feature requirement for separation of concerns, inheritance/polymorphism, and maintainable evolution of item-specific logic.
- Alternatives considered: Continue using one graph-builder class with category-specific branching embedded inline. Rejected because it preserves the current missing-logic bottleneck and weakens extensibility.

## Async Migration Strategy

- Decision: Keep asynchronous methods across orchestration, source loading, WSDL discovery, graph building, and serialization, and propagate `CancellationToken` through every affected code path.
- Rationale: The constitution requires async-first design, and the feature explicitly calls for async/await patterns across methods.
- Alternatives considered: Preserve synchronous internal code paths behind async public wrappers. Rejected because it risks sync-over-async behavior and weak cancellation semantics.

## Documentation Strategy

- Decision: Update the feature quickstart and repository-facing usage guidance around the orchestration service, request models, explicit source kind, and thrown parse failures.
- Rationale: The spec makes consumer documentation part of the feature scope, and the new primary API shape must be discoverable without source-code inspection.
- Alternatives considered: Document only implementation classes or preserve existing separate WSDL/XSD examples as the main entry point. Rejected because it would not reflect the clarified public contract.

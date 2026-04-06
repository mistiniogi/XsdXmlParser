# XML Documentation Coverage Contract

## Goal

Define the implementation and review contract for the XML documentation enhancement pass on the shared production C# surface.

## In-Scope Artifacts

- Production C# source files under `src/`
- All declaration visibility levels present in reviewed source files
- Declaration kinds in scope:
  - Classes
  - Interfaces
  - Structs
  - Enums
  - Constructors
  - Properties
  - Methods
  - Delegates
  - Events
  - Applicable fields

## Out-Of-Scope Artifacts

- `tests/`
- Generated files
- `bin/`
- `obj/`
- `README.md`
- `docs/`

## Documentation Rules

### Universal Requirements

- Every in-scope declaration must have a meaningful `<summary>`.
- Existing accurate comments must be preserved and enhanced, not replaced arbitrarily.
- Documentation text must describe observable contract and intended usage without speculative behavior.
- Terminology must remain consistent with the library architecture: async-first services, DI registration, centralized registries, normalized metadata graphs, parsers, loaders, and serializers.

### Tag Requirements By Declaration Shape

- Add `<param>` for each declared parameter.
- Add `<returns>` for methods with a return value.
- Add `<value>` for properties and other value-bearing declarations where appropriate.
- Add `<typeparam>` for generic declarations.
- Add `<remarks>` when the summary alone is insufficient.
- Add `<exception>` when exceptional conditions are part of the declaration contract and materially help the reader.
- Add `<paramref>`, `<typeparamref>`, `<see>`, or `<seealso>` when inline references materially improve precision or generated reference quality.
- Add `<example>` only when it materially improves understanding of how the declaration should be used.
- Add fuller explanatory detail when terse summary-only text would leave intent, constraints, or usage interplay unclear.

### Declaration Coverage Matrix

- Types in scope must document purpose, role, and architectural responsibility.
- Constructors in scope must document parameters and any lifecycle expectations that are not obvious from the type summary.
- Methods in scope must document inputs, outputs, cancellation expectations when present, and contract boundaries without restating implementation logic.
- Properties and events in scope must document exposed value or notification semantics when the member name alone is not sufficient.
- Delegates and fields in scope must document intent and usage constraints when they are part of the library-facing contract.
- Lower-visibility declarations that remain in scope must meet the same completeness and accuracy standard as consumer-facing declarations, while still avoiding speculative or low-value prose.

### Example Policy

- Examples are required selectively, not universally.
- Prefer examples for orchestration entry points, DI registration methods, or non-obvious contract shapes.
- Do not add examples that merely restate the summary.

## Validation Contract

- The implementation diff must remain documentation-only.
- No signatures, behavior, control flow, or architecture may change.
- Multi-target `dotnet build` must remain valid after the comment pass.
- Review must confirm from source that all in-scope declaration kinds and visibility levels were covered.
- Review must spot-check generated XML documentation output only for representative declarations emitted by the compiler from abstractions, models, parsing, registry, and serialization.

## Completion Criteria

- Every in-scope declaration has an accurate and complete XML comment block.
- Existing correct comment intent remains intact.
- Applicable XML tags are present for each declaration kind.
- Inline reference tags are used where justified.
- Example usage is present only where justified.
- No out-of-scope files were edited.
# Data Model: Enhance XML Documentation Comments

## Overview

This feature does not change runtime data structures or parser behavior. The design impact is a documentation-workflow model that defines which declarations are in scope, how existing comments are enhanced, and what completion criteria govern the XML documentation pass.

## Entities

### Documentation Target

- Purpose: Represents one in-scope declaration in a production C# source file under `src/`.
- Key Attributes:
  - `FilePath`: The source file that contains the declaration.
  - `DeclarationKind`: Class, interface, struct, enum, constructor, property, method, delegate, event, or applicable field.
  - `Visibility`: Any declaration visibility level present in the reviewed source file.
  - `ConsumerFacing`: Indicates whether the declaration primarily shapes external usage, internal architecture, or both.
- Validation Rules:
  - Must be located under `src/`.
  - Must not come from generated files or ignored build output.

### Existing Comment Block

- Purpose: Captures the current XML documentation associated with a documentation target.
- Key Attributes:
  - `HasSummary`: Whether a `<summary>` tag already exists.
  - `ExistingTags`: The set of tags already present.
  - `IntentQuality`: Whether the current text is accurate, partial, placeholder-like, or inconsistent.
- Validation Rules:
  - Accurate existing intent should be preserved.
  - Placeholder or redundant content must be replaced with meaningful explanation.

### Enhanced Comment Block

- Purpose: Represents the final XML documentation content for a documentation target after the feature is applied.
- Key Attributes:
  - `SummaryText`: The declaration’s role and purpose.
  - `ApplicableTags`: The final set of contract tags included for the declaration.
  - `TerminologyAlignment`: Whether the wording matches the library’s established architectural vocabulary.
  - `IncludesExample`: Whether an example is included.
  - `InlineReferences`: Whether XML reference tags such as `paramref`, `typeparamref`, `see`, or `seealso` are used to improve precision.
- Validation Rules:
  - Must accurately describe observable behavior.
  - Must not add speculative guidance.
  - Must remain readable as standalone reference text.
  - Must add detail beyond terse summary-only wording when contract shape or usage constraints are not obvious.

### Applicable Tag Set

- Purpose: Defines which XML tags are required, optional, or not applicable for a documentation target.
- Key Attributes:
  - `RequiresSummary`: Always true for in-scope declarations.
  - `RequiresParamTags`: True for declarations with parameters.
  - `RequiresReturnsTag`: True for methods with return values.
  - `RequiresValueTag`: True for properties and other value-bearing members where appropriate.
  - `RequiresTypeParamTags`: True for generic declarations.
  - `AllowsRemarksOrExceptionTags`: True when additional contract context materially improves clarity.
  - `AllowsInlineReferenceTags`: True when parameters, type parameters, members, or framework types should be linked explicitly.
- Validation Rules:
  - Only applicable tags may be added.
  - Required tags must not be omitted for the declaration kind.

### Usage Example Candidate

- Purpose: Represents a declaration under consideration for an XML `<example>` block.
- Key Attributes:
  - `NeedsExample`: Whether summary and remarks alone are insufficient.
  - `Reason`: Non-obvious usage, orchestration flow, DI registration usage, or consumer confusion risk.
- Validation Rules:
  - Examples are added only when they materially improve understanding.
  - Existing examples are enhanced only when clarity or correctness improves.

## Relationships

- Each `Documentation Target` may have zero or one `Existing Comment Block` and must end with one `Enhanced Comment Block`.
- Each `Enhanced Comment Block` is governed by one `Applicable Tag Set`.
- A `Documentation Target` may optionally map to one `Usage Example Candidate` when example consideration is required.

## State Transitions

### Documentation Review Flow

1. Inventory all in-scope declarations under `src/`.
2. Assess whether each declaration already has XML documentation and whether its intent is accurate.
3. Determine the applicable tag set for the declaration kind and signature shape.
4. Enhance or add the XML comment block without modifying runtime code.
5. Decide whether a usage example is warranted.
6. Validate that the final comment block is accurate, complete, and consistent with neighboring declarations.

## Completion Signals

- Every in-scope declaration has one enhanced XML comment block.
- Existing accurate comment intent is preserved.
- Required tags for each declaration kind are present.
- Inline reference tags are used where they materially improve precision.
- Example coverage exists only where justified.
- The resulting diff contains documentation-only changes in source files.
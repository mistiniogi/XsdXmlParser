# Data Model: WSDL Test Assets

## Overview

This feature adds a filesystem-backed fixture catalog for future tests. The data-model impact is limited to how fixture assets are organized, named, and classified; no production runtime models change.

## Entities

### WsdlFixtureRoot

- Purpose: Top-level integration fixture catalog rooted at `tests/Integration/wsdl-fixtures`.
- Fields:
  - `RootPath`: Fixed repository-relative path for all WSDL fixture assets.
- Relationships:
  - Parent of six `FixtureCategoryFolder` entities.
- Validation Rules:
  - Must exist under `tests/Integration`.
  - Must contain only category folders for this feature's fixture catalog.

### FixtureCategoryFolder

- Purpose: Organizes fixture sets by complexity or validity.
- Required Categories:
  - `simplest-wsdls`
  - `simple-wsdls`
  - `complex-wsdls`
  - `very-complex-wsdls`
  - `very-complex-wsdls-with-xsd-imports`
  - `invalid-wsdls`
- Fields:
  - `CategoryName`: Stable kebab-case directory name.
  - `CategoryIntent`: Human-readable meaning derived from the spec.
- Relationships:
  - Child of `WsdlFixtureRoot`.
  - Parent of one or more `FixtureSet` entities.
- Validation Rules:
  - Must use ASCII kebab-case names.
  - Must contain at least one fixture set.
  - Must not mix invalid fixtures into valid categories.

### FixtureSet

- Purpose: Represents one reusable scenario folder within a category.
- Fields:
  - `SetName`: Stable scenario-specific kebab-case directory name.
  - `PrimaryScenario`: Short label describing the fixture purpose.
  - `Validity`: `Valid` or `Invalid`.
  - `ComplexityCategory`: One of the six category names above.
- Relationships:
  - Child of a `FixtureCategoryFolder`.
  - Parent of one primary `WsdlDocumentAsset` and zero or more `ImportedSchemaAsset` entities.
- Validation Rules:
  - Must contain exactly one primary WSDL file.
  - Import-based valid sets may include companion XSD files.
  - Non-import categories should remain self-contained to the degree implied by their category intent.

### WsdlDocumentAsset

- Purpose: Primary WSDL file for a fixture set.
- Fields:
  - `FileName`: ASCII kebab-case file name ending in `.wsdl`.
  - `StructuralTraitProfile`: Short trait summary used for category placement.
  - `IsStructurallyValidWsdl`: Boolean classification for planning purposes.
- Relationships:
  - Child of `FixtureSet`.
- Validation Rules:
  - Must match the complexity or invalidity rules of its parent category.
  - Invalid fixtures must remain recognizable as WSDL content but be intentionally structurally incorrect.

### ImportedSchemaAsset

- Purpose: Companion XSD file required by an import-based WSDL fixture.
- Fields:
  - `FileName`: ASCII kebab-case file name ending in `.xsd`.
  - `ImportRole`: Short description of what the schema contributes.
- Relationships:
  - Child of an import-based `FixtureSet`.
- Validation Rules:
  - Allowed only where the WSDL scenario requires external XSD imports.
  - Must stay colocated with the WSDL fixture set that references it.

### ComplexityDefinition

- Purpose: Defines how valid fixture sets are classified.
- Fields:
  - `CategoryName`: One of `simplest`, `simple`, `complex`, `very-complex`.
  - `RequiredTraits`: Short list of representative WSDL/XSD structure traits.
  - `ExcludedTraits`: Traits that would move the fixture into another category.
- Relationships:
  - Governs classification for valid `FixtureSet` entities.
- Validation Rules:
  - Must rely on structural traits, not subjective contributor judgment alone.
  - Must distinguish self-contained categories from the import-based category.

## State Transitions

### Valid Fixture Creation Flow

1. Choose the target category by matching the WSDL against documented structural traits.
2. Create a fixture-set directory under the appropriate category.
3. Add one primary WSDL file with a portable ASCII file name.
4. Add companion XSD files only if the category is `very-complex-wsdls-with-xsd-imports`.

### Invalid Fixture Creation Flow

1. Create a fixture-set directory under `invalid-wsdls`.
2. Add a primary WSDL file that is intentionally structurally wrong but still recognizable as WSDL content.
3. Keep the invalid scenario isolated from valid categories.

## Validation Impact

- Repository validation is directory- and content-oriented rather than runtime-model-oriented.
- Future tests can treat each fixture-set directory as an addressable scenario unit.

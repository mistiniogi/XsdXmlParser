# WSDL Fixture Directory Contract

## Goal

Define the stable filesystem contract for reusable WSDL test assets added by this feature.

## Normative Status

This contract is normative for fixture placement, naming, and taxonomy stability. If any example conflicts with a rule below, the rule wins.

## Root Path

All new fixture assets live under:

```text
tests/Integration/wsdl-fixtures/
```

## Category Directories

The root contains exactly these category directories for this feature:

```text
tests/Integration/wsdl-fixtures/
├── simplest-wsdls/
├── simple-wsdls/
├── complex-wsdls/
├── very-complex-wsdls/
├── very-complex-wsdls-with-xsd-imports/
└── invalid-wsdls/
```

## Fixture-Set Layout

Each category contains one or more scenario-specific fixture-set directories.

Example layout:

```text
tests/Integration/wsdl-fixtures/simple-wsdls/customer-lookup/
├── customer-lookup.wsdl
```

Import-based example:

```text
tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/order-platform/
├── order-platform.wsdl
├── order-types.xsd
└── shared-faults.xsd
```

## Naming Rules

- Category directories use lowercase ASCII kebab-case.
- Fixture-set directories use lowercase ASCII kebab-case.
- WSDL files use lowercase ASCII kebab-case and end with `.wsdl`.
- Companion schema files use lowercase ASCII kebab-case and end with `.xsd`.
- Names must avoid spaces and shell-sensitive punctuation.
- Names may contain digits only when they improve first-glance readability.
- Names may use abbreviations only when the abbreviation is a widely recognized domain term and remains readable without opening the file.
- Names should not repeat the parent category label unless repetition is required to disambiguate two otherwise confusing scenario names.
- Names must start with a lowercase letter, may contain only lowercase letters, digits, and single hyphens, and must not end with a hyphen or contain consecutive hyphens.

## Category Precedence Rules

1. `invalid-wsdls` overrides all valid complexity placement. A structurally wrong WSDL always belongs in `invalid-wsdls`, even if the remaining content resembles a valid complexity category.
2. `very-complex-wsdls-with-xsd-imports` overrides the four self-contained valid categories for any valid WSDL that requires one or more external XSD imports.
3. Among the four self-contained valid categories, contributors must choose the single category whose structural rules are the closest match and must not duplicate the same fixture set across valid categories.

## Category Semantics

### `simplest-wsdls`

- One service, one binding, one port type, one operation.
- No faults.
- Message parts use only primitive or otherwise minimal self-contained schema structure.
- No external schema imports.

### `simple-wsdls`

- Self-contained WSDL.
- One service, one binding, and one port type.
- Either two to four operations, or one to two operations with named message or type definitions richer than `simplest`.
- May include a limited number of richer messages or types, but does not cross the `complex-wsdls` triggers below.
- No external schema imports.

### `complex-wsdls`

- Self-contained WSDL.
- Must have at least one complexity trigger such as multiple bindings or ports, five or more operations, multiple faults, or several named complex types with nested structure.
- Must not meet the `very-complex-wsdls` combination threshold.
- No external schema imports.

### `very-complex-wsdls`

- Self-contained WSDL.
- Must have multiple higher-order complexity triggers, such as several interacting WSDL parts, several nested named complex types, or multiple service-definition layers whose combination materially exceeds `complex-wsdls`.
- No external schema imports.

### `very-complex-wsdls-with-xsd-imports`

- Valid WSDL scenario that requires one or more colocated imported XSD files.
- Any valid fixture that needs external XSD imports belongs here, even if its non-imported structure would otherwise appear simple or complex.

### `invalid-wsdls`

- Contains WSDL files that are intentionally structurally incorrect.
- Invalidity is at the WSDL structure level, not merely because the file is unrelated malformed XML.
- The file must still be recognizable as WSDL content rather than a generic malformed XML sample.

## Stability Expectations

- Future tests may treat each fixture-set directory as a stable scenario path.
- Companion XSD files must remain in the same fixture-set directory as the WSDL file that imports them.
- Invalid fixtures must stay isolated from valid categories.
- The six category names and their meanings are frozen for this feature's initial delivery.
- Any future rename or meaning change requires a subsequent approved feature change because downstream tests may depend on the current taxonomy.
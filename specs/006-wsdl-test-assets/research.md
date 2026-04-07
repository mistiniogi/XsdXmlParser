# Research: WSDL Test Assets

## Fixture Root Placement Strategy

- Decision: Place the new fixture catalog under `tests/Integration/wsdl-fixtures`.
- Rationale: The repository already groups integration scenarios under `tests/Integration`, and the clarified spec explicitly selected that area as the stable location for reusable WSDL fixtures.
- Alternatives considered: Put fixtures directly under `tests/`. Rejected because it would flatten the current test taxonomy. Put fixtures under `tests/Contract/` or `tests/Unit/`. Rejected because the assets are intended for broader future integration-style scenarios rather than unit-only or contract-only coverage.

## Directory Layout Strategy

- Decision: Use category folders under `tests/Integration/wsdl-fixtures`, and within each category use a self-contained fixture-set subfolder that holds the files for one scenario.
- Rationale: Fixture-set subfolders prevent file-name collisions, keep companion files together, and give future tests one stable directory per scenario.
- Alternatives considered: Place all files directly in the category root. Rejected because import-based scenarios and invalid variants become harder to group cleanly as the catalog grows.

## Category Naming Strategy

- Decision: Use lowercase ASCII kebab-case folder names: `simplest-wsdls`, `simple-wsdls`, `complex-wsdls`, `very-complex-wsdls`, `very-complex-wsdls-with-xsd-imports`, and `invalid-wsdls`.
- Rationale: Kebab-case is portable across macOS, Windows, and Linux, avoids shell-hostile characters, and preserves the category names requested in the spec while converting them to stable path names. Fixture-set and file names also allow digits and only widely recognized abbreviations when those improve first-glance readability without breaking the lowercase ASCII kebab-case rule.
- Alternatives considered: Keep human-readable folder names with spaces. Rejected because they are less reliable for shell tooling and automated path handling. Use camelCase or PascalCase names. Rejected because they are less consistent with filesystem fixture catalogs and provide no portability advantage.

## Valid Complexity Taxonomy Strategy

- Decision: Define valid categories by concise structural traits:
  - `simplest`: exactly one `service`, one `binding`, one `portType`, one operation, no faults, no imports, and only primitive or otherwise minimal self-contained schema structure.
  - `simple`: self-contained with no imports, one `service`, one `binding`, one `portType`, and either two to four operations or one to two operations with richer named message or type definitions than `simplest`.
  - `complex`: self-contained with no imports and at least one clear complexity trigger such as multiple bindings or ports, five or more operations, multiple faults, or several named complex types with nested structure.
  - `very-complex`: self-contained with no imports and multiple higher-order complexity triggers whose combination materially exceeds `complex`.
  - `very-complex-with-xsd-imports`: any valid WSDL fixture that requires one or more required external XSD imports shipped with the same fixture set.
- Rationale: The clarified spec chose structural traits over contributor judgment alone, so the taxonomy must be concrete enough to classify fixtures consistently without tying the plan to parser internals.
- Alternatives considered: Leave complexity selection to contributor judgment. Rejected because it would make folder placement inconsistent. Define categories only by file count. Rejected because structural complexity matters more than raw file count for future parser tests.

## Category Precedence Strategy

- Decision: Make taxonomy precedence explicit: invalidity overrides all valid complexity categories, and any valid fixture with required external XSD imports belongs only in `very-complex-wsdls-with-xsd-imports`.
- Rationale: This removes ambiguity for structurally wrong fixtures that also appear complex and for valid fixtures whose service surface is otherwise simple but depends on imported schemas.
- Alternatives considered: Allow contributors to choose the "closest" valid category for import-based fixtures. Rejected because it would create inconsistent placement and weaken path stability for future tests.

## Invalid Fixture Strategy

- Decision: Restrict `invalid-wsdls` to WSDL files that remain recognizable as WSDL content but are intentionally structurally incorrect.
- Rationale: The clarified spec selected structurally wrong WSDLs as the negative-path contract, which gives future tests meaningful WSDL failure cases instead of generic malformed XML. Invalidity also takes precedence over otherwise valid-complexity signals so reviewers have only one placement rule for broken fixtures.
- Alternatives considered: Use malformed XML as the invalid category. Rejected because the user selected structurally wrong WSDL rather than XML-level failure. Include unresolved-import scenarios in the invalid category. Rejected because import-based scenarios have their own explicit valid category.

## Validation Strategy

- Decision: Validate implementation by confirming the final directory tree, checking that every category has at least one scenario folder, verifying that import-based scenarios keep WSDL and XSD files together, and running `dotnet build` to confirm static fixture additions do not introduce collateral issues.
- Rationale: The feature does not add executable code, so validation should emphasize asset completeness and repository health rather than code-path coverage.
- Alternatives considered: Build-only validation. Rejected because it cannot prove the fixture catalog is organized correctly. Asset-tree review only. Rejected because it would not catch unrelated repository breakage introduced during implementation.

## Contract Documentation Strategy

- Decision: Treat the file-system layout of `tests/Integration/wsdl-fixtures` as the feature's contract artifact and document the directory contract in `contracts/wsdl-fixture-directory-contract.md`.
- Rationale: Future tests will depend on stable category and fixture-set paths, so the directory layout itself is the interface this feature exposes to later implementation work. The contract therefore acts as the normative rule source for fixture placement and naming.
- Alternatives considered: Skip contracts because the feature is internal-only. Rejected because the directory structure is still a stable consumer-facing contract for future tests and maintainers.

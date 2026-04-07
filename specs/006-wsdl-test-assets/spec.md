# Feature Specification: WSDL Test Assets

**Feature Branch**: `[006-wsdl-test-assets]`  
**Created**: 2026-04-06  
**Status**: Draft  
**Input**: User description: "Create a new feature to add test WSDL files into the test folder for future testing. Do not add any code or change any other file. Just add WSDL files to test with proper names valid for macbook. Create folders of each of the different type of test data: simplest wsdls, simple wsdls, complex wsdls, very complex wsdls, very complex wsdls with xsd imports, invalid wsdls."

## Clarifications

### Session 2026-04-06

- Q: Where should the new WSDL fixture root live? → A: Under `tests/Integration` in a dedicated WSDL fixture root.
- Q: What should the dedicated WSDL fixture root be named? → A: `wsdl-fixtures`.
- Q: How should the valid complexity categories be defined? → A: By a short set of representative WSDL/XSD structure traits.
- Q: What should count as an invalid WSDL fixture? → A: Structurally wrong WSDL.
- Q: Which rules win when category signals conflict? → A: Invalidity overrides valid complexity, and any valid fixture with required external XSD imports belongs only in `very-complex-wsdls-with-xsd-imports`.
- Q: Are taxonomy definitions stable for this feature? → A: Yes. The six category names and their meanings are frozen for this initial delivery and future tests may depend on them.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Organize Core WSDL Samples (Priority: P1)

As a maintainer, I want the test folder to contain clearly separated WSDL samples by complexity so future test work can start from ready-made fixtures instead of creating ad hoc files.

**Why this priority**: The repository needs a stable baseline set of WSDL assets before any future parser or integration tests can be added efficiently.

**Independent Test**: Can be fully tested by verifying that the requested WSDL category folders exist and each contains at least one appropriately named sample file for its category.

**Acceptance Scenarios**:

1. **Given** the repository integration test area does not yet contain the requested WSDL fixture categories, **When** the feature is completed, **Then** folders exist under `tests/Integration/wsdl-fixtures` for simplest, simple, complex, very complex, very complex with XSD imports, and invalid WSDL samples.
2. **Given** a maintainer needs a representative WSDL fixture, **When** they browse the test assets, **Then** they can identify the correct complexity category without opening unrelated folders.
3. **Given** a contributor needs to decide where a valid WSDL belongs, **When** they compare it to the category definitions, **Then** they can classify it using the documented structural traits for that category.
4. **Given** a contributor encounters a structurally wrong WSDL or a valid WSDL that depends on imported XSD files, **When** they apply the taxonomy rules, **Then** invalidity takes priority over valid complexity and imported valid fixtures are placed only in `very-complex-wsdls-with-xsd-imports`.

---

### User Story 2 - Use Portable File Names (Priority: P2)

As a contributor working on macOS, Windows, or Linux, I want all added WSDL asset names to be clear and path-safe so they can be used directly in future tests and repository operations without renaming.

**Why this priority**: Even good sample coverage becomes costly if file names are inconsistent, ambiguous, or problematic on common development machines.

**Independent Test**: Can be fully tested by reviewing the added folder and file names and confirming they are descriptive, consistent, and free of problematic naming patterns.

**Acceptance Scenarios**:

1. **Given** new WSDL files are added for future testing, **When** a contributor views the asset names, **Then** each name communicates its purpose or complexity and uses a Mac-compatible filename format.
2. **Given** the repository is cloned onto a MacBook, **When** a contributor accesses the new asset paths, **Then** the files can be opened and referenced without path normalization or manual renaming.

---

### User Story 3 - Preserve Negative and Import-Based Fixtures (Priority: P3)

As a test author, I want invalid WSDL samples and import-based WSDL samples separated from the simpler valid samples so I can target failure and dependency scenarios intentionally.

**Why this priority**: Future testing needs more than happy-path fixtures; it also needs clearly isolated data for invalid definitions and multi-file dependency cases.

**Independent Test**: Can be fully tested by confirming that invalid WSDLs are stored in their own category and that import-based fixtures include the dependent schema files needed to represent the scenario.

**Acceptance Scenarios**:

1. **Given** a future test needs a failure fixture, **When** the contributor selects assets from the invalid WSDL folder, **Then** the sample set contains intentionally invalid WSDL files only.
2. **Given** a future test needs a dependency-based fixture, **When** the contributor selects assets from the import-based complexity folder, **Then** the primary WSDL and its required imported schema files are available together.
3. **Given** a contributor needs a negative fixture that should still be recognizable as WSDL content, **When** they browse the invalid category, **Then** the sample set contains WSDL definitions that are intentionally structurally incorrect rather than valid WSDLs or unrelated malformed files.

---

### Edge Cases

- If a sample belongs to multiple complexity interpretations, it should be placed in the single category that best represents its dominant testing purpose rather than duplicated across categories.
- If an import-based WSDL depends on companion schema files, those companion files must remain with that fixture set so the scenario is complete and portable.
- If a WSDL is intentionally malformed, it must be stored only in the invalid category and must not be mixed into any valid-complexity folder.
- If a candidate filename contains spaces, shell-sensitive characters, or visually ambiguous wording, it must be normalized to a clear Mac-compatible name before being added.
- If a valid WSDL could fit more than one valid complexity category, the category with the strongest matching structural traits must be used and the file must not be duplicated across multiple valid categories.
- If a negative fixture is invalid because of broken WSDL structure, it belongs in the invalid category even if the XML itself remains well-formed.
- If a valid WSDL uses required external XSD imports, it belongs in `very-complex-wsdls-with-xsd-imports` even if its service surface would otherwise appear simple or complex.
- If a structurally wrong WSDL also resembles one of the valid complexity levels, it still belongs in `invalid-wsdls`; invalidity overrides all valid taxonomy placement.
- If a fixture-set or file name uses numerals or abbreviations, they are allowed only when they improve first-glance readability and remain lowercase ASCII kebab-case.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The repository test assets MUST include six distinct WSDL categories: simplest WSDLs, simple WSDLs, complex WSDLs, very complex WSDLs, very complex WSDLs with XSD imports, and invalid WSDLs.
- **FR-002**: Each requested category MUST be represented by its own dedicated folder under `tests/Integration/wsdl-fixtures`.
- **FR-003**: Each category folder MUST contain at least one WSDL sample whose content matches the category it is placed in.
- **FR-003a**: The valid categories `simplest`, `simple`, `complex`, and `very-complex` MUST be defined by concise, documented WSDL or XSD structural traits so contributors can classify samples consistently.
- **FR-003b**: The category semantics documented by this feature's directory contract are normative placement rules, not illustrative examples.
- **FR-004**: Added folder names and file names MUST be descriptive, consistent, and valid for normal use on a MacBook filesystem.
- **FR-004a**: Category, fixture-set, WSDL, and companion XSD names MUST use lowercase ASCII kebab-case, may include numerals only when they improve clarity, may use only widely recognized abbreviations, and SHOULD avoid repeating the parent category label unless repetition is required for disambiguation.
- **FR-005**: Valid WSDL samples and intentionally invalid WSDL samples MUST be separated so that invalid fixtures are isolated in the invalid WSDL category.
- **FR-005a**: The invalid WSDL category MUST contain intentionally structurally incorrect WSDL definitions rather than valid WSDLs placed there for convenience.
- **FR-005b**: Invalidity MUST override all valid complexity rules; a structurally wrong WSDL belongs in `invalid-wsdls` even if its remaining traits resemble a valid complexity category.
- **FR-006**: The very complex WSDLs with XSD imports category MUST include the companion imported schema files needed for those fixtures to remain complete as a reusable asset set.
- **FR-006a**: Any valid WSDL fixture that requires one or more external XSD imports MUST be placed only in `very-complex-wsdls-with-xsd-imports`, regardless of whether the non-imported service structure would otherwise qualify as `simplest`, `simple`, `complex`, or `very-complex`.
- **FR-007**: The feature MUST be limited to adding WSDL-related test asset files and folders only, with no source code changes, no production behavior changes, and no unrelated repository modifications.
- **FR-008**: The added asset names MUST allow contributors to infer the scenario purpose or complexity level without opening every file first.
- **FR-009**: The complexity definitions MUST rely on representative schema or service-structure traits rather than contributor judgment alone.
- **FR-009a**: The valid complexity categories MUST use the following placement rules:
	- `simplest`: exactly one `service`, one `binding`, one `portType`, one operation, no faults, no external schema imports, and only primitive or otherwise minimal self-contained schema structure.
	- `simple`: self-contained with no external schema imports and either two to four operations or one to two operations with named message or type structure that is richer than `simplest`, while still using one `service`, one `binding`, and one `portType`.
	- `complex`: self-contained with no external schema imports and at least one clear complexity trigger such as multiple bindings or ports, five or more operations, multiple faults, or several named complex types with nested structure, while not meeting `very-complex` triggers.
	- `very-complex`: self-contained with no external schema imports and multiple higher-order complexity triggers, such as several interacting WSDL parts, several nested named complex types, multiple service-definition layers, or a combination that materially exceeds `complex`.
- **FR-010**: The six category names and their meanings are part of the stable fixture contract for this feature's initial delivery; future tests may depend on them, and any rename or meaning change requires a subsequent approved feature change.

### Key Entities *(include if feature involves data)*

- **WSDL Test Asset**: A sample WSDL file intended to support future repository testing, identified by category, validity, and descriptive filename.
- **Asset Category Folder**: A grouping of WSDL test assets organized by complexity or validity under `tests/Integration/wsdl-fixtures` so contributors can locate the correct fixture set quickly.
- **Complexity Definition**: A short description of the representative WSDL or XSD structural traits that determine whether a valid sample belongs in `simplest`, `simple`, `complex`, or `very-complex`.
- **Category Precedence Rule**: The ordered placement rule stating that `invalid-wsdls` overrides all valid categories and that any valid fixture with required external XSD imports belongs only in `very-complex-wsdls-with-xsd-imports`.
- **Invalid WSDL Fixture**: A WSDL sample that remains recognizable as WSDL content but is intentionally structurally incorrect so future tests can exercise failure handling.
- **Imported Schema Companion**: A schema file stored with an import-based WSDL fixture so the dependency scenario remains intact for later testing work.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All six requested WSDL fixture categories are present under `tests/Integration/wsdl-fixtures` with no category omitted.
- **SC-002**: Each category contains at least one sample set that a reviewer can classify correctly by its folder, filename, and documented category traits alone.
- **SC-002a**: A reviewer using the documented placement rules can explain why each sample belongs in its chosen category without relying on unstated contributor judgment.
- **SC-003**: A contributor can locate a valid sample category or the invalid sample category within 30 seconds of opening the test assets.
- **SC-004**: The repository gains reusable fixtures for both positive and negative future testing scenarios without requiring any code or non-asset file changes.
- **SC-005**: Reviewers can distinguish invalid fixtures from valid ones by the documented rule that invalid samples are structurally incorrect WSDL definitions.

## Assumptions

- The requested "test folder" is the repository's integration test area under `tests/Integration/wsdl-fixtures`, not any production source directory.
- The feature scope is limited to adding new fixture folders and WSDL-related sample files; automated tests, parsers, and documentation changes are out of scope.
- Each requested category needs at least one representative sample set unless later planning expands the quantity.
- "Valid for macbook" is interpreted as using clear, path-safe naming that avoids problematic characters and does not require manual renaming on macOS.
- The specification may define category traits in business-facing terms such as relative structural simplicity, imports, message richness, and schema nesting without introducing parser implementation detail.
- Invalid fixtures are expected to fail because of WSDL structural problems, not because they are unrelated non-WSDL files.
- Any future test code may rely on the current category names and meanings as stable inputs, so taxonomy changes are controlled scope changes rather than casual cleanup.

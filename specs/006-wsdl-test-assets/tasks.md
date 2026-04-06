# Tasks: WSDL Test Assets

**Input**: Design documents from `/specs/006-wsdl-test-assets/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: No new automated test tasks are generated because the feature is limited to static fixture assets. Validation tasks below use fixture-tree review, contract review, quickstart checks, and `dotnet build`.

**Organization**: Tasks are grouped by user story so the core valid fixture catalog, naming portability, and negative/import-based fixture coverage can be delivered and validated in controlled increments.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (e.g., [US1], [US2], [US3])
- Every task includes exact file paths

## Phase 1: Setup (Fixture Root Creation)

**Purpose**: Create the shared integration-fixture directory structure required by all stories

- [X] T001 Create the fixture root and category directories under `tests/Integration/wsdl-fixtures/`, `tests/Integration/wsdl-fixtures/simplest-wsdls/`, `tests/Integration/wsdl-fixtures/simple-wsdls/`, `tests/Integration/wsdl-fixtures/complex-wsdls/`, `tests/Integration/wsdl-fixtures/very-complex-wsdls/`, `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/`, and `tests/Integration/wsdl-fixtures/invalid-wsdls/`

---

## Phase 2: Foundational (Blocking Fixture-Set Skeletons)

**Purpose**: Create the scenario directories and stable paths that all user-story asset files will use

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T002 Create the self-contained valid fixture-set directories `tests/Integration/wsdl-fixtures/simplest-wsdls/echo-service/`, `tests/Integration/wsdl-fixtures/simple-wsdls/customer-lookup/`, `tests/Integration/wsdl-fixtures/complex-wsdls/order-processing/`, and `tests/Integration/wsdl-fixtures/very-complex-wsdls/fulfillment-platform/`
- [X] T003 [P] Create the import-based and invalid fixture-set directories `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/` and `tests/Integration/wsdl-fixtures/invalid-wsdls/broken-binding/`

**Checkpoint**: Stable fixture-set paths exist for all planned categories

---

## Phase 3: User Story 1 - Organize Core WSDL Samples (Priority: P1) 🎯 MVP

**Goal**: Populate the core valid fixture categories with representative self-contained WSDL samples

**Independent Test**: Verify that `simplest-wsdls`, `simple-wsdls`, `complex-wsdls`, and `very-complex-wsdls` each contain one scenario folder with one primary WSDL file whose structure matches the category traits.

### Implementation for User Story 1

- [X] T004 [P] [US1] Add the simplest self-contained WSDL fixture in `tests/Integration/wsdl-fixtures/simplest-wsdls/echo-service/echo-service.wsdl`
- [X] T005 [P] [US1] Add the simple self-contained WSDL fixture in `tests/Integration/wsdl-fixtures/simple-wsdls/customer-lookup/customer-lookup.wsdl`
- [X] T006 [P] [US1] Add the complex self-contained WSDL fixture in `tests/Integration/wsdl-fixtures/complex-wsdls/order-processing/order-processing.wsdl`
- [X] T007 [P] [US1] Add the very complex self-contained WSDL fixture in `tests/Integration/wsdl-fixtures/very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl`
- [X] T008 [US1] Validate the four self-contained valid categories against `specs/006-wsdl-test-assets/quickstart.md` and `specs/006-wsdl-test-assets/contracts/wsdl-fixture-directory-contract.md`

**Checkpoint**: The core valid fixture catalog is present and independently usable for future tests

---

## Phase 4: User Story 2 - Use Portable File Names (Priority: P2)

**Goal**: Ensure all newly added fixture directories and asset file names are descriptive and path-safe across supported developer platforms

**Independent Test**: Review the final fixture tree and confirm all category directories, fixture-set directories, WSDL files, and XSD files use lowercase ASCII kebab-case names that remain descriptive without opening the files.

### Implementation for User Story 2

- [X] T009 [US2] Normalize any non-final category or fixture-set names under `tests/Integration/wsdl-fixtures/` so only the contract paths remain: `simplest-wsdls/echo-service/`, `simple-wsdls/customer-lookup/`, `complex-wsdls/order-processing/`, `very-complex-wsdls/fulfillment-platform/`, `very-complex-wsdls-with-xsd-imports/shipping-network/`, and `invalid-wsdls/broken-binding/`
- [X] T010 [US2] Normalize primary WSDL filenames to the final descriptive ASCII kebab-case names `tests/Integration/wsdl-fixtures/simplest-wsdls/echo-service/echo-service.wsdl`, `tests/Integration/wsdl-fixtures/simple-wsdls/customer-lookup/customer-lookup.wsdl`, `tests/Integration/wsdl-fixtures/complex-wsdls/order-processing/order-processing.wsdl`, `tests/Integration/wsdl-fixtures/very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl`, `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl`, and `tests/Integration/wsdl-fixtures/invalid-wsdls/broken-binding/broken-binding.wsdl`
- [X] T011 [P] [US2] Normalize companion schema filenames to the final descriptive ASCII kebab-case names `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/order-types.xsd` and `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/shared-faults.xsd`
- [X] T012 [US2] Validate naming portability and readability across `tests/Integration/wsdl-fixtures/` using `specs/006-wsdl-test-assets/quickstart.md` and `specs/006-wsdl-test-assets/contracts/wsdl-fixture-directory-contract.md`

**Checkpoint**: All fixture paths are portable, descriptive, and stable for future test references

---

## Phase 5: User Story 3 - Preserve Negative and Import-Based Fixtures (Priority: P3)

**Goal**: Add the specialized fixture sets needed for import-based and invalid-scenario future tests

**Independent Test**: Verify that the import-based category contains one WSDL plus required XSD companions in the same fixture-set directory, and that the invalid category contains a structurally wrong WSDL fixture isolated from valid categories.

### Implementation for User Story 3

- [X] T013 [US3] Add the import-based very complex WSDL fixture in `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/shipping-network.wsdl`
- [X] T014 [P] [US3] Add the imported companion schemas in `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/order-types.xsd` and `tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/shared-faults.xsd`
- [X] T015 [P] [US3] Add the structurally wrong WSDL fixture in `tests/Integration/wsdl-fixtures/invalid-wsdls/broken-binding/broken-binding.wsdl`
- [X] T016 [US3] Validate import completeness and invalid-fixture isolation against `specs/006-wsdl-test-assets/quickstart.md` and `specs/006-wsdl-test-assets/contracts/wsdl-fixture-directory-contract.md`

**Checkpoint**: Negative-path and import-based fixture coverage is available independently of the core valid catalog

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency and repository-health validation across all stories

- [X] T017 [P] Verify that every category under `tests/Integration/wsdl-fixtures/` contains at least one fixture-set directory and no loose unrelated asset files using `specs/006-wsdl-test-assets/quickstart.md`
- [X] T018 [P] Run repository build validation from `XsdXmlParser.csproj` with `dotnet build`
- [X] T019 Verify that the final fixture tree under `tests/Integration/wsdl-fixtures/` matches `specs/006-wsdl-test-assets/plan.md`, `specs/006-wsdl-test-assets/research.md`, and `specs/006-wsdl-test-assets/contracts/wsdl-fixture-directory-contract.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; start immediately
- **Foundational (Phase 2)**: Depends on Phase 1; blocks all user-story work
- **User Story 1 (Phase 3)**: Depends on Phase 2
- **User Story 2 (Phase 4)**: Depends on the relevant story assets existing and should finalize after User Story 1 and User Story 3 create the target files
- **User Story 3 (Phase 5)**: Depends on Phase 2 and can proceed in parallel with User Story 1
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational; no dependency on other user stories
- **User Story 2 (P2)**: Finalizes naming across the full fixture catalog and therefore depends on the target assets from User Story 1 and User Story 3
- **User Story 3 (P3)**: Starts after Foundational; no dependency on User Story 1

### Within Each User Story

- Create directory skeletons before adding files
- Add primary WSDL files before story-level validation
- Add companion XSD files before validating import-based completeness
- Normalize naming after the target asset files exist

### Parallel Opportunities

- T003 can run in parallel with the final valid-scenario directory preparation after T002 starts the fixture-set structure
- T004, T005, T006, and T007 can run in parallel once Phase 2 completes
- T011 can run in parallel with T010 once the import-based files exist
- T014 and T015 can run in parallel after T013 defines the import-based and invalid-scenario targets
- T017 and T018 can run in parallel during final validation

---

## Parallel Example: User Story 1

```bash
# Launch the self-contained valid fixtures together after the directory skeleton exists:
Task: "Add the simplest self-contained WSDL fixture in tests/Integration/wsdl-fixtures/simplest-wsdls/echo-service/echo-service.wsdl"
Task: "Add the simple self-contained WSDL fixture in tests/Integration/wsdl-fixtures/simple-wsdls/customer-lookup/customer-lookup.wsdl"
Task: "Add the complex self-contained WSDL fixture in tests/Integration/wsdl-fixtures/complex-wsdls/order-processing/order-processing.wsdl"
Task: "Add the very complex self-contained WSDL fixture in tests/Integration/wsdl-fixtures/very-complex-wsdls/fulfillment-platform/fulfillment-platform.wsdl"
```

---

## Parallel Example: User Story 2

```bash
# Normalize import-based companion naming while primary WSDL naming is being finalized:
Task: "Normalize primary WSDL filenames to the final descriptive ASCII kebab-case names under tests/Integration/wsdl-fixtures/"
Task: "Normalize companion schema filenames to the final descriptive ASCII kebab-case names in tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/"
```

---

## Parallel Example: User Story 3

```bash
# Create import companions and the invalid fixture together after the scenario directories exist:
Task: "Add the imported companion schemas in tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/order-types.xsd and tests/Integration/wsdl-fixtures/very-complex-wsdls-with-xsd-imports/shipping-network/shared-faults.xsd"
Task: "Add the structurally wrong WSDL fixture in tests/Integration/wsdl-fixtures/invalid-wsdls/broken-binding/broken-binding.wsdl"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate the core valid fixture catalog with T008 and T018

### Incremental Delivery

1. Finish Setup + Foundational to lock the final fixture paths
2. Deliver User Story 1 for the core self-contained valid categories
3. Deliver User Story 3 for import-based and invalid categories
4. Deliver User Story 2 to normalize and verify portable naming across the whole catalog
5. Finish with Phase 6 repository-health and consistency validation

### Parallel Team Strategy

1. One developer creates the shared directory skeleton in Phase 1 and Phase 2
2. After Foundational completes:
   - Developer A: User Story 1 valid self-contained fixtures
   - Developer B: User Story 3 import-based and invalid fixtures
3. A final pass completes User Story 2 naming normalization and Phase 6 validation

---

## Notes

- `[P]` tasks touch different files or directories and can be coordinated in parallel
- User-story tasks stay within the feature's asset-only scope and do not require source-code changes
- Validation is performed through directory review, contract review, quickstart checks, and `dotnet build` instead of new automated tests
- Future tests should be able to treat each fixture-set directory as a stable scenario path
# Quickstart: WSDL Test Assets

## Purpose

Validate the planned fixture catalog layout and confirm that the repository contains reusable WSDL scenarios without any production-code changes.

## 1. Confirm The Fixture Root Exists

Expected root:

```text
tests/Integration/wsdl-fixtures/
```

Success means the feature adds a dedicated integration fixture catalog rather than scattering WSDL files across unrelated test folders.

## 2. Confirm The Six Required Categories Exist

Expected category directories:

```text
tests/Integration/wsdl-fixtures/
├── simplest-wsdls/
├── simple-wsdls/
├── complex-wsdls/
├── very-complex-wsdls/
├── very-complex-wsdls-with-xsd-imports/
└── invalid-wsdls/
```

Success means every requested category is present with no omissions or renamed variants.

## 3. Confirm Each Category Has At Least One Fixture Set

For each category, verify there is at least one scenario-specific subfolder such as:

```text
tests/Integration/wsdl-fixtures/simple-wsdls/<fixture-set-name>/
```

Success means future tests can target isolated scenario directories instead of relying on loose files.

## 4. Confirm Naming Rules

Review the new folder and file names and verify that they:

- Use lowercase ASCII kebab-case.
- Avoid spaces and shell-sensitive punctuation.
- End WSDL files with `.wsdl` and companion schema files with `.xsd`.
- Communicate fixture purpose or complexity without opening the file first.

## 5. Confirm Category Semantics

Verify the fixture sets align with these planning rules:

- `simplest-wsdls`: one-operation, minimal self-contained WSDL scenario.
- `simple-wsdls`: self-contained WSDL with modestly richer message or type structure.
- `complex-wsdls`: self-contained WSDL with richer inline schema or broader service structure.
- `very-complex-wsdls`: dense self-contained WSDL with several interacting WSDL parts and no external XSD imports.
- `very-complex-wsdls-with-xsd-imports`: very complex WSDL scenario that requires colocated imported XSD files.
- `invalid-wsdls`: structurally wrong WSDL definitions, not valid WSDLs or unrelated malformed files.

## 6. Confirm Import-Based Fixture Completeness

Inspect at least one fixture set under `very-complex-wsdls-with-xsd-imports` and verify that the primary WSDL file and all required imported XSD files live in the same fixture-set directory.

## 7. Confirm Repository Health

Run the standard repository build validation:

```bash
dotnet build
```

Success means the static fixture additions did not introduce unrelated repository breakage.

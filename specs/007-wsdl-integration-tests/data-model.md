# Data Model: WSDL Fixture Integration Coverage

## FixtureCatalogModel

- Purpose: Represents the full WSDL fixture inventory that the feature must cover.
- Fields:
  - `Categories`: collection of fixture categories included in scope.
  - `StandaloneWsdlCount`: total number of `.wsdl` files that must map to test methods.
  - `ScenarioSetCount`: total number of multi-document fixture networks that require scenario-level coverage.
- Relationships:
  - Contains many `FixtureCategoryModel` instances.
  - Aggregates many `FixtureCoverageMapModel` entries.
- Validation rules:
  - Must include every WSDL enumerated in `FR-001a`.
  - Must not omit `download-web` even though it is not one of the original six taxonomy folders.

## FixtureCategoryModel

- Purpose: Groups fixtures by repository folder or semantic coverage family.
- Fields:
  - `CategoryName`: repository-visible category name such as `simplest-wsdls` or `invalid-wsdls`.
  - `CoverageClassName`: planned integration test class or group name.
  - `AssertionProfileKind`: baseline plus category-specific assertion profile.
  - `IsFailureCategory`: whether fixtures in the category are expected to fail.
- Relationships:
  - Contains many `FixtureSetModel` or direct `FixtureDocumentModel` entries.
  - Is referenced by `FixtureIntegrationTestClassModel`.
- Validation rules:
  - Valid categories must use success-oriented baseline assertions.
  - Failure categories must use stable failure-category assertions.

## FixtureSetModel

- Purpose: Represents a scenario-specific fixture folder containing one or more related WSDL and XSD files.
- Fields:
  - `SetName`: fixture-set folder name.
  - `CategoryName`: owning category.
  - `PrimaryEntryPoint`: designated scenario-level WSDL entry point when the set is multi-document.
  - `HasImportedSchemas`: whether the set requires companion `.xsd` files.
  - `HasScenarioLevelTest`: whether the set requires a dedicated network test.
- Relationships:
  - Contains many `FixtureDocumentModel` entries.
  - May be owned by one `FixtureCategoryModel`.
  - Maps to one or more `ScenarioLevelNetworkTestModel` entries.
- Validation rules:
  - Multi-document sets must identify `PrimaryEntryPoint` explicitly.
  - Import-based sets must include their dependent XSD files in scenario validation.

## FixtureDocumentModel

- Purpose: Represents one WSDL file that must map to a clearly named integration test method.
- Fields:
  - `RelativePath`: repository-relative WSDL path under `tests/Integration/wsdl-fixtures`.
  - `FileName`: exact WSDL file name, preserving current casing.
  - `DocumentRole`: one of `Standalone`, `ScenarioEntryPoint`, `SupportingStandalone`, or `ScenarioOnlySupport`.
  - `ExpectedOutcome`: `Success` or `Failure`.
  - `BaselineRequired`: whether success baseline assertions apply.
- Relationships:
  - Belongs to one `FixtureSetModel` or directly to one `FixtureCategoryModel`.
  - Maps to one `FixtureTestMethodModel`.
- Validation rules:
  - Every in-scope WSDL file must have exactly one primary mapping to a fixture test method.
  - Success documents must use the baseline assertion set.
  - Failure documents must assert stable failure classification.

## FixtureIntegrationTestClassModel

- Purpose: Describes a grouped integration test class organized by fixture category or fixture set.
- Fields:
  - `ClassName`: planned class name.
  - `GroupingStrategy`: `Category` or `FixtureSet`.
  - `FixtureScope`: the folder scope covered by the class.
  - `UsesBaseClass`: whether the class derives from a shared integration base class.
  - `RequiresXmlDocumentation`: always `true` for this feature.
- Relationships:
  - Owns many `FixtureTestMethodModel` entries.
  - May inherit from one `SharedIntegrationTestBaseClassModel`.
- Validation rules:
  - Must not collapse multiple WSDL files into one test method.
  - Must keep fixture-specific assertions visible.

## FixtureTestMethodModel

- Purpose: Represents one clearly named integration test method mapped to one WSDL file.
- Fields:
  - `MethodName`: test method name that exposes the WSDL identity or scenario.
  - `TargetWsdlPath`: exact mapped WSDL path.
  - `ExpectedOutcome`: `Success` or `Failure`.
  - `AssertionProfile`: baseline-only, baseline-plus-category, baseline-plus-fixture, failure-classification, or scenario-level-network.
  - `RequiresXmlDocumentation`: always `true` for this feature.
- Relationships:
  - Belongs to one `FixtureIntegrationTestClassModel`.
  - References one `FixtureDocumentModel`.
- Validation rules:
  - One method maps to one WSDL file.
  - Success methods must include baseline assertions.
  - Failure methods must assert stable failure classification.

## SharedIntegrationTestBaseClassModel

- Purpose: Captures reusable test-side setup or helper behavior that reduces duplication across grouped fixture classes.
- Fields:
  - `ClassName`: planned base class name.
  - `SharedResponsibilities`: fixture-path resolution, DI container creation, parse orchestration, shared assertion helpers, or diagnostic helpers.
  - `DerivedClassCount`: number of grouped classes expected to reuse it.
  - `RequiresXmlDocumentation`: always `true` for this feature.
- Relationships:
  - Is referenced by many `FixtureIntegrationTestClassModel` entries.
- Validation rules:
  - Must not hide category- or fixture-specific assertions.
  - Should exist only when repeated orchestration would otherwise be duplicated.

## BaselineAssertionProfileModel

- Purpose: Defines the success assertions that every valid WSDL fixture must satisfy.
- Fields:
  - `RequiresSuccessfulParse`: always `true`.
  - `RequiresNonEmptyResult`: always `true`.
  - `RequiresMeaningfulArtifact`: always `true`.
  - `MeaningfulArtifactExamples`: service, binding, port type, message, schema element, schema type, or equivalent metadata.
- Relationships:
  - Used by many `FixtureTestMethodModel` entries for valid fixtures.
- Validation rules:
  - Must apply to every valid fixture consistently.
  - Must remain smaller in scope than category-specific assertions.

## FailureAssertionProfileModel

- Purpose: Defines the invalid-fixture assertions for stable failure validation.
- Fields:
  - `RequiresFailure`: always `true`.
  - `RequiresStableFailureCategory`: always `true`.
  - `AllowsMessageVariability`: always `true`.
  - `ClassificationSignals`: exception type, failure stage, or diagnostic code family.
- Relationships:
  - Used by failure-oriented `FixtureTestMethodModel` entries.
- Validation rules:
  - Must reject false-positive success.
  - Must not rely on exact full diagnostic payloads.

## ScenarioLevelNetworkTestModel

- Purpose: Represents an integration test that validates a multi-document WSDL/XSD network through its designated entry point.
- Fields:
  - `ScenarioName`: network scenario name.
  - `EntryPointWsdl`: designated entry-point WSDL path.
  - `SupportingWsdls`: related WSDL dependencies included in the scenario.
  - `SupportingXsds`: required imported schema files.
  - `ExpectedOutcome`: usually `Success`, unless a future network is intentionally invalid.
- Relationships:
  - Belongs to one `FixtureSetModel`.
  - May coexist with standalone `FixtureTestMethodModel` entries for valid supporting WSDLs.
- Validation rules:
  - Must identify the entry point explicitly.
  - Must prove dependency resolution across the full fixture set.
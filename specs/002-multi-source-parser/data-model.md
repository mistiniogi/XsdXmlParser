# Data Model: Multi-Source XSD/WSDL Parser Library

## Entities

### SourceDescriptorModel
- Purpose: Represents one supplied input source.
- Fields:
  - `SourceId`: Stable logical identifier for the source.
  - `SourceKind`: Enum describing file path, stream, memory buffer, or batch stream.
  - `DisplayName`: Consumer-facing name for diagnostics.
  - `VirtualPath`: Stable logical path used by the virtual file system.
  - `RelativePath`: Optional logical path used for import/include resolution.
  - `IsMainSource`: Indicates whether this source is the designated main file.
  - `ContentFingerprint`: Optional stable fingerprint for duplicate detection.
- Relationships:
  - Belongs to one parse request.
  - Can import or include other `SourceDescriptorModel` entries.

### MetadataGraphModel
- Purpose: Top-level normalized graph returned by the parser.
- Fields:
  - `Sources`: Dictionary keyed by `SourceId`.
  - `ComplexTypes`: Dictionary keyed by `RefId`.
  - `SimpleTypes`: Dictionary keyed by `RefId`.
  - `Elements`: Dictionary keyed by `RefId`.
  - `Attributes`: Dictionary keyed by `RefId`.
  - `Relationships`: Dictionary keyed by `RefId` or relationship ID.
  - `ValidationRules`: Dictionary keyed by rule ID.
  - `RootRefIds`: Entry-point references for consumers.
  - `SerializerHints`: Optional metadata describing JSON converter expectations for consumers.
- Relationships:
  - Owns all canonical dictionaries and exported references.

### VirtualFileModel
- Purpose: Represents a source as resolved through `IVirtualFileSystem`.
- Fields:
  - `VirtualPath`: Normalized path used for import/include resolution.
  - `SourceId`: Back-reference to the source descriptor.
  - `IsMemoryBacked`: Indicates whether content originates from memory rather than disk.
  - `ContentLength`: Optional size metadata for diagnostics.

### SchemaRegistryEntryModel
- Purpose: Represents a registry-owned canonical definition tracked by `SchemaRegistryService`.
- Fields:
  - `RefId`: Stable canonical identifier.
  - `EntryKind`: ComplexType, SimpleType, Element, Attribute, or auxiliary graph node.
  - `OwningSourceId`: Source that introduced the canonical definition.
  - `DiscoveryState`: Discovered, canonicalized, linked, or conflicted.
  - `ConflictHash`: Optional hash used to compare duplicate candidates.

### RegistryEntryModel
- Purpose: Shared base shape for canonical graph entries.
- Fields:
  - `RefId`: Stable canonical reference identifier.
  - `SourceId`: Owning logical source.
  - `QualifiedName`: Qualified schema name when present.
  - `SchemaPath`: Canonical schema path for diagnostics.
  - `ParentRefId`: Parent entry when the definition is local or anonymous.
  - `Documentation`: Optional annotation summary.

### ComplexTypeModel
- Purpose: Canonical representation of a complex type.
- Fields:
  - Inherits base registry fields.
  - `BaseTypeRefId`: Optional base type reference.
  - `ChildMemberRefIds`: Ordered child-member references.
  - `AttributeRefIds`: Attached attribute references.
  - `CompositorHints`: Flattened sequence/choice hints for UI and validation.
  - `IsAnonymous`: Indicates whether the type is anonymous.

### SimpleTypeModel
- Purpose: Canonical representation of a simple type.
- Fields:
  - Inherits base registry fields.
  - `BaseTypeRefId`: Optional inherited type reference.
  - `RestrictionRuleIds`: Validation rule references.
  - `EnumerationValues`: Optional allowed values.

### ElementModel
- Purpose: Canonical representation of an element.
- Fields:
  - Inherits base registry fields.
  - `TypeRefId`: Referenced simple or complex type.
  - `MinOccurs`: Minimum occurrence.
  - `MaxOccurs`: Maximum occurrence or unbounded marker.
  - `OrderIndex`: UI/validation ordering hint.
  - `ChoiceGroupKey`: Optional grouping hint for flattened choice semantics.

### AttributeModel
- Purpose: Canonical representation of an attribute.
- Fields:
  - Inherits base registry fields.
  - `TypeRefId`: Referenced simple type.
  - `UseKind`: Required, optional, or prohibited.
  - `DefaultValue`: Optional default.
  - `FixedValue`: Optional fixed value.

### RelationshipModel
- Purpose: Represents a normalized link between registry entries.
- Fields:
  - `RelationshipId`: Stable identifier.
  - `FromRefId`: Source entry reference.
  - `ToRefId`: Target entry reference.
  - `RelationshipKind`: Import, include, contains, references, derives-from, or attribute-of.
  - `OrderIndex`: Optional ordering metadata.
  - `PassAssigned`: Discovery or linkage pass in which the relationship becomes final.

### ConstraintSetModel
- Purpose: Stores validation and generation-oriented rules.
- Fields:
  - `RuleId`: Stable identifier.
  - `OwnerRefId`: Entry this rule belongs to.
  - `MinOccurs`: Optional occurrence minimum.
  - `MaxOccurs`: Optional occurrence maximum.
  - `Pattern`: Optional regex-like pattern.
  - `BaseTypeRefId`: Optional lineage reference.
  - `ValueBounds`: Optional numeric or lexical bounds.
  - `EnumerationValues`: Optional enumerated values.
  - `SerializerShape`: Optional hint for custom JSON conversion.

### ParseDiagnosticModel
- Purpose: Represents one actionable parse or resolution failure surfaced to consumers.
- Fields:
  - `DiagnosticId`: Stable identifier for the diagnostic record.
  - `SourceId`: Optional logical source identity associated with the failure.
  - `VirtualPath`: Optional logical path associated with the failure.
  - `Stage`: Source loading, discovery, linking, serialization, or validation stage.
  - `Code`: Stable error code for unresolved import, invalid main source, unreadable source, empty source, invalid schema, or duplicate conflict.
  - `Message`: Consumer-facing actionable error message.
  - `Details`: Optional extended context for candidate roots, conflicting definitions, or nested parser exceptions.

## State Transitions

### Source Resolution Lifecycle
1. `Registered`: Source descriptor is accepted into the parse request.
2. `Resolving`: Imports/includes are being traversed.
3. `Resolved`: Source content and references have been normalized.
4. `Failed`: Source resolution terminated with an actionable `ParseDiagnosticModel`.

### Registry Entry Lifecycle
1. `Discovered`: Entry identified during parsing.
2. `Canonicalized`: Entry matched to an existing canonical definition or assigned a new RefId.
3. `Linked`: Relationships and rules attached through RefIds.
4. `Exported`: Entry serialized as part of the metadata graph.

### Two-Pass Parsing Lifecycle
1. `DiscoveryPass`: `WsdlDiscoveryService` and `XsdGraphBuilder` register reachable sources and canonical definition shells in `SchemaRegistryService`.
2. `LinkagePass`: `GraphLinkingService` assigns references, inheritance, occurrence rules, and flattened choice/sequence grouping metadata.
3. `SerializationPass`: `MetadataGraphJsonSerializer` emits the normalized graph with `System.Text.Json` and targeted converters.

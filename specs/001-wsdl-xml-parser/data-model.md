# Data Model: WSDL XML Parser

## Entities

### SchemaModel
Represents a parsed WSDL or XSD schema.

**Fields**:
- Id: string (unique identifier)
- Name: string
- Namespace: string
- Elements: List<ElementModel>
- Types: Dictionary<string, TypeModel>
- Imports: List<string> (referenced schemas)

**Validation Rules**:
- Id required
- Name required
- Namespace valid URI format

**Relationships**:
- Has many Elements
- Has many Types
- References other Schemas (imports)

### ElementModel
Represents an XML element in the schema.

**Fields**:
- Name: string (required)
- Type: string (reference to TypeModel)
- MinOccurs: int (default 1)
- MaxOccurs: int (default 1, -1 for unbounded)
- Attributes: List<AttributeModel>
- Children: List<ElementModel> (for complex types)

**Validation Rules**:
- Name required
- Type exists in registry
- MinOccurs >= 0
- MaxOccurs >= MinOccurs or -1

**Relationships**:
- Belongs to SchemaModel
- Has many Attributes
- Has many Children (recursive)

### TypeModel
Represents a data type definition.

**Fields**:
- Name: string (required)
- BaseType: string (optional inheritance)
- IsSimple: bool
- IsComplex: bool
- Facets: List<string> (restrictions)

**Validation Rules**:
- Name required
- BaseType valid if present

**Relationships**:
- Referenced by Elements

### JSONMetadata
Output model for JSON representation.

**Fields**:
- Schema: SchemaModel
- Metadata: Dictionary<string, object>
- GeneratedAt: DateTime

**Validation Rules**:
- Schema required

### XMLOutput
Generated XML structure.

**Fields**:
- RootElement: ElementModel
- Content: string (XML string)
- ValidationErrors: List<string>

**Validation Rules**:
- RootElement required

## State Transitions

### Parsing States
1. Loading: Read WSDL/XSD content
2. Parsing: Build AST/graph
3. Validation: Check schema validity
4. Generation: Produce JSON/XML

### Graph Traversal States
- Visiting: Processing node
- Visited: Completed processing
- Cycle Detected: Handle recursion
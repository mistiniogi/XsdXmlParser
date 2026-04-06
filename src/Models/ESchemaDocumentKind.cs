namespace XsdXmlParser.Core.Models;

/// <summary>
/// Identifies the caller-declared schema document kind.
/// </summary>
public enum ESchemaDocumentKind
{
    /// <summary>
    /// The source contains a WSDL document.
    /// </summary>
    Wsdl,

    /// <summary>
    /// The source contains an XSD document.
    /// </summary>
    Xsd,
}
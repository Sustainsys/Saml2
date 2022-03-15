using System.Xml;

namespace Sustainsys.Saml2.Metadata.Xml;

/// <summary>
/// Traverser to navigate an Xml document. This is essentially a replacement for the XmlNodeReader
/// with the difference that this keeps access to the underlying XmlDocument available. When handling
/// data that contains XmlSignatures it is necessary to be able to read parts of the documents and look up
/// references to the signed elements. That is not possible with a simple forward-only XmlReader. The
/// EnvelopedSignatureReader of the Microsoft.IdentityModel.Xml library is just too complex and error
/// prone in my opinion, I prefer using the SignedXml implementation for signature handling.
/// </summary>
public class XmlTraverser
{
    private XmlNode currentNode;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="rootNode">Root node for this traverser</param>
    public XmlTraverser(XmlNode rootNode)
    {
        currentNode = rootNode;
    }

    /// <summary>
    /// Ensure that the current node has a specific localName and namespace.
    /// </summary>
    /// <param name="namespaceUri">Expected Namespace uri</param>
    /// <param name="localName">Expected local name</param>
    /// <exception cref="Saml2XmlException">Thrown if names do not match</exception>
    public void EnsureName(string namespaceUri, string localName)
    {
        if (currentNode.Name != localName)
        {
            throw new Saml2XmlException($"Unexpected node name \"{currentNode.LocalName}\", expected \"{localName}\"");
        }

        if (currentNode.NamespaceURI != namespaceUri)
        {
            throw new Saml2XmlException($"Unexpected namespace \"{currentNode.NamespaceURI}\" for local name \"{currentNode.Name}\", expected \"{namespaceUri}\"");
        }
    }

    /// <summary>
    /// Get attribute value with specified <paramref name="localName"/> and where there is no namespace
    /// qualifier on the attribute.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Attribute value, null if none.</returns>
    public string? GetAttribute(string localName)
        => currentNode.Attributes?.GetNamedItem(localName)?.Value;

    /// <summary>
    /// Get required attribute value with specified <paramref name="localName"/> and where there is no namespace
    /// qualifier on the attribute.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Attribute value</returns>
    /// <exception cref="Saml2XmlException">If no such attribute is found.</exception>
    public string GetRequiredAttribute(string localName)
    {
        return GetAttribute(localName)
            ?? throw new Saml2XmlException($"Required attribute {localName} not found on {currentNode.Name}");
    }

    /// <summary>
    /// Gets an attribute as timespan.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed Timespan</returns>
    public TimeSpan? GetTimeSpanAttribute(string localName)
    {
        var str = GetAttribute(localName);

        if (str == null) return null;

        return XmlConvert.ToTimeSpan(str);
    }

    /// <summary>
    /// Gets an attribute as a DateTime
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime</returns>
    public DateTime? GetDateTimeAttribute(string localName)
    {
        var str = GetAttribute(localName);

        if (str == null) return null;

        return XmlConvert.ToDateTime(str, XmlDateTimeSerializationMode.RoundtripKind);
    }


}

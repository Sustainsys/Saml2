using System.Xml;

namespace Sustainsys.Saml2.Metadata.Xml;

public class XmlTraverser
{
    private XmlNode currentNode;

    public XmlTraverser(XmlNode rootNode)
    {
        currentNode = rootNode;
    }

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
    /// Get attribute with specified <paramref name="localName"/> and where the attribute is in the same 
    /// namespace as the containing element.
    /// </summary>
    /// <param name="localName"></param>
    /// <returns></returns>
    public string? GetAttribute(string localName)
        => currentNode.Attributes?.GetNamedItem(localName)?.Value;

    public string GetRequiredAttribute(string localName)
    {
        return GetAttribute(localName)
            ?? throw new Saml2XmlException($"Required attribute {localName} not found on {currentNode.Name}");
    }

    public TimeSpan? GetTimeSpanAttribute(string localName)
    {
        var str = GetAttribute(localName);

        if (str == null) return null;

        return XmlConvert.ToTimeSpan(str);
    }

    public DateTime? GetDateTimeAttribute(string localName)
    {
        var str = GetAttribute(localName);

        if (str == null) return null;

        return XmlConvert.ToDateTime(str, XmlDateTimeSerializationMode.RoundtripKind);
    }


}

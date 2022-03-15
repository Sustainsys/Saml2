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
    /// <summary>
    /// Errors encountered so far during the traversal.
    /// </summary>
    public List<Error> Errors = new();

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
    /// Throws exception if the error collection is non-empty.
    /// </summary>
    public void ThrowOnErrors()
    {
        if(Errors.Any(e => !e.Ignore))
        {
            throw new Saml2XmlException(Errors);
        }
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
            Errors.Add(new Error(
                ErrorReason.UnexpectedLocalName,
                localName,
                currentNode, 
                $"Unexpected node name \"{currentNode.LocalName}\", expected \"{localName}\"."));
        }

        if (currentNode.NamespaceURI != namespaceUri)
        {
            Errors.Add(new Error(
                ErrorReason.UnexpectedNamespace,
                localName,
                currentNode,
                $"Unexpected namespace \"{currentNode.NamespaceURI}\" for local name \"{currentNode.Name}\", expected \"{namespaceUri}\"."));
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
    public string? GetRequiredAttribute(string localName)
    {
        var value = GetAttribute(localName);

        if (value == null)
        {
            Errors.Add(new Error(
                ErrorReason.MissingAttribute,
                localName,
                currentNode,
                $"Required attribute {localName} not found on {currentNode.Name}."));
        }
        
        return value;
    }

    /// <summary>
    /// Gets a string attribute and validates that the value is an absolute URI.
    /// Note that even if the validation fails, the value is still returned to
    /// make it possible for consumers to supress the errors.
    /// </summary>
    /// <param name="localName"></param>
    /// <returns></returns>
    public string? GetRequiredAbsoluteUriAttribute(string localName)
    {
        var value = GetRequiredAttribute(localName);

        if (value != null && !Uri.TryCreate(value, UriKind.Absolute, out var _))
        {
            Errors.Add(new Error(
                ErrorReason.NotAbsoluteUri,
                localName,
                currentNode,
                $"Attribute \"{localName}\" should be an absolute Uri, but \"{value}\" isn't")
            {
                StringValue = value
            });
        }

        return value;
    }

    /// <summary>
    /// Gets an attribute as timespan. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed Timespan or null if parse fails</returns>
    public TimeSpan? GetTimeSpanAttribute(string localName)
    {
        return TryGetAttribute(localName, XmlConvert.ToTimeSpan);
    }

    /// <summary>
    /// Gets an attribute as DateTime. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime or null if parse fails</returns>
    public DateTime? GetDateTimeAttribute(string localName)
    {
        return TryGetAttribute(localName, s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind));
    }

    private Nullable<TTarget> TryGetAttribute<TTarget>(string localName, Func<string, TTarget> converter)
        where TTarget: struct
    {
        var source = GetAttribute(localName);
        
        if (source == null)
            return default;

        try
        {
            return converter(source);
        }
        catch(FormatException)
        {
            Errors.Add(new Error(
                ErrorReason.ConversionFailed,
                localName,
                currentNode,
                $"Conversion to {typeof(TTarget).Name} failed for {source}")
                {
                    StringValue = source
                });
        }

        return default;
    }
}

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
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
[DebuggerDisplay("{CurrentNode}")]
public class XmlTraverser
{
    /// <summary>
    /// Errors encountered so far during the traversal.
    /// </summary>
    public List<Error> Errors { get; } = new();

    private XmlNode? currentNode;
    private XmlNode? parentNode;
    private XmlNode? firstChild;

    /// <summary>
    /// The current node being processed.
    /// </summary>
    public XmlNode CurrentNode
    {
        get
        {
            if (currentNode == null)
            {
                throw new InvalidOperationException("There is no current node");
            }
            return currentNode;
        }
    }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="rootNode">Root node for this traverser</param>
    public XmlTraverser(XmlNode rootNode)
    {
        currentNode = rootNode;
    }

    private void AddError(ErrorReason reason, string message)
    {
        Errors.Add(new(reason, CurrentNode.LocalName, CurrentNode, message));
    }

    /// <summary>
    /// Throws exception if the error collection is non-empty.
    /// </summary>
    public void ThrowOnErrors()
    {
        if (Errors.Any(e => !e.Ignore))
        {
            throw new Saml2XmlException(Errors);
        }
    }

    private class ChildScope : IDisposable
    {
        bool isDisposed = false;
        private XmlTraverser xmlTraverser;
        private XmlNode? previousParentNode;

        public ChildScope(XmlTraverser xmlTraverser)
        {
            this.xmlTraverser = xmlTraverser;
            previousParentNode = xmlTraverser.parentNode;
            xmlTraverser.parentNode = xmlTraverser.CurrentNode;
            xmlTraverser.firstChild = xmlTraverser.CurrentNode.FirstChild;

            // We've entered child level, current node cannot be stuck on now-parent.
            xmlTraverser.currentNode = null;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                throw new InvalidOperationException();
            }

            // Detect if we are leaving unprocessed child elements.
            if(xmlTraverser.currentNode != null)
            {
                xmlTraverser.Errors.Add(new(
                    ErrorReason.ExtraElements,
                    xmlTraverser.currentNode.LocalName,
                    xmlTraverser.currentNode,
                    $"Unexpected child element {xmlTraverser.currentNode.LocalName} found, all elements should be processed or explicitly skipped."));
            }

            xmlTraverser.currentNode = xmlTraverser.parentNode;
            xmlTraverser.parentNode = previousParentNode;
            isDisposed = true;
        }
    }

    /// <summary>
    /// Steps down the traverser to the children of the current node. The traverser
    /// steps back up when the returned scope is disposed.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IDisposable EnterChildLevel()
    {
        // Create scope that captures current level and moves to child level
        return new ChildScope(this);
    }

    /// <summary>
    /// If the current node is a signature node, read and validate it and 
    /// </summary>
    /// <param name="trustedSigningKeys">Signing keys trusted when validating the signature.</param>
    /// <param name="allowedHashAlgorithms">Allowed hash algorithms.</param>
    /// <param name="trustLevel">Trust level outcome. None if no signature was processed.</param>
    /// <returns>True if there was a signature node.</returns>
    public bool ReadAndValidateOptionalSignature(
        IEnumerable<SigningKey>? trustedSigningKeys,
        IEnumerable<string>? allowedHashAlgorithms,
        out TrustLevel trustLevel)
    {
        trustLevel = TrustLevel.None;

        if (CurrentNode.LocalName == "Signature" 
            && CurrentNode.NamespaceURI == SignedXml.XmlDsigNamespaceUrl)
        {
            if (trustedSigningKeys != null
                && trustedSigningKeys.Any())
            {
                trustLevel = ReadAndValidateSignature(trustedSigningKeys, allowedHashAlgorithms);
            }

            return true;
        }

        return false;
    }

    // Private worker method, assumes we've already validated current node is a signature.
    private TrustLevel ReadAndValidateSignature(
        IEnumerable<SigningKey> trustedSigningKeys,
        IEnumerable<string>? allowedHashAlgorithms)
    {
        ArgumentNullException.ThrowIfNull(allowedHashAlgorithms);

        var (error, workingKey) = ((XmlElement)CurrentNode)
            .VerifySignature(trustedSigningKeys, allowedHashAlgorithms);

        if(!string.IsNullOrEmpty(error))
        {
            AddError(ErrorReason.SignatureFailure, error);
        }

        return workingKey?.TrustLevel ?? TrustLevel.None;
    }

    /// <summary>
    /// Moves to the next child node in the current collection, if one is available.
    /// </summary>
    /// <returns>true if the move was successful</returns>
    public bool MoveToNextChild()
    {
        while (true)
        {
            // First check if we are fresh into child level, then use firstChild to seed. Otherwise
            // just traverse forward one step (if possible)
            currentNode = firstChild ?? currentNode?.NextSibling;
            firstChild = null;

            if (currentNode == null)
            {
                // No more children.
                return false;
            }

            if (currentNode.NodeType == XmlNodeType.Element)
            {
                // We're happy, we found an element.
                return true;
            }

            // We are ok to skip over white space and comments , but anything else is an error.
            if (currentNode.NodeType != XmlNodeType.Whitespace && currentNode.NodeType != XmlNodeType.Comment)
            {
                AddError(ErrorReason.UnsupportedNodeType, $"Unsupported node type {currentNode.NodeType}");
            }
        }
    }

    /// <summary>
    /// Ignore the rest of the child nodes on this level.
    /// </summary>
    public void SkipChildren()
    {
        currentNode = null;
    }

    /// <summary>
    /// Move to next child if the current collection, record an error if none is available.
    /// </summary>
    /// <returns>ture if the move was successful</returns>
    public bool MoveToNextRequiredChild()
    {
        var result = MoveToNextChild();

        if (!result)
        {
            Errors.Add(new(
                ErrorReason.MissingElement,
                parentNode!.LocalName,
                parentNode,
                $"There should be a child element here under {parentNode!.LocalName}, but found none."));
        }

        return result;
    }

    /// <summary>
    /// Ensures that the node has the specific namespace.
    /// </summary>
    /// <param name="namespaceUri">Expected Namespace uri.</param>
    /// <returns>True if ok</returns>
    public bool EnsureNamespace(string namespaceUri)
    {
        if (CurrentNode.NamespaceURI != namespaceUri)
        {
            AddError(
                ErrorReason.UnexpectedNamespace,
                $"Unexpected namespace \"{CurrentNode.NamespaceURI}\" for local name \"{CurrentNode.Name}\", expected \"{namespaceUri}\".");

            return false;
        }
        return true;
    }

    /// <summary>
    /// Ensure that the current node has a specific localName and namespace.
    /// </summary>
    /// <param name="namespaceUri">Expected Namespace uri</param>
    /// <param name="localName">Expected local name</param>
    /// <returns>True if both are ok</returns>
    public bool EnsureName(string namespaceUri, string localName)
    {
        var namespaceOk = EnsureNamespace(namespaceUri);

        if (CurrentNode.LocalName != localName)
        {
            AddError(
                ErrorReason.UnexpectedLocalName,
                $"Unexpected node name \"{CurrentNode.LocalName}\", expected \"{localName}\".");

            return false;
        }

        return namespaceOk;
    }

    /// <summary>
    /// Checks if the current node has the qualified name.
    /// </summary>
    /// <param name="namespaceUri">Expected namespace</param>
    /// <param name="localName">Expected local name</param>
    /// <returns>True if expected</returns>
    public bool HasName(string namespaceUri, string localName)
        => CurrentNode.LocalName == localName && CurrentNode.NamespaceURI == namespaceUri;

    /// <summary>
    /// Get attribute value with specified <paramref name="localName"/> and where there is no namespace
    /// qualifier on the attribute.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Attribute value, null if none.</returns>
    public string? GetAttribute(string localName)
        => CurrentNode.Attributes?.GetNamedItem(localName)?.Value;

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
            AddError(
                ErrorReason.MissingAttribute,
                $"Required attribute {localName} not found on {CurrentNode.Name}.");
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
                CurrentNode,
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
        => TryGetAttribute(localName, XmlConvert.ToTimeSpan);

    /// <summary>
    /// Gets an attribute as DateTime. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime or null if parse fails</returns>
    public DateTime? GetDateTimeAttribute(string localName)
        => TryGetAttribute(localName, s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind));

    /// <summary>
    /// Gets an optional bool attribute. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime or bool if parse fails</returns>
    public bool? GetBoolAttribute(string localName)
        => TryGetAttribute(localName, XmlConvert.ToBoolean);

    /// <summary>
    /// Get an attribute as int. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of the attribute</param>
    /// <returns>Parsed int or null if parse fails</returns>
    public int? GetRequiredIntAttribute(string localName)
    {
        var stringValue = GetRequiredAttribute(localName);

        if(stringValue == null)
        {
            return default;
        }

        return TryConvertAttribute(localName, int.Parse, stringValue);
    }

    private TTarget? TryGetAttribute<TTarget>(string localName, Func<string, TTarget> converter)
        where TTarget : struct
    {
        var stringValue = GetAttribute(localName);

        if (stringValue == null)
        {
            return default;
        }

        return TryConvertAttribute(localName, converter, stringValue);
    }

    private TTarget? TryConvertAttribute<TTarget>(string localName, Func<string, TTarget> converter, string stringValue) 
        where TTarget : struct
    {
        try
        {
            return converter(stringValue);
        }
        catch (FormatException)
        {
            Errors.Add(new Error(
                ErrorReason.ConversionFailed,
                localName,
                CurrentNode,
                $"Conversion to {typeof(TTarget).Name} failed for {stringValue}")
            {
                StringValue = stringValue
            });
        }

        return default;
    }
}

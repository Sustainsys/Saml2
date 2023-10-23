using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Xml;

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
    public List<Error> Errors { get; }

    /// <summary>
    /// First Node to move to if current is null because it is before start.
    /// </summary>
    private XmlNode? firstNode;

    /// <summary>
    /// Keep parent node around to enable error reporting.
    /// </summary>
    private readonly XmlTraverser? parent;

    /// <summary>
    /// Are the children of the current node handled? Default to true
    /// as we're setting it to false whenever we hit an element.
    /// </summary>
    private bool childrenHandled = true;

    /// <summary>
    /// The current node being processed.
    /// </summary>
    public XmlNode? CurrentNode { get; private set; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="rootNode">Root node for this traverser</param>
    public XmlTraverser(XmlNode rootNode)
    {
        CurrentNode = rootNode;
        Errors = new();
    }

    /// <summary>
    /// Ctor used when processing child nodes.
    /// </summary>
    /// <param name="parent">Parent node to process children for.</param>
    /// <param name="errors">Errors collection</param>
    private XmlTraverser(XmlTraverser parent, List<Error> errors)
    {
        this.parent = parent;
        firstNode = parent.CurrentNode!.FirstChild;
        Errors = errors;
    }

    private void AddError(ErrorReason reason, string message)
    {
        Errors.Add(new(reason, CurrentNode!.LocalName, CurrentNode, message));
    }

    //TODO: Add callback function as parameter that allows ignoring - easier way to wire up with events.
    /// <summary>
    /// Throws exception if the error collection is non-empty.
    /// </summary>
    public void ThrowOnErrors()
    {
        if (parent != null)
        {
            throw new InvalidOperationException("ThrowOnErrors can only be called from the root traverser.");
        }

        if (CurrentNode != null)
        {
            throw new InvalidOperationException("Before completing the traversal, call MoveNext to move past the root element. The root element must also be marked as completely processed for the child processing detection to work.");
        }

        if (Errors.Any(e => !e.Ignore))
        {
            throw new Saml2XmlException(Errors);
        }
    }

    /// <summary>
    /// Creates an XML traverser for the child elements of the current node, keeping
    /// the same error list as the current traverser.
    /// </summary>
    /// <returns>XmlTraverser</returns>
    public XmlTraverser GetChildren() => new(this, Errors);

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

        if (CurrentNode != null
            && CurrentNode.LocalName == "Signature"
            && CurrentNode.NamespaceURI == SignedXml.XmlDsigNamespaceUrl)
        {
            childrenHandled = true;

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

        var (error, workingKey) = ((XmlElement)CurrentNode!)
            .VerifySignature(trustedSigningKeys, allowedHashAlgorithms);

        if (!string.IsNullOrEmpty(error))
        {
            AddError(ErrorReason.SignatureFailure, error);
        }

        return workingKey?.TrustLevel ?? TrustLevel.None;
    }

    /// <summary>
    /// Moves to the next child node in the current collection, if one is available.
    /// </summary>
    /// <param name="expectEnd">Do we expect this MoveNext call to hit the end of the child list? If not
    /// an error is recorded if we do not find any more nodes.</param>
    /// <returns>true if the move was successful</returns>
    public bool MoveNext(bool expectEnd = false)
    {
        while (true)
        {
            if (!childrenHandled && CurrentNode!.HasChildNodes)
            {
                Errors.Add(new(
                    ErrorReason.ExtraElements,
                    CurrentNode.LocalName,
                    CurrentNode,
                    $"All child nodes under {CurrentNode.LocalName} have not been processed."));
            }

            // First check if we are fresh into child level, then use firstChild to seed. Otherwise
            // just traverse forward one step (if possible)
            CurrentNode = firstNode ?? CurrentNode?.NextSibling;
            firstNode = null;

            if (CurrentNode == null)
            {
                if (!expectEnd)
                {
                    Errors.Add(new(
                        ErrorReason.MissingElement,
                        parent!.CurrentNode!.LocalName,
                        parent!.CurrentNode,
                        $"There should be a child element here under {parent.CurrentNode.LocalName}, but found none."));
                }

                // No more children.
                if (parent != null)
                {
                    parent!.childrenHandled = true;
                }

                return false;
            }

            if (CurrentNode.NodeType == XmlNodeType.Element)
            {
                childrenHandled = !CurrentNode.HasChildNodes;

                // We're happy, we found an element. 
                return true;
            }

            // We are ok to skip over white space and comments , but anything else is an error.
            if (CurrentNode.NodeType != XmlNodeType.Whitespace && CurrentNode.NodeType != XmlNodeType.Comment)
            {
                AddError(ErrorReason.UnsupportedNodeType, $"Unsupported node type {CurrentNode.NodeType}.");
            }
        }
    }

    /// <summary>
    /// Ignore any children of this element. This suppresses the error that there are unprocessed child nodes.
    /// </summary>
    public void IgnoreChildren()
    {
        childrenHandled = true;
    }

    /// <summary>
    /// Skip over the rest of the elements on this level. This suppresses any errors if the parent calls MoveNext
    /// </summary>
    public void Skip()
    {
        parent!.childrenHandled = true;
    }

    /// <summary>
    /// Ensures that the node has the specific namespace.
    /// </summary>
    /// <param name="namespaceUri">Expected Namespace uri.</param>
    /// <returns>True if ok</returns>
    public bool EnsureNamespace(string namespaceUri)
    {
        if (CurrentNode == null)
        {
            return false;
        }

        if (CurrentNode.NamespaceURI != namespaceUri)
        {
            AddError(
                ErrorReason.UnexpectedNamespace,
                $"Unexpected namespace \"{CurrentNode.NamespaceURI}\" for local name \"{CurrentNode.Name}\", expected \"{namespaceUri}\".");

            return false;
        }

        return true;
    }

    // TODO: Reorder params to follow XmlNode convention with localname, namespaceUri
    /// <summary>
    /// Ensure that the current node has a specific localName and namespace.
    /// </summary>
    /// <param name="namespaceUri">Expected Namespace uri</param>
    /// <param name="localName">Expected local name</param>
    /// <returns>True if both are ok</returns>
    public bool EnsureName(string namespaceUri, string localName)
    {
        var namespaceOk = EnsureNamespace(namespaceUri);

        if (CurrentNode != null && CurrentNode.LocalName != localName)
        {
            AddError(
                ErrorReason.UnexpectedLocalName,
                $"Unexpected node name \"{CurrentNode.LocalName}\", expected \"{localName}\".");

            return false;
        }

        return namespaceOk;
    }

    /// <summary>
    /// Ensures that the contents of the current node is only text and returns the text.
    /// </summary>
    /// <returns></returns>
    public string GetTextContents()
    {
        //TODO: Test case + error handling for non text content.
        IgnoreChildren();
        return CurrentNode!.InnerText;
    }

    /// <summary>
    /// Ensure that there is a current node and that the current node is an element. Typically used
    /// if the expectation of further elements is not known when <see cref="MoveNext(bool)"/> is called.
    /// </summary>
    /// <returns>Was there an element?</returns>
    public bool EnsureElement()
    {
        if (CurrentNode == null || CurrentNode.NodeType != XmlNodeType.Element)
        {
            Errors.Add(new(
                ErrorReason.MissingElement,
                parent!.CurrentNode?.LocalName,
                parent.CurrentNode,
                "There is no current node or current node is not an element."));

            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the current node has the qualified name.
    /// </summary>
    /// <param name="namespaceUri">Expected namespace</param>
    /// <param name="localName">Expected local name</param>
    /// <returns>True if expected</returns>
    public bool HasName(string namespaceUri, string localName)
        => CurrentNode != null && CurrentNode.LocalName == localName && CurrentNode.NamespaceURI == namespaceUri;

    /// <summary>
    /// Get attribute value with specified <paramref name="localName"/> and where there is no namespace
    /// qualifier on the attribute.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Attribute value, null if none.</returns>
    public string? GetAttribute(string localName)
        => CurrentNode?.Attributes?.GetNamedItem(localName)?.Value;

    /// <summary>
    /// Get required attribute value with specified <paramref name="localName"/> and where there is no namespace
    /// qualifier on the attribute.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Attribute value</returns>
    /// <exception cref="Saml2XmlException">If no such attribute is found.</exception>
    public string GetRequiredAttribute(string localName)
    {
        if (CurrentNode == null)
        {
            // If there's already an error so we're not on a node, just do nothing. There
            // should already be an error reported for missing node.
            return null!;
        }

        var value = GetAttribute(localName);

        if (value == null)
        {
            AddError(
                ErrorReason.MissingAttribute,
                $"Required attribute {localName} not found on {CurrentNode.Name}.");
        }

        return value!;
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
                $"Attribute \"{localName}\" should be an absolute Uri, but \"{value}\" isn't.")
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
    /// Gets an optional attribute as DateTime. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime or null if parse fails</returns>
    public DateTime? GetDateTimeAttribute(string localName)
        => TryGetAttribute(localName, s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind));

    /// <summary>
    /// Gets a required attribute as DateTime. On missing attribute or parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed DateTime or null if parse fails</returns>
    public DateTime GetRequiredDateTimeAttribute(string localName)
        => GetRequiredAttribute(localName, s => XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind));

    /// <summary>
    /// Gets an optional bool attribute. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of attribute</param>
    /// <returns>Parsed bool or null if parse fails.</returns>
    public bool? GetBoolAttribute(string localName)
        => TryGetAttribute(localName, XmlConvert.ToBoolean);

    /// <summary>
    /// Gets an optional enum attribute. On parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <typeparam name="TEnum">Enum type to parse</typeparam>
    /// <param name="localName">Local name of attribute</param>
    /// <param name="ignoreCase">Ignore case when parsing?</param>
    /// <returns>Parsed enum or null if parse fails</returns>
    public TEnum? GetEnumAttribute<TEnum>(string localName, bool ignoreCase)
        where TEnum : struct
        => TryGetAttribute(localName, s => Enum.Parse<TEnum>(s, ignoreCase));

    /// <summary>
    /// Get a required attribute as int. On missing attribute or parse errors the Error
    /// is reported to the errors collection.
    /// </summary>
    /// <param name="localName">Local name of the attribute</param>
    /// <returns>Parsed int or null if parse fails</returns>
    public int GetRequiredIntAttribute(string localName)
        => GetRequiredAttribute(localName, int.Parse);

    private TTarget GetRequiredAttribute<TTarget>(string localName, Func<string, TTarget> converter)
        where TTarget : struct
    {
        var stringValue = GetRequiredAttribute(localName);

        if (stringValue == null)
        {
            return default!;
        }

        return TryConvertAttribute(localName, converter, stringValue) ?? default;
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
        catch (Exception ex) when (
            ex is FormatException // Thrown by XmlConvert
            || ex is ArgumentException) // Thrown by Enum.Parse
        {
            Errors.Add(new Error(
                ErrorReason.ConversionFailed,
                localName,
                CurrentNode,
                $"Conversion to {typeof(TTarget).Name} failed for {stringValue}.")
            {
                StringValue = stringValue
            });
        }

        return default;
    }
}

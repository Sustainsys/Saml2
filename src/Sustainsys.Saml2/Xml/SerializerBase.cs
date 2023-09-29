using Microsoft.VisualBasic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Base class for serializers
/// </summary>
public class SerializerBase
{
    /// <summary>
    /// Default namespace prefix for this serializer
    /// </summary>
    protected string Prefix { get; set; } = default!;

    /// <summary>
    /// Default namespace Uri for this serializer.
    /// </summary>
    protected string NamespaceUri { get; set; } = default!;

    /// <summary>
    /// Allowed hash algorithms if validating signatures.
    /// </summary>
    protected IEnumerable<string>? AllowedHashAlgorithms { get; set; }

    /// <summary>
    /// Signing keys to trust when validating signatures of the metadata.
    /// </summary>
    protected IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <summary>
    /// Helper method that calls ThrowOnErrors. If you want to supress
    /// errors and prevent throwing, this is the last chance method to
    /// override.
    /// </summary>
    protected virtual void ThrowOnErrors(XmlTraverser source)
        => source.ThrowOnErrors();

    /// <summary>
    /// Creates an Xml document with good settings.
    /// </summary>
    protected virtual XmlDocument CreateXmlDocument() => new() { PreserveWhitespace = true };

    /// <summary>
    /// Append an element using the serializers default <see cref="Prefix"/> and <see cref="NamespaceUri"/>.
    /// </summary>
    /// <param name="node">Parent node</param>
    /// <param name="localName">local name of new element</param>
    /// <returns>The new element</returns>
    protected XmlElement Append(XmlNode node, string localName)
    {
        var ownerDoc = node as XmlDocument ?? node.OwnerDocument!;

        var element = ownerDoc.CreateElement(Prefix, localName, NamespaceUri);

        node.AppendChild(element);

        return element;
    }
}
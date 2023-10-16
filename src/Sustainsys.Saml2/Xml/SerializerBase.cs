using Microsoft.VisualBasic;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Shared interface for all serializers
/// </summary>
public interface ISerializerBase
{
    /// <summary>
    /// Allowed hash algorithms if validating signatures.
    /// </summary>
    IEnumerable<string>? AllowedHashAlgorithms { get; set; }

    /// <summary>
    /// Signing keys to trust when validating signatures of the metadata.
    /// </summary>
    IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <summary>
    /// Called when information about a Saml entity is needed, e.g. to read
    /// the signing keys and TrustedSigninKeys
    /// </summary>
    Func<string, Saml2Entity>? EntityResolver { get; set; }
}

/// <summary>
/// Base class for serializers
/// </summary>
public abstract class SerializerBase
{
    /// <summary>
    /// Default namespace prefix for this serializer
    /// </summary>
    protected string Prefix { get; set; } = default!;

    /// <summary>
    /// Default namespace Uri for this serializer.
    /// </summary>
    protected string NamespaceUri { get; set; } = default!;

    /// <inheritdoc/>
    public virtual IEnumerable<string>? AllowedHashAlgorithms { get; set; } =
        new[]
        {
            SignedXml.XmlDsigRSASHA256Url,
            SignedXml.XmlDsigRSASHA384Url,
            SignedXml.XmlDsigRSASHA512Url,
            SignedXml.XmlDsigDSAUrl
        };

    /// <inheritdoc/>
    public virtual IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <inheritdoc/>
    public virtual Func<string, Saml2Entity>? EntityResolver { get; set; }

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
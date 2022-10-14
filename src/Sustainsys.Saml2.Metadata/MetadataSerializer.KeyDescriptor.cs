using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Create Key Descriptor object.
    /// </summary>
    /// <returns>KeyDescriptor</returns>
    protected KeyDescriptor CreateKeyDescriptor() => new();

    /// <summary>
    /// Read KeyDescriptor
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual KeyDescriptor ReadKeyDescriptor(XmlTraverser source)
    {
        var result = CreateKeyDescriptor();

        result.Use = source.GetEnumAttribute<KeyUse>(AttributeNames.use, true) ?? KeyUse.Both;

        var children = source.GetChildren();

        if (children.MoveNext()
            && children.EnsureName(SignedXml.XmlDsigNamespaceUrl, ElementNames.KeyInfo))
        {
            children.IgnoreChildren();

            var keyInfo = new KeyInfo();
            keyInfo.LoadXml((XmlElement)children.CurrentNode);

            result.KeyInfo = keyInfo;

            children.MoveNext(true);
        }

        return result;
    }
}

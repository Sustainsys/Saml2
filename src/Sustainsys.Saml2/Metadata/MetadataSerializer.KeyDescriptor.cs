using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Create Key Descriptor object.
    /// </summary>
    /// <returns>KeyDescriptor</returns>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1822 // Mark members as static - this is intended to be possible to override.
    protected KeyDescriptor CreateKeyDescriptor() => new();
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0079 // Remove unnecessary suppression

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
            && children.EnsureName(SignedXml.XmlDsigNamespaceUrl, Constants.Elements.KeyInfo))
        {
            children.IgnoreChildren();

            var keyInfo = new KeyInfo();
            keyInfo.LoadXml((XmlElement)children.CurrentNode!);

            result.KeyInfo = keyInfo;

            children.MoveNext(true);
        }

        return result;
    }
}

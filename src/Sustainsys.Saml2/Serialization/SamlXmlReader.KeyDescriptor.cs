using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using System.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader
{
    /// <summary>
    /// Read KeyDescriptor
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual KeyDescriptor ReadKeyDescriptor(XmlTraverser source)
    {
        var result = Create<KeyDescriptor>();

        result.Use = source.GetEnumAttribute<KeyUse>(Attributes.use, true) ?? KeyUse.Both;

        var children = source.GetChildren();

        if (children.MoveNext()
            && children.EnsureName(SignedXml.XmlDsigNamespaceUrl, Elements.KeyInfo))
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

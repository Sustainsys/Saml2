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

        var useValue = source.GetAttribute(AttributeNames.use);
        if(useValue != null)
        {
            if(Enum.TryParse(useValue, ignoreCase:true, out KeyUse use))
            {
                result.Use = use;
            }
        }

        using (source.EnterChildLevel())
        { 
            if(source.MoveToNextRequiredChild()
                && source.EnsureName(SignedXml.XmlDsigNamespaceUrl, ElementNames.KeyInfo))
            {
                var keyInfo = new KeyInfo();
                keyInfo.LoadXml((XmlElement)source.CurrentNode);

                result.KeyInfo = keyInfo;

                source.MoveToNextChild();
            }
        }

        return result;
    }
}

using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;
partial class MetadataSerializer
{
    /// <summary>
    /// Read attributes of SSODescriptor.
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    protected virtual void ReadAttributes(XmlTraverser source, SSODescriptor result)
    {
        ReadAttributes(source, (RoleDescriptor)result);
    }
}

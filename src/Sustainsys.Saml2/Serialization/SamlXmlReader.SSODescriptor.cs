using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
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

    /// <summary>
    /// Read elements of SSODescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    /// <returns>More elements available?</returns>
    protected virtual bool ReadElements(XmlTraverser source, SSODescriptor result)
    {
        if(!ReadElements(source, (RoleDescriptor)result))
        {
            return false;
        }

        while (source.HasName(Namespaces.MetadataUri, Elements.ArtifactResolutionService))
        {
            result.ArtifactResolutionServices.Add(ReadIndexedEndpoint(source));

            source.MoveNext(true);
        }

        while(source.HasName(Namespaces.MetadataUri, Elements.SingleLogoutService))
        {
            result.SingleLogoutServices.Add(ReadEndpoint(source));

            source.MoveNext(true);
        }

        while(source.HasName(Namespaces.MetadataUri, Elements.ManageNameIDService)
            || source.HasName(Namespaces.MetadataUri, Elements.NameIDFormat))
        {
            // We're not supporting ManageNameIDService nor NameIDFormat.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        return true;
    }
}
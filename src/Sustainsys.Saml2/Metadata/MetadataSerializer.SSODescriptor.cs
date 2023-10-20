using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Xml;

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

        while (source.HasName(NamespaceUri, Constants.Elements.ArtifactResolutionService))
        {
            result.ArtifactResolutionServices.Add(ReadIndexedEndpoint(source));

            if(!source.MoveNext())
            {
                return false;
            }
        }

        while(source.HasName(NamespaceUri, Constants.Elements.SingleLogoutService))
        {
            result.SingleLogoutServices.Add(ReadEndpoint(source));

            if (!source.MoveNext())
            {
                return false;
            }
        }

        while(source.HasName(NamespaceUri, Constants.Elements.ManageNameIDService)
            || source.HasName(NamespaceUri, Constants.Elements.NameIDFormat))
        {
            // We're not supporting ManageNameIDService nor NameIDFormat.
            source.IgnoreChildren();

            if(!source.MoveNext())
            {
                return false;
            }
        }

        return true;
    }
}
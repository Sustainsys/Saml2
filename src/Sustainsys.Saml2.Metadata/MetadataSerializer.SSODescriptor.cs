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

        while (source.HasName(Namespaces.Metadata, ElementNames.ArtifactResolutionService))
        {
            result.ArtifactResolutionServices.Add(ReadIndexedEndpoint(source));

            if(!source.MoveToNextChild())
            {
                return false;
            }
        }

        while(source.HasName(Namespaces.Metadata, ElementNames.SingleLogoutService))
        {
            result.SingleLogoutServices.Add(ReadEndpoint(source));

            if (!source.MoveToNextChild())
            {
                return false;
            }
        }

        while(source.HasName(Namespaces.Metadata, ElementNames.ManageNameIDService)
            || source.HasName(Namespaces.Metadata, ElementNames.NameIDFormat))
        {
            // We're not supporting ManageNameIDService nor NameIDFormat.
            if(!source.MoveToNextChild())
            {
                return false;
            }
        }

        return true;
    }
}
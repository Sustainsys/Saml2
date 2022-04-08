using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Create an IDPSSODescriptor instance.
    /// </summary>
    protected virtual IDPSSODescriptor CreateIDPSSODescriptor => new();

    /// <summary>
    /// Read the current node as an IDPSSODescriptor
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual IDPSSODescriptor ReadIDPSSODescriptor(XmlTraverser source)
    {
        var result = new IDPSSODescriptor();

        ReadAttributes(source, (SSODescriptor)result);

        using (source.EnterChildLevel())
        {
            ReadElements(source, result);
        }

        return result;
    }

    /// <summary>
    /// Read child elements of 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="result"></param>
    protected virtual void ReadElements(XmlTraverser source, IDPSSODescriptor result)
    {
        if(!source.MoveToNextRequiredChild())
        {
            return;
        }

        do
        {
            if(source.EnsureNamespace(Namespaces.Metadata))
            {
                switch (source.CurrentNode.LocalName)
                {
                    case ElementNames.SingleSignOnService:
                        result.SingleSignOnServices.Add(ReadEndpoint(source));
                        break;
                }
            }
        } while (source.MoveToNextChild());
    }
}

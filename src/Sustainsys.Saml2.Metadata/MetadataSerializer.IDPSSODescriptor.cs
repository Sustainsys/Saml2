using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Process an IDPSSODescriptor element.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="entityDescriptor">Currently processed EntityDescriptor</param>
    /// <returns>True if current node was an IDPSSoDescriptor element</returns>
    protected bool ReadIDPSSODescriptor(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        return source.CurrentNode.LocalName == ElementNames.IDPSSODescriptor
            && source.CurrentNode.NamespaceURI == Namespaces.Metadata;
    }
}

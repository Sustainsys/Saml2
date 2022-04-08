using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Process extensions node. Default just checks qualified name and then returns.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="entityDescriptor">Currently processed EntityDescriptor</param>
    /// <returns>True if current node was an Extensions element</returns>
    protected virtual bool ReadExtensions(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        return source.CurrentNode.LocalName == ElementNames.Extensions
            && source.CurrentNode.NamespaceURI == Namespaces.Metadata;
    }
}

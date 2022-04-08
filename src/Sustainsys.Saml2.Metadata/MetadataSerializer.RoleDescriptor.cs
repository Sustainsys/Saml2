using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata;

partial class MetadataSerializer
{
    /// <summary>
    /// Process a RoleDescriptor element. Default just checks qualified name and then returns.
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="entityDescriptor">Currently processed EntityDescriptor</param>
    /// <returns>True if current node was a RoleDescriptor element</returns>
    protected bool ReadRoleDescriptor(XmlTraverser source, EntityDescriptor entityDescriptor)
    {
        return source.CurrentNode.LocalName == ElementNames.RoleDescriptor
            && source.CurrentNode.NamespaceURI == Namespaces.Metadata;
    }
}

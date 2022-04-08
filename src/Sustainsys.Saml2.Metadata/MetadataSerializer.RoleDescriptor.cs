using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata;
partial class MetadataSerializer
{
    /// <summary>
    /// Create RoleDescriptor instance
    /// </summary>
    /// <returns>RoleDescriptor</returns>
    protected virtual RoleDescriptor CreateRoleDescriptor() => new();

    /// <summary>
    /// Process a RoleDescriptor element.
    /// </summary>
    /// <param name="source">Source</param>
    /// <returns>True if current node was a RoleDescriptor element</returns>
    protected virtual RoleDescriptor ReadRoleDescriptor(XmlTraverser source)
    {
        var result = CreateRoleDescriptor();

        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Read attributs of RoleDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    protected virtual void ReadAttributes(XmlTraverser source, RoleDescriptor result)
    {
        result.ProtocolSupportEnumeration =
            source.GetRequiredAbsoluteUriAttribute(AttributeNames.protocolSupportEnumeration);
    }
}

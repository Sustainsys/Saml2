using Sustainsys.Saml2.Metadata.Attributes;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using System.Security.Cryptography.Xml;

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

    /// <summary>
    /// Read elements of RoleDescriptor
    /// </summary>
    /// <param name="source">Source data</param>
    /// <param name="result">Target to set properties on</param>
    /// <returns>More elements available?</returns>
    protected virtual bool ReadElements(XmlTraverser source, RoleDescriptor result)
    {
        if (source.HasName(SignedXml.XmlDsigNamespaceUrl, ElementNames.Signature))
        {
            // Signatures on RoleDescriptors are not supported.
            if (!source.MoveToNextChild())
            {
                return false;
            }
        }

        if (source.HasName(Namespaces.Metadata, ElementNames.Extensions))
        {
            // Extensions on RoleDescriptors are not supported.
            if (!source.MoveToNextChild())
            {
                return false;
            }
        }

        while (source.HasName(Namespaces.Metadata, ElementNames.KeyDescriptor))
        {
            result.Keys.Add(ReadKeyDescriptor(source));
            if(!source.MoveToNextChild())
            {
                return false;
            }
        }

        if (source.HasName(Namespaces.Metadata, ElementNames.Organization))
        {
            // Organization reading is not supported.
            if(!source.MoveToNextChild())
            {
                return false;
            }
        }

        if(source.HasName(Namespaces.Metadata, ElementNames.ContactPerson))
        {
            // Contact person reading is not supported.
            if(!source.MoveToNextChild())
            {
                return false;
            }
        }

        return true;
    }
}


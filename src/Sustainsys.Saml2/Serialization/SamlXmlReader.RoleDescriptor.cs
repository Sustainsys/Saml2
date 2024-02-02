using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;
partial class SamlXmlReader
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
        ReadElements(source.GetChildren(), result);

        // Custom RoleDesciptors might have other elements that we do not know - ignore them.
        source.IgnoreChildren();

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
    protected virtual void ReadElements(XmlTraverser source, RoleDescriptor result)
    {
        source.MoveNext(true);

        if (source.HasName(SignedXml.XmlDsigNamespaceUrl, Elements.Signature))
        {
            // Signatures on RoleDescriptors are not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.MetadataUri, Elements.Extensions))
        {
            // Extensions on RoleDescriptors are not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        while (source.HasName(Namespaces.MetadataUri, Elements.KeyDescriptor))
        {
            result.Keys.Add(ReadKeyDescriptor(source));
            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.MetadataUri, Elements.Organization))
        {
            // Organization reading is not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }

        if (source.HasName(Namespaces.MetadataUri, Elements.ContactPerson))
        {
            // Contact person reading is not supported.
            source.IgnoreChildren();

            source.MoveNext(true);
        }
    }
}


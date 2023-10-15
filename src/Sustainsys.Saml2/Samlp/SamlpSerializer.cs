using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Saml;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Serializer for Saml protocol classes
/// </summary>
public interface ISamlpSerializer : ISerializerBase
{
    /// <summary>
    /// Create an Xml document and write an AuthnRequest to it.
    /// </summary>
    /// <param name="authnRequest">AuthnRequest</param>
    /// <returns>Created XmlDoc</returns>
    XmlDocument Write(AuthnRequest authnRequest);

    /// <summary>
    /// Read an AuthnRequest from Xml
    /// </summary>
    /// <param name="source">XmlTraverser to read from</param>
    /// <returns>AuthnRequest</returns>
    public AuthnRequest ReadAuthnRequest(XmlTraverser source);

    /// <summary>
    /// Read a SamlResponse from Xml.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public SamlResponse ReadSamlResponse(XmlTraverser source);
}

/// <summary>
/// Serializer for Saml protocol classes
/// </summary>
public partial class SamlpSerializer : SerializerBase, ISamlpSerializer
{
    private readonly ISamlSerializer samlSerializer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="samlSerializer"></param>
    public SamlpSerializer(ISamlSerializer samlSerializer)
    {
        Prefix = "samlp";
        NamespaceUri = Constants.Namespaces.Samlp;
        this.samlSerializer = samlSerializer;
    }
}

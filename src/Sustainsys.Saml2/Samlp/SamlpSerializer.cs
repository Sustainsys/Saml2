using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Saml;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Serializer for Saml protocol classes
/// </summary>
public interface ISamlpSerializer
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

    /// <summary>
    /// Gets or sets Trusted Signing Keys. This property just wraps
    /// the inner <see cref="SamlSerializer"/> TrustedSigninKeys.
    /// </summary>
    public override IEnumerable<SigningKey>? TrustedSigningKeys 
    { 
        get => samlSerializer.TrustedSigningKeys; 
        set => samlSerializer.TrustedSigningKeys = value; 
    }

    /// <summary>
    /// Gets or sets Allowed Hash algorithm. This property just wraps
    /// the inner <see cref="SamlSerializer"/> AllowedHashAlgorithms.
    /// </summary>
    public override IEnumerable<string>? AllowedHashAlgorithms 
    { 
        get => samlSerializer.AllowedHashAlgorithms; 
        set => samlSerializer.AllowedHashAlgorithms = value; 
    }

    /// <summary>
    /// Placeholder for extension reading. Default ignores contents.
    /// </summary>
    /// <param name="source">Xml Traverser</param>
    /// <returns>Extensions</returns>
    public virtual Extensions ReadExtensions(XmlTraverser source)
    {
        source.IgnoreChildren();
        return new Extensions();
    }
}

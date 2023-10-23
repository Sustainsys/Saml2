using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using System.Xml;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Write Saml entities to Xml
/// </summary>
public interface ISamlXmlWriter
{
    /// <summary>
    /// Create an Xml document and write an AuthnRequest to it.
    /// </summary>
    /// <param name="authnRequest">AuthnRequest</param>
    /// <returns>Created XmlDoc</returns>
    XmlDocument Write(AuthnRequest authnRequest);
}

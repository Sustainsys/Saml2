using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Abstract base class for requests
/// </summary>
public class RequestAbstractType
{
    /// <summary>
    /// Id of the request.
    /// </summary>
    public string Id { get; set; } = XmlHelpers.CreateId();

    /// <summary>
    /// Version of message, should always be 2.0
    /// </summary>
    public string Version { get; set; } = "2.0";

    /// <summary>
    /// Issue instant
    /// </summary>
    public DateTime IssueInstant { get; set; }

    /// <summary>
    /// Identifies the entity that generated the request message.
    /// </summary>
    public NameId? Issuer { get; set; }
}

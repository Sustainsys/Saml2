using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;

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

    /// <summary>
    /// Destination Url that the messages is/was sent to.
    /// </summary>
    public string? Destination { get; set; }

    /// <summary>
    /// URI reference for consent.
    /// </summary>
    public string? Consent { get; set; }

    /// <summary>
    /// Extensions
    /// </summary>
    public Extensions? Extensions { get; set; }

    /// <summary>
    /// Trust level, based on signature validation
    /// </summary>
    public TrustLevel TrustLevel { get; set; }
}
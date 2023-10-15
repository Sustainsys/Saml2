using Sustainsys.Saml2.Samlp.Elements;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Samlp;
/// <summary>
/// Saml2 p abstract StatusResponseType
/// </summary>
public class StatusResponseType
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
    /// Saml status
    /// </summary>
    public SamlStatus Status { get; set; } = default!;
}

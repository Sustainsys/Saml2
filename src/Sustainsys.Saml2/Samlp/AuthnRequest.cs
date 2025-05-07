using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// Authentication request
/// </summary>
public class AuthnRequest : RequestAbstractType
{
    /// <summary>
    /// The assertion consumer service Url where the response should be posted back to.
    /// </summary>
    public string? AssertionConsumerServiceUrl { get; set; }

    /// <summary>
    /// Identifying Uri for protocol binding
    /// </summary>
    public string? ProtocolBinding { get; set; }

    /// <summary>
    /// Requested Subject
    /// </summary>
    public Subject? Subject { get; set; }

    /// <summary>
    /// Tailors the name identifier in the sucject's of assertions. 
    /// </summary>
    public NameIdPolicy? NameIdPolicy { get; set; }

    /// <summary>
    /// SAML constructs that place constraints on the use of SAML assertion.
    /// </summary>
    /// 
    public Conditions? Conditions { get; set; }

    /// <summary>
    /// Indicates that the identity provider should force (re)authentication and not
    /// rely on an existing session to do single sign on.
    /// </summary>
    public bool ForceAuthn { get; set; } = false;

    /// <summary>
    /// Indicates that the identity provider should not show any UI nor require interaction.
    /// </summary>
    public bool IsPassive { get; set; } = false;

    /// <summary>
    /// Index of assertion consumer service where the SP wants the response.
    /// </summary>
    public int? AssertionConsumerServiceIndex { get; set; }
}
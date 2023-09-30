namespace Sustainsys.Saml2;

/// <summary>
/// A Saml2 identity provider
/// </summary>
public class IdentityProvider
{
    /// <summary>
    /// The entity id of the identity provider
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// The Sso service url of the identity provider
    /// </summary>
    public string? SsoServiceUrl { get; set; }
    
    /// <summary>
    /// Identifying Uri for SSO Service binding
    /// </summary>
    public string? SsoServiceBinding { get; set; }
}

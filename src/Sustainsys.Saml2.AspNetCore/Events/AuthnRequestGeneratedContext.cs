using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.AspNetCore;
using Sustainsys.Saml2.Samlp;

namespace Sustainsys.Saml2.AspNetCore.Events;

/// <summary>
/// Context for AuthnRequestGenerated event
/// </summary>
public class AuthnRequestGeneratedContext : PropertiesContext<Saml2Options>
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="context">Http Context</param>
    /// <param name="scheme">Authentication Scheme</param>
    /// <param name="options">Options</param>
    /// <param name="properties">Authentication Properties</param>
    /// <param name="authnRequest">AuthnRequest</param>
    public AuthnRequestGeneratedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        Saml2Options options,
        AuthenticationProperties properties,
        AuthnRequest authnRequest)
        : base(context, scheme, options, properties)
    {
        AuthnRequest = authnRequest;
    }

    /// <summary>
    /// The generated AuthnRequest
    /// </summary>
    public AuthnRequest AuthnRequest { get; set; }
}
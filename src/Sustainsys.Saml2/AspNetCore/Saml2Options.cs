using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Saml2 authentication options
/// </summary>
public class Saml2Options : RemoteAuthenticationOptions
{
    /// <summary>
    /// Ctor
    /// </summary>
    public Saml2Options()
    {
        CallbackPath = new PathString("/Saml2/Acs");
    }

    /// <summary>
    /// Events can be used to override behaviour. Setting this property is the easy way.
    /// To resolve from DI register Saml2Events as a keyed service with the scheme name
    /// as the key, or to use the same events for all schemes register as an normal
    /// service
    /// </summary>
    public new Saml2Events Events
    {
        get => (Saml2Events)base.Events;
        set => base.Events = value;
    }

    /// <summary>
    /// Identityprovider configuration for this scheme.
    /// </summary>
    public IdentityProvider? IdentityProvider { get; set; }

    /// <summary>
    /// NameId of the service provider.
    /// </summary>
    public NameId? EntityId { get; set; }
}
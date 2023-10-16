using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Saml.Elements;

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
    /// The service resolver can 
    /// </summary>
    public ServiceResolver ServiceResolver { get; set; } = new ServiceResolver();

    /// <summary>
    /// Events can be used to override behaviour. Setting this property is the easy way.
    /// To resolve the events form DI, use <see cref="ServiceResolver.CreateEvents"/>
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
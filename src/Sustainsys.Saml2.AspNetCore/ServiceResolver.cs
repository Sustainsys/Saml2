using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// The Sustainsys.Saml2 library uses multiple loosely coupled services internally. The 
/// default implementation is to not register these in the main dependency injection 
/// system to avoid clutter. All services are resolved using the service resolver.
/// To override services, override the factory method here. The resolver context
/// always contains the HttpContext, which can be used to resolve services from DI.
/// </summary>
public class ServiceResolver
{
    /// <summary>
    /// Context for service resolver
    /// </summary>
    /// <param name="Context">Current http context</param>
    /// <param name="Options">Current options</param>
    /// <param name="AuthenticationProperties">Authentication properties, if available</param>
    public record struct ResolverContext(
        HttpContext Context,
        Saml2Options Options,
        AuthenticationProperties? AuthenticationProperties = null)
    { }

    /// <summary>
    /// Factory for the events class. Defaults to returning a new Saml2Events instance. It's usually
    /// easier to just set the Events property on the options than to use this. If you want to
    /// resolve the events from DI, this is the best place to do it.
    /// </summary>
    public Func<ResolverContext, Saml2Events> CreateEvents { get; set; } 
        = (_) => new Saml2Events();
}

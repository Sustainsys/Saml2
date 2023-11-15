using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
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
    /// <param name="context">Current http context</param>
    /// <param name="options">Current options</param>
    /// <param name="scheme">Current authentication scheme</param>
    /// <param name="authenticationProperties">Authentication properties, if available</param>
    public class ResolverContext(
        HttpContext context,
        Saml2Options options,
        AuthenticationScheme scheme,
        AuthenticationProperties? authenticationProperties)
    {
        /// <summary>
        /// Current HttpContext
        /// </summary>
        public HttpContext Context { get; } = context;

        /// <summary>
        /// Current options
        /// </summary>
        public Saml2Options Options { get; } = options;

        /// <summary>
        /// Current authentication scheme
        /// </summary>
        public AuthenticationScheme Scheme { get; } = scheme;

        /// <summary>
        /// Current AuthenticationProperties, if available
        /// </summary>
        public AuthenticationProperties? AuthenticationProperties { get; } = authenticationProperties;
    }

    /// <summary>
    /// Factory for the events class. Defaults to returning a new Saml2Events instance. It's usually
    /// easier to just set the Events property on the options than to use this. If you want to
    /// resolve the events from DI, this is the best place to do it.
    /// </summary>
    public Func<ResolverContext, Saml2Events> CreateEvents { get; set; }
        = _ => new Saml2Events();

    /// <summary>
    /// Factory for <see cref="ISamlXmlReader"/>
    /// </summary>
    public Func<ResolverContext, ISamlXmlReader> GetSamlXmlReader { get; set; }
        = _ => new SamlXmlReader();

    /// <summary>
    /// Factory for <see cref="ISamlXmlWriter"/>
    /// </summary>
    public Func<ResolverContext, ISamlXmlWriter> GetSamlXmlWriter { get; set; }
        = _ => new SamlXmlWriter();

    /// <summary>
    /// Factory for collection of front channel bindings.
    /// </summary>
    public Func<ResolverContext, IEnumerable<IFrontChannelBinding>> GetAllBindings { get; set; }
        = _ => new IFrontChannelBinding[] { new HttpRedirectBinding(), new HttpPostBinding() };

    /// <summary>
    /// Context for resolving binding
    /// </summary>
    /// <param name="context">Current http context</param>
    /// <param name="options">Current options</param>
    /// <param name="scheme">Current authentication scheme</param>
    /// <param name="authenticationProperties">Authentication properties, if available</param>
    /// <param name="binding">Uri for requested binding</param>
    public class BindingResolverContext(
        HttpContext context,
        Saml2Options options,
        AuthenticationScheme scheme,
        AuthenticationProperties? authenticationProperties,
        string binding) 
        : ResolverContext(context, options, scheme, authenticationProperties)
    {

        /// <summary>
        /// Requested binding
        /// </summary>
        public string Binding { get; } = binding;
    }

    /// <summary>
    /// Factory for front channel bindings
    /// </summary>
    public Func<BindingResolverContext, IFrontChannelBinding> GetBinding { get; set; }
        = ctx =>
        {
            if(string.IsNullOrEmpty(ctx.Binding))
            {
                throw new ArgumentNullException("Binding property must have value to get binding");
            }

            return ctx.Options.ServiceResolver.GetAllBindings(ctx)
                .SingleOrDefault(b => b.Identifier == ctx.Binding)
                ?? throw new NotImplementedException($"Unknown binding {ctx.Binding} requested");
        };
}

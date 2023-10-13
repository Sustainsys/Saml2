using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
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
    public class ResolverContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="options">Current options</param>
        /// <param name="scheme">Current authentication scheme</param>
        /// <param name="authenticationProperties">Authentication properties, if available</param>
        public ResolverContext(
            HttpContext context,
            Saml2Options options,
            AuthenticationScheme scheme,
            AuthenticationProperties? authenticationProperties)
        {
            Context = context;
            Options = options;
            Scheme = scheme;
            AuthenticationProperties = authenticationProperties;
        }

        /// <summary>
        /// Current HttpContext
        /// </summary>
        public HttpContext Context { get; }

        /// <summary>
        /// Current options
        /// </summary>
        public Saml2Options Options { get; }

        /// <summary>
        /// Current authentication scheme
        /// </summary>
        public AuthenticationScheme Scheme { get; }

        /// <summary>
        /// Current AuthenticationProperties, if available
        /// </summary>
        public AuthenticationProperties? AuthenticationProperties { get; }
    }

    /// <summary>
    /// Factory for the events class. Defaults to returning a new Saml2Events instance. It's usually
    /// easier to just set the Events property on the options than to use this. If you want to
    /// resolve the events from DI, this is the best place to do it.
    /// </summary>
    public Func<ResolverContext, Saml2Events> CreateEvents { get; set; }
        = _ => new Saml2Events();

    /// <summary>
    /// Factory for <see cref="ISamlpSerializer"/>
    /// </summary>
    public Func<ResolverContext, ISamlpSerializer> GetSamlpSerializer { get; set; }
        = ctx => new SamlpSerializer(ctx.Options.ServiceResolver.GetSamlSerializer(ctx));

    /// <summary>
    /// Factory for <see cref="ISamlSerializer"/>
    /// </summary>
    public Func<ResolverContext, ISamlSerializer> GetSamlSerializer { get; set; }
        = _ => new SamlSerializer();

    /// <summary>
    /// Factory for collection of front channel bindings.
    /// </summary>
    public Func<ResolverContext, IEnumerable<IFrontChannelBinding>> GetAllBindings { get; set; }
        = _ => new IFrontChannelBinding[] { new HttpRedirectBinding(), new HttpPostBinding() };
    
    /// <summary>
    /// Context for resolving binding
    /// </summary>
    public class BindingResolverContext : ResolverContext
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Current http context</param>
        /// <param name="options">Current options</param>
        /// <param name="scheme">Current authentication scheme</param>
        /// <param name="authenticationProperties">Authentication properties, if available</param>
        /// <param name="binding">Uri for requested binding</param>
        public BindingResolverContext(
            HttpContext context,
            Saml2Options options,
            AuthenticationScheme scheme,
            AuthenticationProperties? authenticationProperties,
            string binding)
            : base(context, options, scheme, authenticationProperties)
        {
            Binding = binding;
        }

        /// <summary>
        /// Requested binding
        /// </summary>
        public string Binding { get; }
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

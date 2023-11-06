using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore.Events;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.AspNetCore;

// TODO: OTel Metrics + Activities + logging/traces

/// <summary>
/// Saml2 authentication handler
/// </summary>
public class Saml2Handler : RemoteAuthenticationHandler<Saml2Options>
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Options</param>
    /// <param name="logger">Logger factory</param>
    /// <param name="encoder">Url encoder</param>
    /// <param name="clock">System Clock</param>
    public Saml2Handler(
        IOptionsMonitor<Saml2Options> options,
        ILoggerFactory logger,
        UrlEncoder encoder
#if NET8_0_OR_GREATER
        )
        : base(options, logger, encoder)
#else
        ,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
#endif
    {
    }

    /// <summary>
    /// Create events by invoking Options.ServiceResolver.CreateEventsAsync()
    /// </summary>
    /// <returns><see cref="Saml2Events"/>Saml2 events instance</returns>
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(GetService(s => s.CreateEvents));

    private T GetService<T>(
        Func<ServiceResolver, Func<ServiceResolver.ResolverContext, T>> factorySelector,
        AuthenticationProperties? authenticationProperties = null) =>
        factorySelector(Options.ServiceResolver)
        (new ServiceResolver.ResolverContext(Context, Options, Scheme, authenticationProperties));

    /// <summary>
    /// Events represents the easiest and most straight forward way to customize the
    /// behaviour of the Saml2 handler. An event can inspect and alter data.
    /// </summary>
    protected new Saml2Events Events
    {
        get => (Saml2Events)base.Events;
        set { base.Events = value; }
    }

    /// <summary>
    /// Handles incoming request on the callback path.
    /// </summary>
    /// <returns></returns>
    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var bindings = GetService(sr => sr.GetAllBindings);

        var binding = bindings.SingleOrDefault(b => b.CanUnbind(Context.Request));

        if (binding == null)
        {
            return HandleRequestResult.Fail("No binding could find a Saml message in the request");
        }

        var samlMessage = await binding.UnbindAsync(Context.Request, str => Task.FromResult<Saml2Entity>(Options.IdentityProvider!));

        var source = XmlHelpers.GetXmlTraverser(samlMessage.Xml);
        var reader = GetService(sr => sr.GetSamlXmlReader);
        var samlResponse = reader.ReadSamlResponse(source);

        // For now, to make half-baked test pass.
        return HandleRequestResult.Handle();
    }

    /// <summary>
    /// Redirects to identity provider with an authentication request.
    /// </summary>
    /// <param name="properties">Authentication properties</param>
    /// <returns>Task</returns>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authnRequest = new AuthnRequest()
        {
            Issuer = Options.EntityId,
            IssueInstant = Clock.UtcNow.DateTime,
            AssertionConsumerServiceUrl = BuildRedirectUri(Options.CallbackPath)
        };

        var authnRequestGeneratedContext = new AuthnRequestGeneratedContext(Context, Scheme, Options, properties, authnRequest);
        await Events.AuthnRequestGeneratedAsync(authnRequestGeneratedContext);

        var xmlDoc = GetService(sr => sr.GetSamlXmlWriter, properties).Write(authnRequest);

        var message = new Saml2Message
        {
            Destination = Options.IdentityProvider!.SsoServiceUrl!,
            Name = Constants.SamlRequest,
            Xml = xmlDoc.DocumentElement!,
        };

        var binding = Options.ServiceResolver.GetBinding(
            new(Context, Options, Scheme, properties, Options.IdentityProvider.SsoServiceBinding!));

        await binding.BindAsync(Response, message);
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.AspNetCore.Events;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Validation;
using Sustainsys.Saml2.Xml;
using System.Text.Encodings.Web;

namespace Sustainsys.Saml2.AspNetCore;

// TODO: OTel Metrics + Activities + logging/traces

/// <summary>
/// Saml2 authentication handler
/// </summary>
/// <param name="options">Options</param>
/// <param name="logger">Logger factory</param>
/// <param name="encoder">Url encoder</param>
/// <param name="serviceProvider">Service provider used to resolve services</param>
public class Saml2Handler(
    IOptionsMonitor<Saml2Options> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IServiceProvider serviceProvider)
    : RemoteAuthenticationHandler<Saml2Options>(options, logger, encoder)
{
    private const string RequestIdKey = ".reqId";
    private const string IdpKey = ".idp";

    private TService GetRequiredService<TService>() where TService : notnull =>
        serviceProvider.GetKeyedService<TService>(Scheme.Name) ??
        serviceProvider.GetRequiredService<TService>();

    private IEnumerable<IFrontChannelBinding> GetAllFrontChannelBindings() =>
        serviceProvider.GetKeyedServices<IFrontChannelBinding>(Scheme.Name)
        .Union(serviceProvider.GetServices<IFrontChannelBinding>());

    private IFrontChannelBinding GetFrontChannelBinding(string uri) =>
        GetAllFrontChannelBindings().First(b => b.Identifier == uri);

    /// <summary>
    /// Resolves events as keyed service from DI, falls back to non-keyed service and 
    /// finally falls back to creating an events instance.
    /// </summary>
    /// <returns><see cref="Saml2Events"/>Saml2 events instance</returns>
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(
        serviceProvider.GetKeyedService<Saml2Events>(Scheme.Name) ??
        new Saml2Events());

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
        var bindings = GetAllFrontChannelBindings();

        var binding = bindings.FirstOrDefault(b => b.CanUnbind(Context.Request));

        if (binding == null)
        {
            return HandleRequestResult.Fail("No binding could find a Saml message in the request");
        }

        var samlMessage = await binding.UnbindAsync(Context.Request, str => Task.FromResult<Saml2Entity>(Options.IdentityProvider!));

        var source = XmlHelpers.GetXmlTraverser(samlMessage.Xml);
        var reader = GetRequiredService<ISamlXmlReader>();
        var samlResponse = reader.ReadResponse(source);

        var validator = GetRequiredService<IValidator<Response, ResponseValidationParameters>>();

        // TODO: Do proper validation! + Tests!
        ResponseValidationParameters validationParameters = new()
        {
            AssertionValidationParameters = new()
            {
                ValidIssuer = Options.IdentityProvider!.EntityId!,
                ValidAudience = Options.EntityId!.Value,
                ValidRecipient = GetAbsoluteUrl(Options.CallbackPath)
            },
        };

        var validatedResponse = samlResponse.Validate(validator, validationParameters);

        // TODO: Handle multiple assertions.
        var validatedAssertion = validatedResponse.GetValid(r => r.Assertions.Single());

        var claimsFactory = GetRequiredService<IClaimsFactory>();

        var identity = claimsFactory.GetClaimsIdentity(validatedAssertion);

        AuthenticationTicket authenticationTicket = new(new(identity), Scheme.Name);

        return HandleRequestResult.Success(authenticationTicket);
    }

    private string GetAbsoluteUrl(PathString callbackPath) =>
        $"{Request.Scheme}://{Request.Host}{callbackPath}";

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
            IssueInstant = TimeProvider.GetUtcNow().UtcDateTime,
            AssertionConsumerServiceUrl = BuildRedirectUri(Options.CallbackPath)
        };

        //TODO: Don't use Options.IdentityProvider directly, access via event/callback.
        var authnRequestGeneratedContext = new AuthnRequestGeneratedContext(
            Context, Scheme, Options, properties, authnRequest, Options.IdentityProvider!);

        await Events.AuthnRequestGeneratedAsync(authnRequestGeneratedContext);

        // Capture the security sensitive state after the event
        properties.Items[RequestIdKey] = authnRequest.Id;
        properties.Items[IdpKey] = Options.IdentityProvider!.EntityId;

        var xmlDoc = GetRequiredService<ISamlXmlWriter>().Write(authnRequest);

        var idpEntityIdHash = Options.IdentityProvider.EntityId!.Sha256(10);
        var message = new Saml2Message
        {
            Destination = Options.IdentityProvider!.SsoServiceUrl!,
            Name = Constants.SamlRequest,
            Xml = xmlDoc.DocumentElement!,
            RelayState = $"{idpEntityIdHash}.{authnRequest.Id}"
        };

        var cookieOptions = Options.CorrelationCookie.Build(Context, TimeProvider.GetUtcNow());

        // TODO: If needed: make an alternative to this to allow multiple concurrent sign in attempts to same Idp
        var cookieName = "Saml2." + idpEntityIdHash;
        var cookieValue = Options.StateCookieDataFormat.Protect(properties);

        Options.StateCookieManager.AppendResponseCookie(Context, cookieName, cookieValue, cookieOptions);

        var binding = GetFrontChannelBinding(Options.IdentityProvider.SsoServiceBinding!);

        await binding.BindAsync(Response, message);
    }
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

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
using Sustainsys.Saml2.Services;
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
    private const string CookiePrefix = "Saml2.";
    private const int StateIdpHashLength = 10;

    // TODO: Make protected virtual and move keyed service fallback to plus package.
    private TService GetRequiredService<TService>() where TService : notnull =>
        serviceProvider.GetKeyedService<TService>(Scheme.Name) ??
        serviceProvider.GetRequiredService<TService>();

    private IEnumerable<IFrontChannelBinding> GetAllFrontChannelBindings() =>
        serviceProvider.GetKeyedServices<IFrontChannelBinding>(Scheme.Name)
        .Union(serviceProvider.GetServices<IFrontChannelBinding>());

    private IFrontChannelBinding GetFrontChannelBinding(string uri) =>
        GetAllFrontChannelBindings().First(b => b.Identifier == uri);

    /// <summary>
    /// Resolves events as keyed service from DI, falls back to creating an events instance.
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

    private IdentityProvider? effectiveIdentityProvider = null;
    private async Task<IdentityProvider> GetEffectiveIdentityProviderAsync()
    {
        if (effectiveIdentityProvider == null)
        {
            var resolver = GetRequiredService<IIdentityProviderConfigurationResolver>();

            IdentityProviderConfigurationResolverContext context = new()
            {
                HttpContext = Context,
                // TODO: Plus package should use callback to resolve this.
                StaticConfiguration = Options.IdentityProvider
                    ?? throw new InvalidOperationException("Missing IdentityProvider config")
            };

            await resolver.GetEffectiveConfigurationAsync(context);

            effectiveIdentityProvider = context.EffectiveConfiguration
                ?? throw new InvalidOperationException("No effective configuration returned");
        }

        return effectiveIdentityProvider;
    }

    /// <summary>
    /// Handles incoming request on the callback path.
    /// </summary>
    /// <returns></returns>
    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var bindings = GetAllFrontChannelBindings();

        var binding = bindings.FirstOrDefault(b => b.CanUnBind(Context.Request));

        if (binding == null)
        {
            return HandleRequestResult.Fail("No binding could find a Saml message in the request");
        }

        var samlMessage = await binding.UnBindAsync(Context.Request,
            async str => await GetEffectiveIdentityProviderAsync());

        AuthenticationProperties authenticationProperties =
            ProcessResponseRelayState(samlMessage);

        var effectiveIdentityProvider = await GetEffectiveIdentityProviderAsync();

        var source = XmlHelpers.GetXmlTraverser(samlMessage.Xml);
        var reader = GetRequiredService<ISamlXmlReader>();
        reader.TrustedSigningKeys = effectiveIdentityProvider.SigningKeys;
        reader.AllowedAlgorithms = effectiveIdentityProvider.AllowedAlgorithms;
        var samlResponse = reader.ReadResponse(source);

        var validator = GetRequiredService<IValidator<Response, ResponseValidationParameters>>();

        // TODO: Do proper validation! + Tests!
        // - TrustLevel
        ResponseValidationParameters validationParameters = new()
        {
            AssertionValidationParameters = new()
            {
                ValidInResponseTo = authenticationProperties.Items[RequestIdKey],
                ValidIssuer = effectiveIdentityProvider.EntityId,
                ValidAudience = Options.EntityId!.Value,
                ValidRecipient = GetAbsoluteUrl(Options.CallbackPath),
            },
        };

        var validatedResponse = samlResponse.Validate(validator, validationParameters);

        // TODO: Handle multiple assertions.
        var validatedAssertion = validatedResponse.GetValid(r => r.Assertions.Single());

        var claimsFactory = GetRequiredService<IClaimsFactory>();

        var identity = claimsFactory.GetClaimsIdentity(validatedAssertion);

        AuthenticationTicket authenticationTicket = new(new(identity), authenticationProperties, Scheme.Name);

        return HandleRequestResult.Success(authenticationTicket);
    }

    private AuthenticationProperties ProcessResponseRelayState(Saml2Message samlMessage)
    {
        if (string.IsNullOrEmpty(samlMessage.RelayState))
        {
            throw new ValidationException<Saml2Message>("No RelayState found.");
        }

        var dotIndex = samlMessage.RelayState.IndexOf('.');
        var idpEntityIdHash = samlMessage.RelayState[..dotIndex];
        var inResponseTo = samlMessage.RelayState[(dotIndex + 1)..];

        var cookieName = CookiePrefix + idpEntityIdHash;

        var cookieValue = Options.StateCookieManager.GetRequestCookie(Context, cookieName);

        if (string.IsNullOrEmpty(cookieValue))
        {
            throw new ValidationException<Saml2Message>($"The state cookie {cookieName} was not found.");
        }

        try
        {
            var authenticationProperties = Options.StateCookieDataFormat.Unprotect(cookieValue)
                ?? throw new InvalidOperationException("Failed to unprotect state cookie");

            var idpEntityId = authenticationProperties.Items[IdpKey];

            var hashedStoredIdpEntityId = idpEntityId.Sha256(StateIdpHashLength);

            if (idpEntityIdHash != hashedStoredIdpEntityId)
            {
                throw new ValidationException<Saml2Message>(
                    $"Hash from RelayState {idpEntityIdHash} does not match {hashedStoredIdpEntityId} calculated from {idpEntityId}");
            }

            var storedRequestId = authenticationProperties.Items[RequestIdKey];

            if (storedRequestId != inResponseTo)
            {
                throw new ValidationException<Saml2Message>(
                    $"RelayState InResponseTo {inResponseTo} doens't matched stored request Id {storedRequestId}");
            }

            return authenticationProperties;
        }
        finally
        {
            Options.StateCookieManager.DeleteCookie(Context, cookieName, BuildCookieOptions());
        }
    }

    private string GetAbsoluteUrl(PathString callbackPath) =>
        $"{Request.Scheme}://{Request.Host}{callbackPath}";

    private CookieOptions BuildCookieOptions() =>
        Options.CorrelationCookie.Build(Context, TimeProvider.GetUtcNow());

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

        var effectiveIdentityProvider = await GetEffectiveIdentityProviderAsync();

        var authnRequestGeneratedContext = new AuthnRequestGeneratedContext(
            Context, Scheme, Options, properties, authnRequest, effectiveIdentityProvider);

        await Events.AuthnRequestGeneratedAsync(authnRequestGeneratedContext);

        // Capture the security sensitive state after the event
        properties.Items[RequestIdKey] = authnRequest.Id;
        properties.Items[IdpKey] = effectiveIdentityProvider!.EntityId;

        var xmlDoc = GetRequiredService<ISamlXmlWriter>().Write(authnRequest);

        var idpEntityIdHash = effectiveIdentityProvider.EntityId!.Sha256(StateIdpHashLength);
        var message = new Saml2Message
        {
            Destination = effectiveIdentityProvider!.SsoServiceUrl!,
            Name = Constants.SamlRequest,
            Xml = xmlDoc.DocumentElement!,
            RelayState = $"{idpEntityIdHash}.{authnRequest.Id}",
            Binding = Constants.BindingUris.HttpRedirect
        };

        // TODO: If needed: make an alternative to this to allow multiple concurrent sign in attempts to same Idp
        var cookieName = CookiePrefix + idpEntityIdHash;
        var cookieValue = Options.StateCookieDataFormat.Protect(properties);
        var cookieOptions = BuildCookieOptions();

        Options.StateCookieManager.AppendResponseCookie(Context, cookieName, cookieValue, cookieOptions);

        var binding = GetFrontChannelBinding(effectiveIdentityProvider.SsoServiceBinding!);

        await binding.BindAsync(Response, message);
    }
}
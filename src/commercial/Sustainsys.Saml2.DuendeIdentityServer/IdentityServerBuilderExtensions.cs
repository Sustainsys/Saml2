// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;
using Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using Sustainsys.Saml2.Serialization;

// Builder extensions are by convention placed in the Microsoft.Extensions.DependencyInjection namespace
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for Saml2 for IdentityServerBuilder.
/// </summary>
public static class IdentityServerBuilderExtensions
{
    /// <summary>
    /// Add Saml2 protocol support to IdentityServer
    /// </summary>
    /// <param name="identityServerBuilder">IdentityServerBuilder</param>
    /// <returns>IdentityServerBuilder</returns>
    public static IIdentityServerBuilder AddSaml2(this IIdentityServerBuilder identityServerBuilder)
    {
        identityServerBuilder.AddEndpoint<SingleSignOnServiceEndpoint>(
            "Saml2 SingleSignOnService", "/saml2/sso");

        // Use singletons for stateless light weight services
        identityServerBuilder.Services.TryAddEnumerable(
            new ServiceDescriptor(typeof(IFrontChannelBinding), typeof(HttpRedirectBinding), ServiceLifetime.Singleton));
        identityServerBuilder.Services.TryAddEnumerable(
            new ServiceDescriptor(typeof(IFrontChannelBinding), typeof(HttpPostBinding), ServiceLifetime.Singleton));
        identityServerBuilder.Services.TryAddSingleton<IHttpResponseWriter<Saml2FrontChannelResult>, Saml2FrontChannelHttpWriter>();
        identityServerBuilder.Services.TryAddSingleton<IHttpResponseWriter<LoginPageResult>, LoginPageResultHttpWriter>();
        identityServerBuilder.Services.TryAddSingleton<ISamlXmlWriterPlus, SamlXmlWriterPlus>();
        identityServerBuilder.Services.AddSingleton<IPostConfigureOptions<IdentityServerOptions>, PostConfigureIdentityServerOptions>();

        // Use transient for services that have dependencies that might have any lifespan
        identityServerBuilder.Services.TryAddTransient<IAuthnRequestValidator, AuthnRequestValidator>();
        identityServerBuilder.Services.TryAddTransient<ISaml2SsoInteractionResponseGenerator, Saml2SsoInteractionResponseGenerator>();
        identityServerBuilder.Services.TryAddTransient<ISaml2SsoResponseGenerator, Saml2SSoResponseGenerator>();
        identityServerBuilder.Services.TryAddTransient<ISaml2IssuerNameService, Saml2IssuerNameService>();

        // The reader has state and must be transient.
        identityServerBuilder.Services.TryAddTransient<ISamlXmlReaderPlus, SamlXmlReaderPlus>();

        return identityServerBuilder;
    }
}
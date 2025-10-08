// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Microsoft.Extensions.DependencyInjection.Extensions;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
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

        identityServerBuilder.Services.TryAddSingleton<IFrontChannelBinding, HttpRedirectBinding>();
        identityServerBuilder.Services.TryAddSingleton<IFrontChannelBinding, HttpPostBinding>();
        identityServerBuilder.Services.TryAddTransient<ISamlXmlReaderPlus, SamlXmlReaderPlus>();

        return identityServerBuilder;
    }
}
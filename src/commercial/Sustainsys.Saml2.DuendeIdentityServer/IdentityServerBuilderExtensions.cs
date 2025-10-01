// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

namespace Microsoft.Extensions.DependencyInjection;

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

        return identityServerBuilder;
    }
}

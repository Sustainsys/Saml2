// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using Microsoft.Extensions.Options;

namespace Sustainsys.Saml2.DuendeIdentityServer;

internal class PostConfigureIdentityServerOptions : IPostConfigureOptions<IdentityServerOptions>
{
    public void PostConfigure(string? name, IdentityServerOptions options)
    {
        // TODO: See if this can be replaced by dynamically wrapping existing keys in an X509Certificate. This would require
        // the certificates to become binary identical.

        var signingAlgorithms = options.KeyManagement.SigningAlgorithms;

        // If no signing algorithms are configured, add the default RS256
        // with the UseX509Certificate flag enabled.
        if (signingAlgorithms.Count == 0)
        {
            signingAlgorithms.Add(new SigningAlgorithmOptions("RS256") { UseX509Certificate = true });
        }

        // Now make sure that at least one algorithm (if added by us or someone else) has an X509Certificate.
        if (!signingAlgorithms.Any(sa => sa.UseX509Certificate))
        {
            throw new Exception("At least one signing algorithm must have the UseX509Certificate flag enabled when using Saml2");
        }
    }
}
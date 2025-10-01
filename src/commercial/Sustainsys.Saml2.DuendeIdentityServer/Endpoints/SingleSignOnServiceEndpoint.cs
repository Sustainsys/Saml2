// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Hosting;
using Microsoft.AspNetCore.Http;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;
internal class SingleSignOnServiceEndpoint : IEndpointHandler
{
    public Task<IEndpointResult?> ProcessAsync(HttpContext context)
    {
        return Task.FromResult<IEndpointResult?>(null);
    }
}

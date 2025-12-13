// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

internal class MetadataEndpoint(
    IServerUrls serverUrls,
    ISaml2IssuerNameService saml2IssuerNameService,
    IOptions<Saml2Options> saml2Options,
    ISaml2MetadataResponseGenerator metadataResponseGenerator)
    : IEndpointHandler
{
    public async Task<IEndpointResult?> ProcessAsync(HttpContext context)
    {
        var issuer = await saml2IssuerNameService.GetCurrentAsync();
        var options = saml2Options.Value;
        var baseUrl = serverUrls.BaseUrl;

        if (!options.Metadata.Enabled)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        // validate HTTP
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        return await metadataResponseGenerator.GenerateMetadataAsync(issuer, options, baseUrl);
    }
}

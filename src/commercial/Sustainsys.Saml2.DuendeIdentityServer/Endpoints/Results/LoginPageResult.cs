// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;

/// <summary>
/// Result when Saml2 single sign on resulted in login being required.
/// </summary>
public class LoginPageResult : EndpointResult<LoginPageResult>
{
    /// <summary>
    /// AuthnRequest validation context
    /// </summary>
    public required ValidatedAuthnRequest Request { get; init; }

    /// <summary>
    /// Url to redirect to (i.e. the login endpoint)
    /// </summary>
    public required string RedirectUrl { get; init; }

    /// <summary>
    /// Name of the returnUrl parameter to attach to redirect
    /// </summary>
    public required string ReturnUrlParameterName { get; init; }
}

/// <summary>
/// Response writer for redirecting to the login page.
/// </summary>
public class LoginPageResultHttpWriter(
    IdentityServerOptions identityServerOptions)
    : IHttpResponseWriter<LoginPageResult>
{
    /// <inheritdoc/>
    public Task WriteHttpResponse(LoginPageResult result, HttpContext context)
    {
        var encodedUrl = Uri.EscapeDataString(
            context.Request.PathBase + context.Request.Path + context.Request.QueryString);

        var url = identityServerOptions.UserInteraction.LoginUrl
            + "?ReturnUrl=" + encodedUrl;

        context.Response.Redirect(url);

        return Task.CompletedTask;
    }
}
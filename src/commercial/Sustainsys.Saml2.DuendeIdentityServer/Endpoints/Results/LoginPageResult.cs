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
/// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
/// <param name="redirectUrl">Url to redirect to</param>
/// <param name="returnUrlParameter">Name of returnUrl query string param</param>
public class LoginPageResult(
    ValidatedAuthnRequest validatedAuthnRequest,
    string? redirectUrl,
    string? returnUrlParameter)
    : EndpointResult<LoginPageResult>
{
    /// <summary>
    /// AuthnRequest validation context
    /// </summary>
    public ValidatedAuthnRequest Request { get; } = validatedAuthnRequest;

    /// <summary>
    /// Url to redirect to (i.e. the login endpoint)
    /// </summary>
    public string RedirectUrl { get; } = redirectUrl ?? throw new ArgumentNullException(nameof(redirectUrl));

    /// <summary>
    /// Name of the returnUrl parameter to attach to redirect
    /// </summary>
    public string ReturnUrlParameterName { get; } = returnUrlParameter
        ?? throw new ArgumentNullException(nameof(returnUrlParameter));
}

/// <summary>
/// Response writer for redirecting to the login page.
/// </summary>
public class LoginPageResultHttpWriter
    : IHttpResponseWriter<LoginPageResult>
{
    /// <inheritdoc/>
    public Task WriteHttpResponse(LoginPageResult result, HttpContext context)
    {
        var encodedUrl = Uri.EscapeDataString(
            context.Request.PathBase + context.Request.Path + context.Request.QueryString);

        var url = $"{result.RedirectUrl}?{result.ReturnUrlParameterName}={encodedUrl}";

        context.Response.Redirect(url);

        return Task.CompletedTask;
    }
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;

/// <summary>
/// Result from a Saml2 endpoint that wraps a Saml2 message and should be handled by
/// a front channel binding.
/// </summary>
public class Saml2FrontChannelResult : EndpointResult<Saml2FrontChannelResult>
{
    /// <summary>
    /// Contained Saml2 Message
    /// </summary>
    public Saml2Message? Message { get; set; }

    /// <summary>
    /// Error message if this result is an error.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Entity Id of Sp as received in incoming message, may not be validated.
    /// </summary>
    public string? SpEntityId { get; set; }
}

/// <summary>
/// Write a Saml2 front channel result to the HttpContext
/// </summary>
public class Saml2FrontChannelResultHttpWriter(
    IClock clock,
    IMessageStore<ErrorMessage> errorMessageStore,
    IdentityServerOptions identityServerOptions,
    IEnumerable<IFrontChannelBinding> bindings)
    : IHttpResponseWriter<Saml2FrontChannelResult>
{
    /// <inheritdoc/>
    public async Task WriteHttpResponse(Saml2FrontChannelResult result, HttpContext context)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            var errorMessage = new ErrorMessage()
            {
                Error = "Saml2 error",
                ErrorDescription = result.Error,
                ClientId = result.SpEntityId,
                RequestId = context.TraceIdentifier,
                ActivityId = System.Diagnostics.Activity.Current?.Id
            };

            var message = new Message<ErrorMessage>(errorMessage, clock.UtcNow.UtcDateTime);
            var id = await errorMessageStore.WriteAsync(message);

            var errorUrl = identityServerOptions.UserInteraction.ErrorUrl;

            if (errorUrl.Contains('?'))
            {
                if (!errorUrl.EndsWith('&'))
                {
                    errorUrl += "&";
                }
            }
            else
            {
                errorUrl += "?";
            }

            var url = errorUrl += identityServerOptions.UserInteraction.ErrorIdParameter + "=" + id;
            context.Response.Redirect(url);
            return;
        }

        if (result.Message != null)
        {
            var binding = bindings.Single(b => b.Identifier == result.Message.Binding);

            await binding.BindAsync(context.Response, result.Message);

            return;
        }

        throw new InvalidOperationException("Saml2FrontChannelResponse contains no properties to take action on");
    }
}
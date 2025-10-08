// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

internal class SingleSignOnServiceEndpoint(
    IEnumerable<IFrontChannelBinding> frontChannelBindings,
    ISamlXmlReaderPlus samlXmlReader,
    IClientStore clientStore,
    IUserSession userSession,
    IdentityServerOptions identityServerOptions)
    : IEndpointHandler
{
    public async Task<IEndpointResult?> ProcessAsync(HttpContext context)
    {
        var result = new Saml2FrontChannelResult();

        var binding = frontChannelBindings.Single(b => b.CanUnBind(context.Request));

        var requestMessage = await binding.UnBindAsync(context.Request, s => default!);

        var authnRequest = samlXmlReader.ReadAuthnRequest(new XmlTraverser(requestMessage.Xml));

        if (authnRequest.Issuer == null)
        {
            throw new InvalidOperationException();
        }

        result.SpEntityID = authnRequest.Issuer.Value;

        var client = await clientStore.FindEnabledClientByIdAsync(result.SpEntityID);

        if (client == null)
        {
            result.Error = "Invalid SP EntityID.";
            return result;
        }

        var user = await userSession.GetUserAsync();

        if (user == null)
        {
            var encodedUrl = Uri.EscapeDataString(
                context.Request.PathBase + context.Request.Path + context.Request.QueryString);

            result.RedirectUrl = identityServerOptions.UserInteraction.LoginUrl
                + "?ReturnUrl=" + encodedUrl;
            return result;
        }

        return result;
    }
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.ResponseHandling;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
using Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

internal class SingleSignOnServiceEndpoint(
    IEnumerable<IFrontChannelBinding> frontChannelBindings,
    ISamlXmlReaderPlus samlXmlReader,
    IUserSession userSession,
    IdentityServerOptions identityServerOptions,
    IAuthnRequestValidator authnRequestValidator,
    ISaml2SsoInteractionResponseGenerator interactionResponseGenerator,
    ISaml2SsoResponseGenerator responseGenerator,
    ISaml2IssuerNameService saml2IssuerNameService)
    : IEndpointHandler
{
    public async Task<IEndpointResult?> ProcessAsync(HttpContext context)
    {
        var binding = frontChannelBindings.Single(b => b.CanUnBind(context.Request));
        var requestMessage = await binding.UnBindAsync(context.Request, s => default!);
        var authnRequest = samlXmlReader.ReadAuthnRequest(new XmlTraverser(requestMessage.Xml));

        var user = await userSession.GetUserAsync();

        ValidatedAuthnRequest validatedAuthnRequest = new()
        {
            AuthnRequest = authnRequest,
            Binding = binding.Identifier,
            Saml2Message = requestMessage,
            Subject = user,
            Saml2IdpEntityId = await saml2IssuerNameService.GetCurrentAsync(),
        };

        var requestValidationResult = await authnRequestValidator.ValidateAsync(validatedAuthnRequest);

        if (requestValidationResult.IsError)
        {
            return new Saml2FrontChannelResult()
            {
                Error = requestValidationResult.ErrorDescription,
                SpEntityId = requestValidationResult.ValidatedRequest.AuthnRequest.Issuer?.Value,
            };
        }

        var interactionResponse = await interactionResponseGenerator.ProcessInteractionAsync(validatedAuthnRequest);

        var interactionResult = CreateInteractionResult(validatedAuthnRequest, interactionResponse);

        if (interactionResult != null)
        {
            return interactionResult;
        }

        var response = await responseGenerator.CreateResponse(validatedAuthnRequest);

        return response;
    }

    private LoginPageResult? CreateInteractionResult(ValidatedAuthnRequest validatedAuthnRequest, InteractionResponse interactionResponse)
    {
        if (interactionResponse.IsLogin)
        {
            return new LoginPageResult(
                validatedAuthnRequest,
                identityServerOptions.UserInteraction.LoginUrl,
                identityServerOptions.UserInteraction.LoginReturnUrlParameter);
        };

        return null;
    }
}
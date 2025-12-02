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
    ISamlXmlWriterPlus samlXmlWriter,
    IAuthnRequestValidator authnRequestValidator,
    ISaml2SsoInteractionResponseGenerator interactionResponseGenerator,
    IClock clock)
    : IEndpointHandler
{
    const string certPath = "Sustainsys.Saml2.Tests.pfx";
    private static readonly X509Certificate2 signingCertificate =
#if NET9_0_OR_GREATER
        X509CertificateLoader.LoadPkcs12FromFile(certPath, "", X509KeyStorageFlags.EphemeralKeySet);
#else
        new(certPath);
#endif

    public async Task<IEndpointResult?> ProcessAsync(HttpContext context)
    {
        var result = new Saml2FrontChannelResult();

        var binding = frontChannelBindings.Single(b => b.CanUnBind(context.Request));
        var requestMessage = await binding.UnBindAsync(context.Request, s => default!);
        var authnRequest = samlXmlReader.ReadAuthnRequest(new XmlTraverser(requestMessage.Xml));

        var user = await userSession.GetUserAsync();

        ValidatedAuthnRequest validatedAuthnRequest = new()
        {
            AuthnRequest = authnRequest,
            Binding = binding.Identifier,
            Saml2Message = requestMessage,
            Subject = user
        };

        var requestValidationResult = await authnRequestValidator.ValidateAsync(validatedAuthnRequest);

        if (requestValidationResult.IsError)
        {
            result.Error = requestValidationResult.ErrorDescription;
            result.SpEntityId = requestValidationResult.ValidatedRequest.AuthnRequest.Issuer?.Value;
        }

        var interaction = await interactionResponseGenerator.ProcessInteractionAsync(validatedAuthnRequest);

        if (interaction.IsLogin)
        {
            ArgumentNullException.ThrowIfNull(identityServerOptions.UserInteraction.LoginUrl);
            ArgumentNullException.ThrowIfNull(identityServerOptions.UserInteraction.LoginReturnUrlParameter);

            return new LoginPageResult()
            {
                Request = validatedAuthnRequest,
                RedirectUrl = identityServerOptions.UserInteraction.LoginUrl,
                ReturnUrlParameterName = identityServerOptions.UserInteraction.LoginReturnUrlParameter
            };
        }

        var issuer = $"{context.Request.Scheme.ToLowerInvariant()}://{context.Request.Host}/Saml2";
        var destination = requestValidationResult.ValidatedRequest.Saml2Sp!.AsssertionConsumerServices.Single().Location;

        Response response = new()
        {
            Destination = destination,
            Issuer = issuer,
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Success
                }
            },
            IssueInstant = clock.UtcNow.UtcDateTime,
            InResponseTo = authnRequest.Id,
            Assertions =
            {
                new()
                {
                    Issuer = issuer,
                    IssueInstant = clock.UtcNow.UtcDateTime,
                    Subject = new()
                    {
                        NameId = new()
                        {
                            Value = user!.FindFirstValue("sub")!,
                            Format = Constants.NameIdFormats.Unspecified
                        },
                        SubjectConfirmation = new()
                        {
                            Method = Constants.SubjectConfirmationMethods.Bearer,
                            SubjectConfirmationData = new()
                            {
                                NotOnOrAfter = clock.UtcNow.UtcDateTime.AddMinutes(5),
                                Recipient = destination,
                                InResponseTo = authnRequest.Id
                            }
                        }
                    },
                    Conditions = new()
                    {
                        NotOnOrAfter = clock.UtcNow.UtcDateTime.AddMinutes(5),
                        AudienceRestrictions =
                        {
                            new()
                            {
                                Audiences = { requestValidationResult.ValidatedRequest.Saml2Sp.EntityId }
                            }
                        }
                    },
                    AuthnStatement = new()
                    {
                        AuthnInstant = clock.UtcNow.UtcDateTime,
                        AuthnContext = new()
                        {
                            AuthnContextClassRef = Constants.AuthnContextClasses.Unspecified
                        }
                    }
                }
            }
        };

        result.Message = new()
        {
            Destination = destination,
            Name = Constants.SamlResponse,
            RelayState = requestMessage.RelayState,
            Xml = samlXmlWriter.Write(response).DocumentElement!,
            SigningCertificate = signingCertificate,
            Binding = Constants.BindingUris.HttpPOST
        };

        return result;
    }
}
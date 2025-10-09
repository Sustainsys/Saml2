// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Hosting;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

internal class SingleSignOnServiceEndpoint(
    IEnumerable<IFrontChannelBinding> frontChannelBindings,
    ISamlXmlReaderPlus samlXmlReader,
    IClientStore clientStore,
    IUserSession userSession,
    IdentityServerOptions identityServerOptions,
    ISamlXmlWriterPlus samlXmlWriter,
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

        if (authnRequest.Issuer == null)
        {
            throw new InvalidOperationException();
        }

        result.SpEntityID = authnRequest.Issuer.Value;

        var client = await clientStore.FindEnabledClientByIdAsync(result.SpEntityID);

        if (client == null || client.ProtocolType != Saml2Constants.Saml2Protocol)
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

        var issuer = $"{context.Request.Scheme.ToLowerInvariant()}://{context.Request.Host}/Saml2";
        var destination = client.RedirectUris.Single();
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
                            Value = user.FindFirstValue("sub")!,
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
                                Audiences = { result.SpEntityID }
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
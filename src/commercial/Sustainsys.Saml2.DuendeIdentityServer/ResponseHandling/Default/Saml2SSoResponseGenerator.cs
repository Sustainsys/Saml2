// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;

/// <inheritdoc/>
public class Saml2SSoResponseGenerator(
    ISaml2IssuerNameService saml2IssuerNameService,
    IClock clock,
    ISamlXmlWriterPlus samlXmlWriter) 
    : ISaml2SsoResponseGenerator
{

    const string certPath = "Sustainsys.Saml2.Tests.pfx";
    private static readonly X509Certificate2 signingCertificate =
#if NET9_0_OR_GREATER
        X509CertificateLoader.LoadPkcs12FromFile(certPath, "", X509KeyStorageFlags.EphemeralKeySet);
#else
        new(certPath);
#endif

    /// <inheritdoc/>
    public async Task<Saml2FrontChannelResult> CreateResponse(ValidatedAuthnRequest validatedAuthnRequest)
    {
        var response = await CreateSaml2Response(validatedAuthnRequest);

        return new()
        {
            Message = new()
            {
                Destination = response.Destination!,
                Name = Constants.SamlResponse,
                RelayState = validatedAuthnRequest.Saml2Message.RelayState,
                Xml = samlXmlWriter.Write(response).DocumentElement!,
                SigningCertificate = signingCertificate,
                Binding = Constants.BindingUris.HttpPOST
            }
        };
    }

    /// <summary>
    /// Create the Saml2 response.
    /// </summary>
    /// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
    /// <returns>The Saml2 Response object</returns>
    protected virtual async Task<Response> CreateSaml2Response(ValidatedAuthnRequest validatedAuthnRequest)
    {
        var issuer = await saml2IssuerNameService.GetCurrentAsync();
        var destination = validatedAuthnRequest.Saml2Sp!.AsssertionConsumerServices.Single().Location;

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
            InResponseTo = validatedAuthnRequest.AuthnRequest.Id,
            Assertions =
            { 
                CreateAssertion(validatedAuthnRequest, issuer, destination)
            }
        };
        return response;
    }

    /// <summary>
    /// Create the Assertion
    /// </summary>
    /// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
    /// <param name="issuer">The issuer to use</param>
    /// <param name="destination">Destination URL</param>
    /// <returns>Assertion</returns>
    protected virtual Saml.Assertion CreateAssertion(ValidatedAuthnRequest validatedAuthnRequest, string issuer, string destination)
        => new()
        {
            Issuer = issuer,
            IssueInstant = clock.UtcNow.UtcDateTime,
            Subject = CreateSubject(validatedAuthnRequest, destination),
            Conditions = CreateConditions(validatedAuthnRequest),
            AuthnStatement = CreateAuthnStatement(),
        };

    /// <summary>
    /// Create the AuthnStatement
    /// </summary>
    /// <returns>AuthnStatement</returns>
    protected virtual Saml.AuthnStatement CreateAuthnStatement()
        => new()
        {
            AuthnInstant = clock.UtcNow.UtcDateTime,
            AuthnContext = new()
            {
                AuthnContextClassRef = Constants.AuthnContextClasses.Unspecified
            }
        };

    /// <summary>
    /// Create the Conditions
    /// </summary>
    /// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
    /// <returns>Conditions</returns>
    protected virtual Saml.Conditions CreateConditions(ValidatedAuthnRequest validatedAuthnRequest)
        => new()
        {
            NotOnOrAfter = clock.UtcNow.UtcDateTime.AddMinutes(5),
            AudienceRestrictions =
            {
                new()
                {
                    Audiences = { validatedAuthnRequest.Saml2Sp!.EntityId }
                }
            }
        };

    /// <summary>
    /// Creat the Subject
    /// </summary>
    /// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
    /// <param name="destination">Destination URL</param>
    /// <returns>Subject</returns>
    protected virtual Saml.Subject CreateSubject(ValidatedAuthnRequest validatedAuthnRequest, string destination)
    {
        return new()
        {
            NameId = new()
            {
                Value = validatedAuthnRequest.Subject!.FindFirstValue(JwtClaimTypes.Subject)!,
                Format = Constants.NameIdFormats.Unspecified
            },
            SubjectConfirmation = new()
            {
                Method = Constants.SubjectConfirmationMethods.Bearer,
                SubjectConfirmationData = new()
                {
                    NotOnOrAfter = clock.UtcNow.UtcDateTime.AddMinutes(5),
                    Recipient = destination,
                    InResponseTo = validatedAuthnRequest.AuthnRequest.Id
                }
            }
        };
    }
}

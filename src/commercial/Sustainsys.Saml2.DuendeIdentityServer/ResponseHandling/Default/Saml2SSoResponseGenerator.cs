// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
using Sustainsys.Saml2.DuendeIdentityServer.Services;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;

/// <summary>
/// Response Generator for Saml2 Single Sign On.
/// </summary>
/// <param name="saml2IssuerNameService">Issuer name service for Saml2</param>
/// <param name="clock">Clock</param>
/// <param name="samlXmlWriter">Xml Writer/serializer</param>
/// <param name="profileService">Profile Service</param>
public class Saml2SSoResponseGenerator(
    ISaml2IssuerNameService saml2IssuerNameService,
    IClock clock,
    ISamlXmlWriterPlus samlXmlWriter,
    IProfileService profileService)
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
                await CreateAssertionAsync(validatedAuthnRequest, issuer, destination)
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
    protected virtual async Task<Saml.Assertion> CreateAssertionAsync(ValidatedAuthnRequest validatedAuthnRequest, string issuer, string destination)
        => new()
        {
            Issuer = issuer,
            IssueInstant = clock.UtcNow.UtcDateTime,
            Subject = CreateSubject(validatedAuthnRequest, destination),
            Conditions = CreateConditions(validatedAuthnRequest),
            AuthnStatement = CreateAuthnStatement(),
            Attributes = [.. await CreateAttributesAsync(validatedAuthnRequest)]
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
                // TODO: Map this based on acr/amr claims
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
    /// Create the Subject
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

    /// <summary>
    /// Create Attributes
    /// </summary>
    /// <param name="validatedAuthnRequest">AuthnRequest validation context</param>
    /// <returns>Attributes</returns>
    protected virtual async Task<IList<SamlAttribute>> CreateAttributesAsync(ValidatedAuthnRequest validatedAuthnRequest)
    {
        List<Claim> claims = [];

        ArgumentNullException.ThrowIfNull(validatedAuthnRequest.ValidatedResources);

        var requestedClaims = validatedAuthnRequest.ValidatedResources.Resources.IdentityResources
            .SelectMany(ir => ir.UserClaims)
            .Distinct();

        ProfileDataRequestContext profileRequest = new()
        {
            Caller = Saml2Constants.SsoResponseProfileCaller,
            Client = validatedAuthnRequest.Saml2Sp!,
            Subject = validatedAuthnRequest.Subject!,
            ValidatedRequest = validatedAuthnRequest.ToValidatedRequest(),
            RequestedClaimTypes = requestedClaims
        };

        await profileService.GetProfileDataAsync(profileRequest);

        claims.AddRange(profileRequest.IssuedClaims);

        // TODO: Add mapping mechanism for attribute names
        var attributes = claims
            .GroupBy(c => c.Type)
            .Select(g => new SamlAttribute() { Name = g.Key, Values = [.. g.Select(c => c.Value)] })
            .ToList();

        return attributes;
    }
}
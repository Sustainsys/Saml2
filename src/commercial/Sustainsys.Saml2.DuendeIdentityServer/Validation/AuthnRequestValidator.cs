// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
using Sustainsys.Saml2.Samlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Validation;

/// <summary>
/// Validated AuthnRequest
/// </summary>
public class ValidatedAuthnRequest
{
    /// <summary>
    /// The current IdentityServerOptions
    /// </summary>
    public required IdentityServerOptions IdentityServerOptions { get; init; }

    /// <summary>
    /// The AuthnRequest
    /// </summary>
    public required AuthnRequest AuthnRequest { get; init; }

    /// <summary>
    /// Identifier of binding used to read the AuthnRequest
    /// </summary>
    public required string Binding { get; init; }

    /// <summary>
    /// The Saml2 message
    /// </summary>
    public required Saml2Message Saml2Message { get; init; }

    /// <summary>
    /// The Saml2 SP
    /// </summary>
    public Saml2Sp? Saml2Sp { get; set; }

    /// <summary>
    /// The current user
    /// </summary>
    public ClaimsPrincipal? Subject { get; init; }

    /// <summary>
    /// The current SessionId
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    /// The Saml2 identifier for IdentityServer
    /// </summary>
    public required string Saml2IdpEntityId { get; set; }

    /// <summary>
    /// AssertionConsumerService to respond to, set once we have enough validation to be able
    /// to trust it and return error responses to it.
    /// </summary>
    public IndexedEndpoint AssertionConsumerService { get; set; }

    /// <summary>
    /// Resource "validation" results. Used to get list of claims to include in response.
    /// </summary>
    public ResourceValidationResult? ValidatedResources { get; set; }

    /// <summary>
    /// Generate a OIDC ValidatedRequest as good as possible from the ValidatedAuthnRequest.
    /// </summary>
    /// <remarks>
    /// The ValidatedRequest base class is built for OpenId Connect, that's why the ValidatedAuthnRequest
    /// class does not inherit it. But in some cases, e.g. the profile service call, it's better to supply
    /// a ValidatedRequest on a best-effort than to supply nothing.
    /// </remarks>
    /// <returns>ValidatedRequest</returns>
    public ValidatedRequest ToValidatedRequest()
        => new()
        {
            ClientId = Saml2Sp?.EntityId ?? throw new ArgumentNullException(nameof(Saml2Sp)),
            Client = Saml2Sp,
            IssuerName = Saml2IdpEntityId,
            Options = IdentityServerOptions,
            Subject = Subject,
            SessionId = SessionId,
        };
}

/// <summary>
/// Result of AuthnRequestValidation
/// </summary>
public class AuthnRequestValidationResult : ValidationResult
{
    /// <summary>
    /// Creates a valid validation result
    /// </summary>
    /// <param name="validatedAuthnRequest">Validated request</param>
    /// <returns>Valid validation result</returns>
    public static AuthnRequestValidationResult Valid(ValidatedAuthnRequest validatedAuthnRequest)
        => new()
        {
            ValidatedRequest = validatedAuthnRequest,
            IsError = false
        };

    /// <summary>
    /// Creates an invalid validation result.
    /// </summary>
    /// <param name="validatedAuthnRequest">The AuthnRequest validation context</param>
    /// <param name="saml2ErrorCode">Error code (a Saml2 status code)</param>
    /// <param name="errorDescription">Error description</param>
    /// <returns></returns>
    public static AuthnRequestValidationResult InValid(
        ValidatedAuthnRequest validatedAuthnRequest,
        string saml2ErrorCode,
        string? errorDescription = null)
        => new()
        {
            ValidatedRequest = validatedAuthnRequest,
            Error = saml2ErrorCode,
            ErrorDescription = errorDescription
        };

    /// <summary>
    /// The validated request.
    /// </summary>
    public required ValidatedAuthnRequest ValidatedRequest { get; init; }
}


/// <summary>
/// Validator for AuthnRequest
/// </summary>
public interface IAuthnRequestValidator
{
    /// <summary>
    /// Validate an AuthnRequest
    /// </summary>
    /// <param name="request">AuthnRequest validation context</param>
    /// <returns>Validation result</returns>
    public Task<AuthnRequestValidationResult> ValidateAsync(ValidatedAuthnRequest request);
}

/// <summary>
/// AuthnRequest validator
/// </summary>
public class AuthnRequestValidator(
    IClientStore clientStore,
    IResourceValidator resourceValidator)
    : IAuthnRequestValidator
{
    /// <inheritdoc />
    public async Task<AuthnRequestValidationResult> ValidateAsync(ValidatedAuthnRequest request)
    {
        var spResult = await ValidateSpAsync(request);
        if (spResult.IsError)
        {
            return spResult;
        }

        var resourceResult = await ValidateResourcesAsync(request);
        if (resourceResult.IsError)
        {
            {
                return resourceResult;
            }
        }

        return AuthnRequestValidationResult.Valid(request);
    }

    /// <summary>
    /// Validate the SP/Client.
    /// </summary>
    /// <param name="request">AuthnRequest validation context</param>
    /// <returns>Validation result</returns>
    protected virtual async Task<AuthnRequestValidationResult> ValidateSpAsync(ValidatedAuthnRequest request)
    {
        if (request.AuthnRequest.Issuer == null)
        {
            return AuthnRequestValidationResult.InValid(request, Constants.StatusCodes.Requester, "Missing SP EntityID in AuthnRequest");
        }

        var spEntityId = request.AuthnRequest.Issuer.Value;

        var client = await clientStore.FindEnabledClientByIdAsync(spEntityId);

        if (client == null || client.ProtocolType != Saml2Constants.Saml2Protocol)
        {
            return AuthnRequestValidationResult.InValid(request, Constants.StatusCodes.Requester, "Invalid SP EntityId.");
        }

        request.Saml2Sp = client.AsSaml2Sp();

        return AuthnRequestValidationResult.Valid(request);
    }

    /// <summary>
    /// "Validates" the request resources
    /// </summary>
    /// <param name="request">AuthnRequest validation context</param>
    /// <returns>Validation results</returns>
    /// <remarks>
    /// Most saml2 deployments do not use any mechanism for the SP/client to request specific claims/attributes,
    /// so we use the AllowedScopes to indicate what IdentityResources should be used to generate claims. The
    /// "validation" is done because the validator also loads the scopes and extracts the list of claims.
    /// </remarks>
    protected virtual async Task<AuthnRequestValidationResult> ValidateResourcesAsync(ValidatedAuthnRequest request)
    {
        ResourceValidationRequest resourceValidationRequest = new()
        {
            Client = request.Saml2Sp ?? throw new ArgumentNullException(nameof(request.Saml2Sp)),
            Scopes = request.Saml2Sp.IdentityResources
        };

        var result = await resourceValidator.ValidateRequestedResourcesAsync(resourceValidationRequest);

        // There is really no way this could fail because we just validate the allowed list. But let's be safe.
        if (!result.Succeeded)
        {
            return AuthnRequestValidationResult.InValid(request, Constants.StatusCodes.Responder, "IdentityResource configuration error");
        }

        request.ValidatedResources = result;

        return AuthnRequestValidationResult.Valid(request);
    }
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Validation;
using Sustainsys.Saml2.Bindings;
using Sustainsys.Saml2.DuendeIdentityServer.Models;
using Sustainsys.Saml2.Samlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Validation;

/// <summary>
/// Validated AuthnRequest
/// </summary>
public class ValidatedAuthnRequest
{
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
    /// AssertionConsumerService to respond to, set once we have enough validation to be able
    /// to trust it and return error responses to it.
    /// </summary>
    public IndexedEndpoint AssertionConsumerService { get; set; }
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
public class AuthnRequestValidator
    (IClientStore clientStore)
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
}
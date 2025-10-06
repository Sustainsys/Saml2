// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Validation;


/// <summary>
/// DTO carrying parameters for Saml response validation.
/// </summary>
public class ResponseValidationParameters
{
    /// <summary>
    /// Validation parameters for assertions embedded in the response.
    /// </summary>
    public required AssertionValidationParameters AssertionValidationParameters { get; set; }

    /// <summary>
    /// Valid issuer of the response. Returns the ValidIssuer of the embedded
    /// SamlAssertionValidationParameters to ensure they are the same.
    /// </summary>
    /// <remarks>
    /// Deliberately only a plain string here as the issuer value and not
    /// a full <see cref="NameId"/> as there are special rules for the format
    /// when a <see cref="NameId"/> is used as an Issuer.
    /// </remarks>
    public string? ValidIssuer { get => AssertionValidationParameters.ValidIssuer; }

    /// <summary>
    ///  A URI reference that identifies an intended audience.
    /// </summary>
    public string? ValidAudience { get; set; }

    /// <summary>
    /// Valid destination of the response and assertions. 
    /// A URI reference indicating the address to which this request has been sent.
    /// </summary>
    public string? ValidDestination { get => AssertionValidationParameters.ValidRecipient; }

    /// <summary>
    /// Valid/Expected InResponseTo
    /// </summary>
    public string? ValidInResponseTo { get => AssertionValidationParameters.ValidInResponseTo; }
}
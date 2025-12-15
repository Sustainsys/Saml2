// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.ResponseHandling;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;

/// <summary>
/// Interaction response generator for Saml2 AuthnRequests
/// </summary>
public interface ISaml2SsoInteractionResponseGenerator
{
    /// <summary>
    /// Process a validated AuthnRequest and decide what/if interaction is required.
    /// </summary>
    /// <param name="request">Validated AuthnRequest</param>
    /// <returns>Interaction</returns>
    Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthnRequest request);
}
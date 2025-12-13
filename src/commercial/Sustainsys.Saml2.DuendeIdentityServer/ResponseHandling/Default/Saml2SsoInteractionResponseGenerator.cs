// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Endpoints.Results;
using Duende.IdentityServer.ResponseHandling;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;

/// <inheritdoc/>
public class Saml2SsoInteractionResponseGenerator : ISaml2SsoInteractionResponseGenerator
{
    /// <inheritdoc/>
    public async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthnRequest request)
    {
        // TODO: Check ProfileService.IsActiveAsync

        if (request.Subject == null)
        {
            return new InteractionResponse()
            {
                IsLogin = true
            };
        }

        return new InteractionResponse();
    }
}
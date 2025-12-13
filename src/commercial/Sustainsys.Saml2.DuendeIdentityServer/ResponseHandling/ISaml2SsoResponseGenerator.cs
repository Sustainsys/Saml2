// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.DuendeIdentityServer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;

/// <summary>
/// Response generator for Saml2 Single Sign On
/// </summary>
public interface ISaml2SsoResponseGenerator
{
    /// <summary>
    /// Create a response for validated AuthnRequest
    /// </summary>
    /// <param name="validatedAuthnRequest">Validated AuthnRequest</param>
    /// <returns>Saml2 front channel response</returns>
    Task<Saml2FrontChannelResult> CreateResponse(ValidatedAuthnRequest validatedAuthnRequest);
}
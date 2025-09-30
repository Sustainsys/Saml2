// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Samlp;

namespace Sustainsys.Saml2.AspNetCore.Events;

/// <summary>
/// Context for AuthnRequestGenerated event
/// </summary>
/// <param name="context">Http Context</param>
/// <param name="scheme">Authentication Scheme</param>
/// <param name="options">Options</param>
/// <param name="properties">Authentication Properties</param>
/// <param name="authnRequest">AuthnRequest</param>
/// <param name="identityProvider">The IdentityProvider the request is being generated for</param>
public class AuthnRequestGeneratedContext(
    HttpContext context,
    AuthenticationScheme scheme,
    Saml2Options options,
    AuthenticationProperties properties,
    AuthnRequest authnRequest,
    IdentityProvider identityProvider)
    : PropertiesContext<Saml2Options>(context, scheme, options, properties)
{

    /// <summary>
    /// The generated AuthnRequest
    /// </summary>
    public AuthnRequest AuthnRequest { get; set; } = authnRequest;

    /// <summary>
    /// The IdentityProvider the request is being generated for.
    /// </summary>
    public IdentityProvider IdentityProvider { get; } = identityProvider;
}
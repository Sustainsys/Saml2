// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Saml2 authentication options
/// </summary>
public class Saml2Options : RemoteAuthenticationOptions
{
    /// <summary>
    /// Ctor
    /// </summary>
    public Saml2Options()
    {
        BasePath = Saml2Defaults.BasePath;

        // Other providers use 15 minutes, which sometimes causes problems for login
        // flows that includes a password reset/recovery or waiting for an email. But
        // with a higher value, there's a risk the user gets too many cookies created so
        // I guess that's why they stick to 15 minutes default.
        // We can allow a full hour thanks to overwriting the cookie on repeated login attempts.
        RemoteAuthenticationTimeout = TimeSpan.FromHours(1);
    }

    /// <summary>
    /// Events can be used to override behaviour. Setting this property is the easy way.
    /// To resolve from DI register Saml2Events as a keyed service with the scheme name
    /// as the key, or to use the same events for all schemes register as an normal
    /// service
    /// </summary>
    public new Saml2Events Events
    {
        get => (Saml2Events)base.Events;
        set => base.Events = value;
    }

    /// <summary>
    /// Identityprovider configuration for this scheme.
    /// </summary>
    public IdentityProvider IdentityProvider { get; set; } = new();

    /// <summary>
    /// Base path for default paths for endpoints. Defaults to /Saml2.
    /// </summary>
    public string BasePath { get; set; }

    /// <summary>
    /// NameId of the service provider.
    /// </summary>
    public NameId? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the type used to encrypt and sign data for the state cookie used across redirects.
    /// </summary>
    public ISecureDataFormat<AuthenticationProperties> StateCookieDataFormat { get; set; } = default!;

    /// <summary>
    /// Gets or sets the cookie manager that is used for the state data cookie.
    /// </summary>
    /// <remarks>
    /// Saml2 has a strict limitation on relayState size, so we cannot pass the encrypted 
    /// AuthenticationProperties as state in the redirect like OpenIdConnect does. We need
    /// to store it all in a cookie and that cookie can become large. So we're using the
    /// CookieManager to get chunking support.
    /// </remarks>
    public ICookieManager StateCookieManager { get; set; } = default!;
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Post configure options for Saml2. Validates config and sets the default 
/// for parameters that have not been set.
/// </summary>
public class Saml2PostConfigureOptions(IDataProtectionProvider dataProtectionProvider) : IPostConfigureOptions<Saml2Options>
{
    /// <summary>
    /// Validates config and sets the default for parameters that have not
    /// been set
    /// </summary>
    /// <param name="name">Name of the scheme</param>
    /// <param name="options">Options to validate and set defaults on</param>
    public void PostConfigure(string? name, Saml2Options options)
    {
        options.DataProtectionProvider ??= dataProtectionProvider;

        options.StateCookieDataFormat ??=
            new PropertiesDataFormat(options.DataProtectionProvider!.CreateProtector(
                typeof(Saml2Handler).FullName!, name!, "v3"));

        options.StateCookieManager ??= new ChunkingCookieManager();

        if (options.CallbackPath == null)
        {
            options.CallbackPath = new PathString(options.BasePath + Saml2Defaults.CallbackPathSuffix);
        }
    }
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Services;

/// <summary>
/// Access to current IdentityServer Entity Id.
/// </summary>
public interface ISaml2IssuerNameService
{
    /// <summary>
    /// Get the current IdentityServer Entity Id
    /// </summary>
    /// <returns>Entity Id string</returns>
    Task<string> GetCurrentAsync();
}

internal class Saml2IssuerNameService(
    IIssuerNameService issuerNameService,
    IOptions<Saml2Options> options)
    : ISaml2IssuerNameService
{
    public async Task<string> GetCurrentAsync()
    {
        return options.Value.EntityId
            ?? (await issuerNameService.GetCurrentAsync()) + options.Value.EntityIdPath;
    }
}

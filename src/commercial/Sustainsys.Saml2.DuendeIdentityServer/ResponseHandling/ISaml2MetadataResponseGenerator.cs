// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling;

/// <summary>
/// Saml2 metadata respones generator
/// </summary>
public interface ISaml2MetadataResponseGenerator
{
    /// <summary>
    /// Generates metadata response.
    /// </summary>
    /// <param name="issuer">Entity Id of IdentityServer</param>
    /// <param name="options">Saml2 options</param>
    /// <param name="baseUrl">Base url of IdentityServer</param>
    /// <returns></returns>Gener
    Task<Saml2MetadataResult> GenerateMetadataAsync(
        string issuer, Saml2Options options, string baseUrl);
}
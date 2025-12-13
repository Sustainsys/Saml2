// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer;

/// <summary>
/// Options for Saml2
/// </summary>
public class Saml2Options
{
    /// <summary>
    /// The Entity Id of this Saml2 IdentityProvider. Default to null to derive
    /// from the OIDC Issuer.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Path component of the EntityId if created from the OIDC issuer. Ignored if
    /// <see cref="EntityIdPath"/> is set.
    /// </summary>
    public string EntityIdPath { get; set; } = Saml2Constants.Defaults.Saml2Path;

    /// <summary>
    /// Options controlling endpoints
    /// </summary>
    public EndpointsOptions Endpoints { get; set; } = new();

    /// <summary>
    /// Options for endpoints
    /// </summary>
    public class EndpointsOptions
    {
        /// <summary>
        /// Bindings supported for SingleSignOnService. Set to empty to
        /// disable endpoint.
        /// </summary>
        public ICollection<string> SingleSignOnServiceBindings =
            [
                Constants.BindingUris.HttpRedirect,
                Constants.BindingUris.HttpPOST
            ];

        /// <summary>
        /// Path for the SingleSignOnService Endpoint.
        /// </summary>
        public string SingleSignOnServicePath { get; set; } 
            = Saml2Constants.Defaults.SingleSignOnServicePath;

        /// <summary>
        /// Path of Metadata document.
        /// </summary>
        public string MetadataPath { get; set; } = Saml2Constants.Defaults.Saml2Path;

    }

    /// <summary>
    /// Options for Metadata generation
    /// </summary>
    public MetadataOptions Metadata { get; } = new();

    /// <summary>
    /// Options for Metadata generation
    /// </summary>
    public class MetadataOptions
    {
        /// <summary>
        /// Is Saml2 metadata exposing enabled? Defaults to ture.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Cache duration for generated metadata.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(12);

        /// <summary>
        /// Absolute expiry for generated metadata.
        /// </summary>
        public TimeSpan ExpiryDuration { get; set; } = TimeSpan.FromDays(5);
    }
}

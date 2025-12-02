// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

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
    /// <see cref="EntityId"/> is set.
    /// </summary>
    public string EntityIdPath { get; set; } = Saml2Constants.Defaults.Saml2Path;
}

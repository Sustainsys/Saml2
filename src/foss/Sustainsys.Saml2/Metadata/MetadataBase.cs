// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Abstract class for Metadata root node.
/// </summary>
public abstract class MetadataBase
{
    /// <summary>
    /// Id of the root node.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Recommended interval for cache renewal.
    /// </summary>
    public TimeSpan? CacheDuration { get; set; }

    /// <summary>
    /// Absolute expiry time of any cached data.
    /// </summary>
    public DateTimeUtc? ValidUntil { get; set; }

    /// <summary>
    /// Trustlevel, set from validation of the signature, if there was one.
    /// </summary>
    public TrustLevel TrustLevel { get; set; }
}

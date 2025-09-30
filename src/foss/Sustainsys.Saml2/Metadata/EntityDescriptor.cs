// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// A Saml2 Metadata &lt;EntityDescriptor&gt; element.
/// </summary>
public class EntityDescriptor
{
    /// <summary>
    /// Id of the Entity. MUST be an absolute URI
    /// </summary>
    public string EntityId { get; set; } = "";

    /// <summary>
    /// Id of the EntityDescriptor node.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Recommended interval for cache renewal.
    /// </summary>
    public TimeSpan? CacheDuraton { get; set; }

    /// <summary>
    /// Absolute expiry time of any cached data.
    /// </summary>
    public DateTimeUtc? ValidUntil { get; set; }

    /// <summary>
    /// The extensions node of the metadata.
    /// </summary>
    public Extensions? Extensions { get; set; }

    /// <summary>
    /// Role Descriptors.
    /// </summary>
    public List<RoleDescriptor> RoleDescriptors { get; } = [];

    /// <summary>
    /// Trustlevel, set from validation of the signature, if there was one.
    /// </summary>
    public TrustLevel TrustLevel { get; set; }
}
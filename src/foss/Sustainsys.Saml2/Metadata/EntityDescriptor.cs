// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// A Saml2 Metadata &lt;EntityDescriptor&gt; element.
/// </summary>
public class EntityDescriptor : MetadataBase
{
    /// <summary>
    /// Id of the Entity. MUST be an absolute URI
    /// </summary>
    public string EntityId { get; set; } = "";

    /// <summary>
    /// The extensions node of the metadata.
    /// </summary>
    public Extensions? Extensions { get; set; }

    /// <summary>
    /// Role Descriptors.
    /// </summary>
    public List<RoleDescriptor> RoleDescriptors { get; } = [];
}
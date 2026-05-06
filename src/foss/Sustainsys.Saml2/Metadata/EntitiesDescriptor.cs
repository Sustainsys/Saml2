// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// A Saml2 Metadata &lt;EntitiesDescriptor&gt; element.
/// </summary>
public class EntitiesDescriptor : MetadataBase
{
    /// <summary>
    /// The extensions node of the metadata
    /// </summary>
    public Extensions? Extensions { get; set; }

    /// <summary>
    /// Nested EntityDescriptor entries.
    /// </summary>
    public List<EntityDescriptor> EntityDescriptors { get; } = [];
}
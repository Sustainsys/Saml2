// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Sustainsys.Saml2.Metadata;


/// <summary>
/// Abstract SSODescriptor type.
/// </summary>
public abstract class SSODescriptor : RoleDescriptor
{
    /// <summary>
    /// Artifact resolution services.
    /// </summary>
    public List<IndexedEndpoint> ArtifactResolutionServices { get; } = [];

    /// <summary>
    /// Single logout services.
    /// </summary>
    public List<Endpoint> SingleLogoutServices { get; } = [];
}
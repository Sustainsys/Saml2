// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Audience Restrictions, Core 2.5.1.4
/// </summary>
public class AudienceRestriction
{
    /// <summary>
    /// Audiences, a list of URIs identifying the audiences.
    /// </summary>
    public List<string> Audiences { get; } = [];
}
// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Saml Attribute, Core 2.7.3.1
/// </summary>
[DebuggerDisplay("{Name,nq}:{AllValues}")]
public class SamlAttribute
{
    /// <summary>
    /// Name of attribute
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Attribute values.
    /// </summary>
    public List<string?> Values { get; init; } = [];

    private string AllValues { get => string.Join(", ", Values); }
}
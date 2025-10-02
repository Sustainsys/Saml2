// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Common;

/// <summary>
/// Extensions
/// </summary>
public class Extensions
{
    /// <summary>
    /// Collection of content nodes read by extension-aware readers.
    /// </summary>
    public List<object> Contents { get; set; } = [];
}
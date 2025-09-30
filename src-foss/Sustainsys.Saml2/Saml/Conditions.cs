// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// Conditions, core 2.5.1
/// </summary>
public class Conditions
{
    /// <summary>
    /// Not before
    /// </summary>
    public DateTimeUtc? NotBefore { get; set; }

    /// <summary>
    /// Not on or after
    /// </summary>
    public DateTimeUtc? NotOnOrAfter { get; set; }

    /// <summary>
    /// Audience restrictions
    /// </summary>
    public List<AudienceRestriction> AudienceRestrictions { get; } = [];

    /// <summary>
    /// One time use.
    /// </summary>
    /// <remarks>
    /// In the XML, this is stored as an empty element. But it makes more
    /// sense in the object model to use a bool. True if the element was
    /// present, false if not.
    /// </remarks>
    public bool OneTimeUse { get; set; }
}
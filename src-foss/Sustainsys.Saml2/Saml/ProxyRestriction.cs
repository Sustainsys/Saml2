// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Saml;
/// <summary>
/// Specifies limitations that the asserting party imposes on relying parties that in turn wish to act as
///asserting parties and issue subsequent assertions on their own on the basis of the information contained
/// in the original assertion.
/// </summary>
public class ProxyRestriction
{
    /// <summary>
    /// Specifies the maximum number of indirections that the asserting party permits to exist.
    /// </summary>
    public int? Count { get; set; }
}
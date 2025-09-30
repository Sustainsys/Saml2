// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// SubjectConfirmation, Core 2.4.1.1
/// </summary>
public class SubjectConfirmation
{
    /// <summary>
    /// Subject Confirmation Method
    /// </summary>
    public string Method { get; set; } = default!;

    /// <summary>
    /// Subject Confirmation Data
    /// </summary>
    public SubjectConfirmationData? SubjectConfirmationData { get; set; }
}
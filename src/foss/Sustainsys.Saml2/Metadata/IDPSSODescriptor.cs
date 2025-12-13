// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// IDPSSODescriptor
/// </summary>
public class IDPSSODescriptor : SSODescriptor
{
    /// <summary>
    /// List of SingleSignOnService endpoints.
    /// </summary>
    public List<Endpoint> SingleSignOnServices { get; } = [];

    /// <summary>
    /// Does the Idp wants any AuthnRequests to be signed?
    /// </summary>
    public bool? WantAuthnRequestsSigned { get; set; }
}
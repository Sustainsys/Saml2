// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Samlp;

/// <summary>
/// A Saml2p SamlResponse
/// </summary>
public class Response : StatusResponseType
{
    /// <summary>
    /// Assertions
    /// </summary>
    public List<Assertion> Assertions { get; } = [];
}
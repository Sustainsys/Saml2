// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Saml;

/// <summary>
/// A Saml assertion
/// </summary>
public class Assertion
{
    /// <summary>
    /// Id of the assertion
    /// </summary>
    public string Id { get; set; } = XmlHelpers.CreateId();

    /// <summary>
    /// Issuer of the assertion.
    /// </summary>
    public NameId Issuer { get; set; } = default!;

    /// <summary>
    /// Version. Must be 2.0
    /// </summary>
    public string Version { get; set; } = "2.0";

    /// <summary>
    /// Isssue instant of the assertion
    /// </summary>
    public DateTimeUtc IssueInstant { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Subject of the assertion
    /// </summary>
    public Subject? Subject { get; set; }

    /// <summary>
    /// Conditions of the assertion
    /// </summary>
    public Conditions? Conditions { get; set; }

    /// <summary>
    /// Authentication Statement
    /// </summary>
    public AuthnStatement? AuthnStatement { get; set; }

    /// <summary>
    /// Attributes
    /// </summary>
    public AttributeStatement Attributes { get; } = [];

    /// <summary>
    /// Trust level derived from the signature validation
    /// </summary>
    public TrustLevel TrustLevel { get; set; }
}
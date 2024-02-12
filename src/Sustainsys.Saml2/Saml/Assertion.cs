using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    public DateTime IssueInstant { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Subject of the assertion
    /// </summary>
    public Subject Subject { get; set; } = default!;

    /// <summary>
    /// Conditions of the assertion
    /// </summary>
    public Conditions? Conditions {  get; set; }
    
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

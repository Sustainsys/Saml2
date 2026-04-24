// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Represents a Saml2 message as seen by the binding.
/// </summary>
public abstract class Saml2Message
{
    /// <summary>
    /// Name of the message to be used in query strings, form fields etc.
    /// This is typically "SamlRequest" or "SamlResponse".
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// RelayState included with the message
    /// </summary>
    public string? RelayState { get; init; }

    /// <summary>
    /// The XML payload.
    /// </summary>
    public required XmlElement Xml { get; init; }

    /// <summary>
    /// Destination URL of the message. For outbound messages the URL
    /// to send the message to. For inbound, the URL the message was
    /// received at.
    /// </summary>
    public required string Destination { get; init; }

    /// <summary>
    /// Binding to use when sending the message, or used when messaged was read.
    /// </summary>
    public required string Binding { get; init; }
}

/// <summary>
/// An outbound Saml2 message as seen by the binding.
/// </summary>
public class OutboundSaml2Message : Saml2Message
{
    /// <summary>
    /// Signing certificate that the message should be signed with. The
    /// method for signing is binding dependent.
    /// </summary>
    public X509Certificate2? SigningCertificate { get; init; }
}

/// <summary>
/// An inbound Saml2 message as received by the binding.
/// </summary>
public class InboundSaml2Message: Saml2Message
{
    // TODO: Add Trustlevel when implementing Redirect binding signatures.
}
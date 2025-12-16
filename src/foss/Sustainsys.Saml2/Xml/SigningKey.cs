// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Xml;

// TODO: Redesign to handle both signing and encryption keys, as well as
// signing validaton keys that validates e.g. thumbprint of the certificate
// that is embedded in the signature.

/// <summary>
/// Represents a signing key.
/// </summary>
public class SigningKey
{
    /// <summary>
    /// The asymmetric algorithm.
    /// </summary>
    public X509Certificate2? Certificate { get; set; }

    /// <summary>
    /// TrustLevel of the key. Defaults to ConfiguredKey because if you create
    /// a SigninKey yourself, the source is most likely configuration.
    /// </summary>
    public TrustLevel TrustLevel { get; init; } = TrustLevel.ConfiguredKey;
}
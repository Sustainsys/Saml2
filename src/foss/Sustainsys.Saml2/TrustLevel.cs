// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2;

/// <summary>
/// What is the trust level of a piece of data? The levels reflect
/// how trustworthy the data is based on if it is signed and how
/// the signature can be validated.
/// </summary>
public enum TrustLevel
{
    /// <summary>
    /// There is no integrity protection for the data.
    /// </summary>
    None = 0,

    /// <summary>
    /// The data was retreived over an outbound network connection,
    /// but the transport was not protected. This level is also set
    /// on all data that is verified as signed by a key that was retrieved
    /// over plain http.
    /// </summary>
    Http = 10,

    /// <summary>
    /// The data was directly retreived from the source using a valid
    /// TLS (https) connection. This level is also set on all data that
    /// is verified as signed by a key that was retrieved over TLS/https.
    /// In most setups, this level is regarded as secure.
    /// </summary>
    TLS = 20,

    /// <summary>
    /// The data was verified by a signature where signing key or a strong
    /// identifier of the key (such as a SHA256 cert thumbprint) was read 
    /// from configuration.
    /// </summary>
    ConfiguredKey = 30
}
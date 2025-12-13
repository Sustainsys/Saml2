// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Base class for role descriptors, implements RoleDescriptorType
/// </summary>
public class RoleDescriptor
{
    /// <summary>
    /// ProtocolSupportEnumeration attribute value. Defaults to <see cref="Constants.Saml2Protocol"/>
    /// </summary>
    public string ProtocolSupportEnumeration { get; set; }
        = Constants.Saml2Protocol;

    /// <summary>
    /// Cryptographif keys for signing and encryption.
    /// </summary>
    public List<KeyDescriptor> Keys { get; set; } = [];
}
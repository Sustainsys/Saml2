// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.AspNetCore;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// Options for bindings
/// </summary>
public class BindingOptions
{
    /// <summary>
    /// Max allowed size in bytes for message.
    /// </summary>
    public int MaxMessageSize { get; set; } = Saml2Defaults.MaxMessageSize;

    /// <summary>
    /// Max allowed size in bytes for RelayState.
    /// </summary>
    public int MaxRelayStateSize { get; set; } = Saml2Defaults.MaxRelayStateSize;

}
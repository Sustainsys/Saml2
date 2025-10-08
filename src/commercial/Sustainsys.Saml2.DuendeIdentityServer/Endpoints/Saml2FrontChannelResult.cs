// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Endpoints.Results;
using Sustainsys.Saml2.Bindings;

namespace Sustainsys.Saml2.DuendeIdentityServer.Endpoints;

/// <summary>
/// Result from a Saml2 endpoint that wraps a Saml2 message and should be handled by
/// a front channel binding.
/// </summary>
public class Saml2FrontChannelResult : EndpointResult<Saml2FrontChannelResult>
{
    /// <summary>
    /// Contained Saml2 Message
    /// </summary>
    public Saml2Message? Message { get; set; }

    /// <summary>
    /// Error message if this result is an error.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Entity Id of Sp as received in incoming message, may not be validated.
    /// </summary>
    public string? SpEntityID { get; set; }
}

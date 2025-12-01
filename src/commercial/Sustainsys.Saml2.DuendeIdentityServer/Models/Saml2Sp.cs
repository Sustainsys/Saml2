// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Models;

/// <summary>
/// Configuration for a Saml2 Service Provider
/// </summary>
public class Saml2Sp
{
    private Client inner;

    /// <summary>
    /// Default Ctor creating a new Client to wrap.
    /// </summary>
    public Saml2Sp()
    {
        inner = new()
        {
            ProtocolType = Saml2Constants.Saml2Protocol
        };

        AsssertionConsumerServices = new SerializingCollectionWrapper(inner);
    }

    internal Saml2Sp(Client inner)
    {
        if (inner.ProtocolType != Saml2Constants.Saml2Protocol)
        {
            throw new InvalidOperationException("Client must have Saml2 protocol type to be a valid Saml2 SP");
        }

        this.inner = inner;
        AsssertionConsumerServices = new SerializingCollectionWrapper(inner);
    }

    /// <summary>
    /// Convert Saml2 SP to a <see cref="Client"/>
    /// </summary>
    /// <param name="saml2Sp">Saml2 SP to convert</param>
    /// <remarks>
    /// The Saml2 SP is implemented as a wrapper around a Client object. This method simply
    /// returns the wrapped object.
    /// </remarks>
    public static implicit operator Client(Saml2Sp saml2Sp) => saml2Sp.inner;

    /// <summary>
    /// Entity Id identifying the SP (maps to ClientId)
    /// </summary>
    public string EntityId
    {
        get => inner.ClientId;
        set => inner.ClientId = value;
    }

    /// <summary>
    /// Assertion Consumer Services
    /// </summary>
    public ICollection<IndexedEndpoint> AsssertionConsumerServices { get; }
}

/// <summary>
/// Extensions to help converting.
/// </summary>
public static class Saml2SpExtensions
{
    /// <summary>
    /// Treat a Client object as a Saml2 SP
    /// </summary>
    /// <param name="client">Client</param>
    /// <returns>Saml2 SP</returns>
    public static Saml2Sp AsSaml2Sp(this Client client) => new Saml2Sp(client);
}


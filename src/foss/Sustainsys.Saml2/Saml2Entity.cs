// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2;
/// <summary>
/// A Saml2 entity, i.e. an Identity Provider or a Service Provider
/// </summary>
public class Saml2Entity
{
    /// <summary>
    /// The entity id of the identity provider
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Use Metadata for configuration. Defaults to true.
    /// </summary>
    public bool LoadMetadata { get; set; } = true;

    /// <summary>
    /// Location of metadata. If null, the EntityId is used.
    /// </summary>
    public string? MetadataLocation { get; set; }

    /// <summary>
    /// Allowed algorithms if validating signatures.
    /// </summary>
    public IEnumerable<string> AllowedAlgorithms { get; set; }
        = Constants.DefaultAllowedAlgorithms;

    /// <summary>
    /// Signing keys of the entity.
    /// </summary>
    public IEnumerable<SigningKey>? SigningKeys { get; set; }
}
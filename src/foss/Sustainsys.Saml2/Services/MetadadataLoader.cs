// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Services;

/// <summary>
/// Loader service for Metadata
/// </summary>
public interface IMetadadataLoader
{
    /// <summary>
    /// Gets the metadata from a specific location, validated
    /// by a specific signing key. Item might be cached respecting
    /// the cacheDuration and validUntil values.
    /// </summary>
    /// <param name="location">Location: URL or path</param>
    /// <param name="trustedSigningKeys">Signing key used to validate metadata</param>
    /// <param name="allowedAlgorithms">Allowed algorithms for signing/hashing</param>
    /// <returns>Metadata</returns>
    Task<MetadataBase?> GetMetadataAsync(
        string location,
        IEnumerable<SigningKey>? trustedSigningKeys,
        IEnumerable<string>? allowedAlgorithms);
}

/// <summary>
/// Loader service for Metadata
/// </summary>
public class MetadataLoader(ISamlXmlReader samlXmlReader) : IMetadadataLoader
{
    private readonly HttpClient metadataClient = new();

    /// <inheritdoc/>
    public async Task<MetadataBase?> GetMetadataAsync(
        string location,
        IEnumerable<SigningKey>? trustedSigningKeys,
        IEnumerable<string>? allowedAlgorithms)
    {
        XmlDocument xmlDoc = new();
        TrustLevel trustLevel;
        if (Uri.TryCreate(location, UriKind.Absolute, out var uri))
        {
            trustLevel = uri.Scheme switch
            {
                "https" => TrustLevel.TLS,
                "http" => TrustLevel.Http,
                _ => throw new InvalidOperationException("Unrecoqnized scheme " + uri.Scheme)
            };

            // TODO: Add caching

            var result = await metadataClient.GetAsync(uri);

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            // TODO: Add validation of Content type, with compatibility/option to relax.

            xmlDoc.Load(await result.Content.ReadAsStreamAsync());
        }
        else
        {
            // TODO: Implement.
            // TODO: Use File change monitor for caching.
            //trustLevel = TrustLevel.ConfiguredKey;
            throw new NotImplementedException("File metadata not implemented");
        }

        if (xmlDoc.DocumentElement == null)
        {
            return null;
        }

        XmlTraverser xmlTraverser = new(xmlDoc.DocumentElement, trustLevel);

        if (xmlTraverser.HasName(Constants.Elements.EntityDescriptor, Constants.Namespaces.MetadataUri))
        {
            samlXmlReader.TrustedSigningKeys = trustedSigningKeys;
            samlXmlReader.AllowedAlgorithms = allowedAlgorithms;

            // TODO: Plus package: add ErrorInspector from config.
            return samlXmlReader.ReadEntityDescriptor(xmlTraverser);
        }

        // TODO: Add support for EntitiesDescriptor too.

        return null;
    }
}
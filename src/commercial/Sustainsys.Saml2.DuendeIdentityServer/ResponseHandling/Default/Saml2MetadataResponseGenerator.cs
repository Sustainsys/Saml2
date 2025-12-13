// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Sustainsys.Saml2.DuendeIdentityServer.Endpoints.Results;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.ResponseHandling.Default;

/// <inheritdoc />
public class Saml2MetadataResponseGenerator(
    ISamlXmlWriterPlus samlXmlWriter)
    : ISaml2MetadataResponseGenerator
{
    /// <inheritdoc/>
    public async Task<Saml2MetadataResult> GenerateMetadataAsync(
        string issuer, Saml2Options options, string baseUrl)
    {
        var entity = await GenerateEntityDescriptorAsync(
            issuer, options, baseUrl);

        var xml = samlXmlWriter.Write(entity);

        return new()
        {
            Xml = xml
        };
    }

    /// <summary>
    /// Generate the EntityDescriptor
    /// </summary>
    /// <returns></returns>
    protected virtual Task<EntityDescriptor> GenerateEntityDescriptorAsync(
        string issuer, Saml2Options options, string baseUrl)
    {
        IDPSSODescriptor iDPSSODescriptor = new();

        EntityDescriptor entity = new()
        {
            EntityId = issuer,
            CacheDuration = options.Metadata.CacheDuration,
            ValidUntil = DateTime.UtcNow + options.Metadata.ExpiryDuration,
            Id = XmlHelpers.CreateId(),
            RoleDescriptors =
            {
                iDPSSODescriptor
            }
        };

        iDPSSODescriptor.SingleSignOnServices.AddRange(
            options.Endpoints.SingleSignOnServiceBindings.Select(b =>
            new Endpoint
            {
                Binding = b,
                Location = options.Endpoints.SingleSignOnServicePath
            }));

        return Task.FromResult(entity);
    }
}

// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Sustainsys.Saml2.Services;

/// <summary>
/// Context for <see cref="IdentityProviderConfigurationResolver.GetEffectiveConfigurationAsync"/>
/// </summary>
public class IdentityProviderConfigurationResolverContext
{
    /// <summary>
    /// Static/source configuration
    /// </summary>
    public required IdentityProvider StaticConfiguration { get; set; }

    /// <summary>
    /// Effective/resulting configuration
    /// </summary>
    public IdentityProvider? EffectiveConfiguration { get; set; }

    /// <summary>
    /// HttpContext
    /// </summary>
    public required HttpContext HttpContext { get; set; }
}

/// <summary>
/// Configuration resolver getting effective configuration
/// from static configuration.
/// </summary>
public interface IIdentityProviderConfigurationResolver
{
    /// <summary>
    /// Gets the effective configuration for an IdentityProvider from the static
    /// configuration.
    /// </summary>
    /// <param name="context">Context</param>
    /// <returns>Task</returns>
    Task GetEffectiveConfigurationAsync(
        IdentityProviderConfigurationResolverContext context);
}

/// <summary>
/// Configuration resolver getting effective configuration
/// from static configuration.
/// </summary>
/// <param name="metadadataLoader">Metadata Loader</param>
public class IdentityProviderConfigurationResolver
    (IMetadadataLoader metadadataLoader)
    : IIdentityProviderConfigurationResolver
{
    /// <inheritdoc/>
    public virtual async Task GetEffectiveConfigurationAsync(
        IdentityProviderConfigurationResolverContext context)
    {
        MetadataBase? metadata = null;

        if (context.StaticConfiguration.LoadMetadata)
        {
            metadata = await metadadataLoader.GetMetadataAsync(
                context.StaticConfiguration.MetadataLocation
                ?? context.StaticConfiguration.EntityId
                ?? throw new InvalidOperationException("EntityId or MetadataLocation must be set."),
                null);
        }

        await ResolveEffectiveConfigurationAsync(context, metadata);
    }

    /// <summary>
    /// Create the effective configuration.
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="metadata">Metadata if available</param>
    /// <returns>Task</returns>
    protected virtual async Task ResolveEffectiveConfigurationAsync(
        IdentityProviderConfigurationResolverContext context, MetadataBase? metadata)
    {
        var entityDescriptor = metadata as EntityDescriptor;

        if (entityDescriptor == null)
        {
            context.EffectiveConfiguration = context.StaticConfiguration;
        }
        else
        {
            if(context.StaticConfiguration.EntityId != null 
                && context.StaticConfiguration.EntityId !=
                entityDescriptor.EntityId)
            {
                throw new InvalidOperationException(
                    $"Unexpected EntityId {entityDescriptor.EntityId} in loaded metadata. " +
                    $"Expected {context.StaticConfiguration.EntityId}. To allow any EntityId" +
                    $"value from loaded Metadata, set the static configuration EntityId to null.");
            }

            var IdpSsoDescriptor = entityDescriptor.RoleDescriptors
                .OfType<IDPSSODescriptor>().Single();

            var ssoService = IdpSsoDescriptor.SingleSignOnServices
                .First(sso => sso.Binding == Constants.BindingUris.HttpRedirect)
                ?? IdpSsoDescriptor.SingleSignOnServices.First();

            context.EffectiveConfiguration = new()
            {
                EntityId = entityDescriptor.EntityId,
                AllowedAlgorithms = context.StaticConfiguration.AllowedAlgorithms,
                
                // TODO: Union with configured keys and use best trust level for keys in both.
                SigningKeys = IdpSsoDescriptor.Keys.Select(k =>
                    new Xml.SigningKey()
                    {
                        Certificate = k.KeyInfo!.OfType<KeyInfoX509Data>()
                            .Single().Certificates!.OfType<X509Certificate2>().Single(),
                        TrustLevel = entityDescriptor.TrustLevel
                    }),
                SsoServiceUrl = context.StaticConfiguration.SsoServiceUrl ?? ssoService.Location,
                SsoServiceBinding = context.StaticConfiguration.SsoServiceBinding ?? ssoService.Binding,
            };
        }
    }
}

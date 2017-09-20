using Kentor.AuthServices.Metadata;
using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp.Models
{
    public static class MetadataModel
    {
        public static ExtendedEntityDescriptor CreateIdpMetadata(bool includeCacheDuration = true)
        {
            var metadata = new ExtendedEntityDescriptor()
            {
                EntityId = new EntityId(UrlResolver.MetadataUrl.ToString())
            };

            if (includeCacheDuration)
            {
                metadata.CacheDuration = new TimeSpan(0, 15, 0);
                metadata.ValidUntil = DateTime.UtcNow.AddDays(1);
            }

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.SsoServiceUrl
            });
            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpPostUri,
                Location = UrlResolver.SsoServiceUrl
            });

            idpSsoDescriptor.ArtifactResolutionServices.Add(0, new IndexedProtocolEndpoint()
            {
                Index = 0,
                IsDefault = true,
                Binding = Saml2Binding.SoapUri,
                Location = UrlResolver.ArtifactServiceUrl
            });

            idpSsoDescriptor.SingleLogoutServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.LogoutServiceUrl
            });

            idpSsoDescriptor.SingleLogoutServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpPostUri,
                Location = UrlResolver.LogoutServiceUrl
            });

            idpSsoDescriptor.Keys.Add(CertificateHelper.SigningKey);

            return metadata;
        }

        public static ExtendedEntitiesDescriptor CreateFederationMetadata()
        {
            var metadata = new ExtendedEntitiesDescriptor
            {
                Name = "Kentor.AuthServices.StubIdp Federation",
                CacheDuration = new TimeSpan(0, 15, 0),
                ValidUntil = DateTime.UtcNow.AddDays(1)
            };

            metadata.ChildEntities.Add(CreateIdpMetadata(false));

            return metadata;
        }
    }
}
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Metadata.Descriptors;
using Sustainsys.Saml2.Metadata.Services;
using Sustainsys.Saml2.WebSso;
using System;
using EntitiesDescriptor = Sustainsys.Saml2.Metadata.Descriptors.EntitiesDescriptor;
using EntityDescriptor = Sustainsys.Saml2.Metadata.Descriptors.EntityDescriptor;

namespace Sustainsys.Saml2.StubIdp.Models
{
    public static class MetadataModel
    {
        public static EntityDescriptor CreateIdpMetadata(bool includeCacheDuration = true)
        {
            var metadata = new EntityDescriptor()
            {
                EntityId = new Metadata.EntityId(UrlResolver.MetadataUrl.ToString())
            };

            if (includeCacheDuration)
            {
                metadata.CacheDuration = new XsdDuration(minutes: 15);
                metadata.ValidUntil = DateTime.UtcNow.AddDays(1);
            }

            var idpSsoDescriptor = new IdpSsoDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new SingleSignOnService()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.SsoServiceUrl
            });
            idpSsoDescriptor.SingleSignOnServices.Add(new SingleSignOnService()
            {
                Binding = Saml2Binding.HttpPostUri,
                Location = UrlResolver.SsoServiceUrl
            });

            idpSsoDescriptor.ArtifactResolutionServices.Add(0, new ArtifactResolutionService()
            {
                Index = 0,
                IsDefault = true,
                Binding = Saml2Binding.SoapUri,
                Location = UrlResolver.ArtifactServiceUrl
            });

            idpSsoDescriptor.SingleLogoutServices.Add(new SingleLogoutService()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.LogoutServiceUrl
            });

            idpSsoDescriptor.SingleLogoutServices.Add(new SingleLogoutService()
            {
                Binding = Saml2Binding.HttpPostUri,
                Location = UrlResolver.LogoutServiceUrl
            });

            idpSsoDescriptor.Keys.Add(CertificateHelper.SigningKey);

            return metadata;
        }

        public static EntitiesDescriptor CreateFederationMetadata()
        {
            var metadata = new EntitiesDescriptor
            {
                Name = "Sustainsys.Saml2.StubIdp Federation",
                CacheDuration = new XsdDuration(minutes: 15),
                ValidUntil = DateTime.UtcNow.AddDays(1)
            };

            metadata.ChildEntities.Add(CreateIdpMetadata(false));

            return metadata;
        }
    }
}
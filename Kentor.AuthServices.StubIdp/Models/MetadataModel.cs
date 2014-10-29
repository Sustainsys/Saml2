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
            }

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            metadata.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = UrlResolver.SsoServiceUrl
            });

            idpSsoDescriptor.Keys.Add(CertificateHelper.SigningKey);

            return metadata;
        }

        public static ExtendedEntitiesDescriptor CreateFederationMetadata()
        {
            var metadata = new ExtendedEntitiesDescriptor
            {
                Name = "Kentor.AuthServices.StubIdp Federation",
                CacheDuration = new TimeSpan(0, 15, 0)
            };

            metadata.ChildEntities.Add(CreateIdpMetadata(false));

            return metadata;
        }
    }
}
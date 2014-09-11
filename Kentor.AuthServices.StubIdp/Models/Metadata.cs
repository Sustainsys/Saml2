using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp.Models
{
    public static class Metadata
    {
        public static EntityDescriptor IdpMetadata
        {
            get
            {
                var metadata = new EntityDescriptor()
                {
                    EntityId = new EntityId(UrlResolver.MetadataUrl.ToString())
                };

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
        }

        public static EntitiesDescriptor FederationMetadata
        {
            get
            {
                var metadata = new EntitiesDescriptor();
                metadata.Name = "Kentor.AuthServices.StubIdp Federation";

                metadata.ChildEntities.Add(IdpMetadata);

                return metadata;
            }
        }
    }
}
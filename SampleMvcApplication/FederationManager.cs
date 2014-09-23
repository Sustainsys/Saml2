using System;
using System.IdentityModel.Metadata;
using Kentor.AuthServices;

namespace SampleMvcApplication
{
    public class FederationManager : IFederationManager
    {

        public bool AllowUnsolicitedAuthnResponse
        {
            get { return true; }
        }

        public EntitiesDescriptor Load()
        {
            var idp = new EntityDescriptor()
            {
                EntityId = new EntityId("http://stubidp.kentor.se/Metadata")
            };

            var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
            idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
            idp.RoleDescriptors.Add(idpSsoDescriptor);

            idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
            {
                Binding = Saml2Binding.HttpRedirectUri,
                Location = new Uri("http://stubidp.kentor.se")
            });

            idpSsoDescriptor.Keys.Add(CertificateHelper.SigningKey);
            
            var entities = new EntitiesDescriptor();
            entities.ChildEntities.Add(idp);
            
            return entities;
        }
    }
}
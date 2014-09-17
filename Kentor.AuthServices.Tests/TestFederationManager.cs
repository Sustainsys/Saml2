using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Tests
{
    public class TestFederationManager : IFederationManager
    {
        public static int NumberOfEntries { get; set; }
        public bool AllowUnsolicitedAuthnResponse
        {
            get { return true; }
        }

        static TestFederationManager()
        {
            NumberOfEntries = 1;
        }

        public EntitiesDescriptor Load()
        {
            var entities = new EntitiesDescriptor();
            for (int i = 1; i <= NumberOfEntries; i++)
            {
                var idp = new EntityDescriptor()
                {
                    EntityId = new EntityId("http://idp.test.com/metadata" + i)
                };

                var idpSsoDescriptor = new IdentityProviderSingleSignOnDescriptor();
                idpSsoDescriptor.ProtocolsSupported.Add(new Uri("urn:oasis:names:tc:SAML:2.0:protocol"));
                idp.RoleDescriptors.Add(idpSsoDescriptor);

                idpSsoDescriptor.SingleSignOnServices.Add(new ProtocolEndpoint()
                {
                    Binding = Saml2Binding.HttpRedirectUri,
                    Location = new Uri("http://idp.test.com")
                });

                idpSsoDescriptor.Keys.Add(new KeyDescriptor(new SecurityKeyIdentifier(
                    (new X509SecurityToken(SignedXmlHelper.TestCert))
                        .CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>())));

                entities.ChildEntities.Add(idp);
            }
            return entities;
        }
    }
}
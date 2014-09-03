using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;
using System.Net;
using System.IdentityModel.Metadata;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace Kentor.AuthServices
{
    class IdentityProvider
    {
        private static readonly IDictionary<EntityId, IdentityProvider> configuredIdentityProviders =
            KentorAuthServicesSection.Current.IdentityProviders
            .ToDictionary(idp => new EntityId(idp.EntityId), 
                          idp => new IdentityProvider(idp),
                          EntityIdEqualityComparer.Instance);

        public static IDictionary<EntityId, IdentityProvider> ConfiguredIdentityProviders
        {
            get
            {
                return configuredIdentityProviders;
            }
        }

        public IdentityProvider() { }

        // Ctor used for testing.
        internal IdentityProvider(Uri destinationUri)
        {
            DestinationUri = destinationUri;
        }

        internal IdentityProvider(IdentityProviderElement config)
        {
            DestinationUri = config.DestinationUri;
            EntityId = new EntityId(config.EntityId);
            Binding = config.Binding;

            var certificate = config.SigningCertificate.LoadCertificate();

            if (certificate != null)
            {
                SigningKey = certificate.PublicKey.Key;
            }

            if (config.LoadMetadata)
            {
                LoadMetadata();
            }
        }

        public Saml2BindingType Binding { get; private set; }

        public Uri DestinationUri { get; private set; }

        public EntityId EntityId { get; private set; }

        public Saml2AuthenticationRequest CreateAuthenticateRequest(Uri returnUri)
        {
            var request = new Saml2AuthenticationRequest()
            {
                DestinationUri = DestinationUri,
                AssertionConsumerServiceUrl = KentorAuthServicesSection.Current.AssertionConsumerServiceUrl,
                Issuer = KentorAuthServicesSection.Current.EntityId
            };

            var responseData = new StoredRequestState(EntityId, returnUri);

            PendingAuthnRequests.Add(new Saml2Id(request.Id), responseData);

            return request;
        }

        public CommandResult Bind(Saml2AuthenticationRequest request)
        {
            return Saml2Binding.Get(Binding).Bind(request);
        }

        public AsymmetricAlgorithm SigningKey { get; private set; }

        private void LoadMetadata()
        {
            // So far only support for metadata at well known location.
            var metadata = MetadataLoader.Load(new Uri(EntityId.Id));

            var idpDescriptor = metadata.RoleDescriptors
                .OfType<IdentityProviderSingleSignOnDescriptor>().Single();

            var ssoService = idpDescriptor.SingleSignOnServices.First();

            Binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
            DestinationUri = ssoService.Location;

            SigningKey = ((AsymmetricSecurityKey)idpDescriptor.Keys.Single().KeyInfo.CreateKey())
                .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
        }
    }
}

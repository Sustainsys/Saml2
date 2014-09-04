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
using System.Configuration;
using System.Globalization;

namespace Kentor.AuthServices
{
    class IdentityProvider
    {
        private static readonly IDictionary<EntityId, IdentityProvider> configuredIdentityProviders =
            KentorAuthServicesSection.Current.IdentityProviders
            .ToDictionary(idp => new EntityId(idp.EntityId), 
                          idp => new IdentityProvider(idp),
                          EntityIdEqualityComparer.Instance);

        public static IDictionary<EntityId, IdentityProvider> ActiveIdentityProviders
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
            AssertionConsumerServiceUrl = destinationUri;
        }

        internal IdentityProvider(IdentityProviderElement config)
        {
            AssertionConsumerServiceUrl = config.DestinationUri;
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

            Validate();
        }

        private void Validate()
        {
            if(Binding == 0)
            {
                throw new ConfigurationErrorsException("Missing binding configuration on Idp " + EntityId.Id + ".");
            }

            if(SigningKey == null)
            {
                throw new ConfigurationErrorsException("Missing signing certificate configuration on Idp " + EntityId.Id + ".");
            }

            if (AssertionConsumerServiceUrl == null)
            {
                throw new ConfigurationErrorsException("Missing assertion consumer service url configuration on Idp " + EntityId.Id + ".");
            }
        }

        public Saml2BindingType Binding { get; private set; }

        public Uri AssertionConsumerServiceUrl { get; private set; }

        public EntityId EntityId { get; private set; }

        public Saml2AuthenticationRequest CreateAuthenticateRequest(Uri returnUri)
        {
            var request = new Saml2AuthenticationRequest()
            {
                DestinationUri = AssertionConsumerServiceUrl,
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

            if(metadata.EntityId.Id != EntityId.Id)
            {
                var msg = string.Format(CultureInfo.InvariantCulture, 
                    "Unexpected entity id \"{0}\" found when loading metadata for \"{1}\".",
                    metadata.EntityId.Id, EntityId.Id);
                throw new ConfigurationErrorsException(msg);
            }

            var idpDescriptor = metadata.RoleDescriptors
                .OfType<IdentityProviderSingleSignOnDescriptor>().Single();

            var ssoService = idpDescriptor.SingleSignOnServices.First();

            Binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
            AssertionConsumerServiceUrl = ssoService.Location;

            var key = idpDescriptor.Keys.SingleOrDefault();

            if(key != null)
            {
                SigningKey = ((AsymmetricSecurityKey)key.KeyInfo.CreateKey())
                    .GetAsymmetricAlgorithm(SignedXml.XmlDsigRSASHA1Url, false);
            }
        }
    }
}

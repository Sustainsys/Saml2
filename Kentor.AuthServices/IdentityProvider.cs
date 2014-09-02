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
            certificate = config.SigningCertificate.LoadCertificate();

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

        readonly X509Certificate2 certificate;
        public X509Certificate2 Certificate
        {
            get
            {
                return certificate;
            }
        }

        private void LoadMetadata()
        {
            // So far only support for metadata at well known location.
            var metadata = MetadataLoader.Load(new Uri(EntityId.Id));

            var ssoService = metadata.RoleDescriptors
                .OfType<IdentityProviderSingleSignOnDescriptor>().First()
                .SingleSignOnServices.First();

            Binding = Saml2Binding.UriToSaml2BindingType(ssoService.Binding);
            DestinationUri = ssoService.Location;
        }
    }
}

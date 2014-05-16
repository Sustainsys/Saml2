using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices
{
    class IdentityProvider
    {
        private static readonly IDictionary<string, IdentityProvider> configuredIdentityProviders =
            KentorAuthServicesSection.Current.IdentityProviders
            .ToDictionary(idp => idp.EntityId, idp => new IdentityProvider(idp));

        public static IDictionary<string, IdentityProvider> ConfiguredIdentityProviders
        {
            get
            {
                return configuredIdentityProviders;
            }
        }

        public IdentityProvider() { }

        public IdentityProvider(IdentityProviderElement config)
        {
            DestinationUri = config.DestinationUri;
            Issuer = config.EntityId;
            Binding = config.Binding;
            certificate = config.SigningCertificate.LoadCertificate();
        }

        public Saml2BindingType Binding { get; set; }

        public Uri DestinationUri { get; set; }

        public string Issuer { get; set; }

        public Saml2AuthenticationRequest CreateAuthenticateRequest(Uri returnUri)
        {
            var request = new Saml2AuthenticationRequest()
            {
                DestinationUri = DestinationUri,
                AssertionConsumerServiceUrl = KentorAuthServicesSection.Current.AssertionConsumerServiceUrl,
                Issuer = KentorAuthServicesSection.Current.EntityId
            };

            var responseData = new StoredRequestState(Issuer, returnUri);

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
    }
}

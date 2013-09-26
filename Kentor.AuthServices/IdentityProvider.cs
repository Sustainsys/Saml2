using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices
{
    class IdentityProvider
    {
        private static readonly IDictionary<string, IdentityProvider> configuredIdentityProviders =
            Saml2AuthenticationModuleSection.Current.IdentityProviders
            .ToDictionary(idp => idp.Issuer, idp => new IdentityProvider(idp));

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
            Binding = config.Binding;

            if (!string.IsNullOrEmpty(config.SigningCertificateFile))
            {
                certificate = new X509Certificate2(config.SigningCertificateFile);
            }
        }

        public Saml2BindingType Binding { get; set; }

        public Uri DestinationUri { get; set; }

        public Saml2AuthenticationRequest CreateAuthenticateRequest()
        {
            return new Saml2AuthenticationRequest()
            {
                DestinationUri = DestinationUri,
                AssertionConsumerServiceUrl = Saml2AuthenticationModuleSection.Current.AssertionConsumerServiceUrl,
                Issuer = Saml2AuthenticationModuleSection.Current.Issuer
            };
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

using Kentor.AuthServices.Configuration;
using System;
using System.IdentityModel.Services;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace Kentor.AuthServices
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestBase request)
        {
            var binding = Saml2Binding.Get(request);

            if (binding != null)
            {
                try
                {
                    var samlResponse = binding.Unbind<Saml2Response>(request);

                    samlResponse.Validate(GetSigningCert(samlResponse.Issuer));

                    var principal = new ClaimsPrincipal(samlResponse.GetClaims());
                    FederatedAuthentication.FederationConfiguration.IdentityConfiguration
                        .ClaimsAuthenticationManager.Authenticate(null, principal);

                    return new CommandResult()
                    {
                        HttpStatusCode = HttpStatusCode.SeeOther,
                        Location = samlResponse.ReturnUri ?? KentorAuthServicesSection.Current.ReturnUri,
                        Principal = principal
                    };
                }
                catch (FormatException ex)
                {
                    throw new BadFormatSamlResponseException(
                            "The SAML Response did not contain valid BASE64 encoded data.", ex);
                }
                catch (XmlException ex)
                {
                    throw new BadFormatSamlResponseException(
                        "The SAML response contains incorrect XML", ex);
                }
            }

            throw new NoSamlResponseFoundException();
        }

        private static X509Certificate2 GetSigningCert(string issuer)
        {
            return IdentityProvider.ConfiguredIdentityProviders[issuer].Certificate;
        }
    }
}

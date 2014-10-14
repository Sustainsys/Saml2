using Kentor.AuthServices.Configuration;
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace Kentor.AuthServices
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }

            if(options == null)
            {
                throw new ArgumentNullException("options");
            }

            var binding = Saml2Binding.Get(request);

            if (binding != null)
            {
                try
                {
                    var samlResponse = Saml2Response.Read(binding.Unbind(request));

                    samlResponse.Validate(GetSigningKey(samlResponse.Issuer));

                    var principal = new ClaimsPrincipal(samlResponse.GetClaims());
                    
                    principal = FederatedAuthentication.FederationConfiguration.IdentityConfiguration
                        .ClaimsAuthenticationManager.Authenticate(null, principal);

                    return new CommandResult()
                    {
                        HttpStatusCode = HttpStatusCode.SeeOther,
                        Location = 
                            samlResponse.RequestState != null && samlResponse.RequestState.ReturnUri != null 
                            ? samlResponse.RequestState.ReturnUri 
                            : options.SPOptions.ReturnUri,
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

        private static AsymmetricAlgorithm GetSigningKey(EntityId issuer)
        {
            return IdentityProvider.ActiveIdentityProviders[issuer].SigningKey;
        }
    }
}

using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                    var samlResponse = binding.Unbind(request);

                    samlResponse.Validate(GetSigningCert(samlResponse.Issuer));
                    
                    var principal = new ClaimsPrincipal(samlResponse.GetClaims());
                    FederatedAuthentication.FederationConfiguration.IdentityConfiguration
                        .ClaimsAuthenticationManager.Authenticate(null, principal);

                    return new CommandResult()
                    {
                        HttpStatusCode = HttpStatusCode.SeeOther,
                        Location = Saml2AuthenticationModuleSection.Current.ReturnUri,
                        Principal = principal
                    };
                }
                catch (Exception ex)
                {
                    if (ex is XmlException || ex is FormatException)
                    {
                        return new CommandResult()
                        {
                            ErrorCode = CommandResultErrorCode.BadFormatSamlResponse,
                            HttpStatusCode = HttpStatusCode.InternalServerError
                        };
                    }
                    throw;
                }
            }

            return noSamlResponseFoundResult;
        }

        private static X509Certificate2 GetSigningCert(string issuer)
        {
            return IdentityProvider.ConfiguredIdentityProviders[issuer].Certificate;
        }

        static readonly CommandResult noSamlResponseFoundResult = new CommandResult()
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            ErrorCode = CommandResultErrorCode.NoSamlResponseFound
        };
    }
}

using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Saml2P;
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;
using System.Linq;
​
namespace Kentor.AuthServices.WebSso
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }
​
            if(options == null)
            {
                throw new ArgumentNullException("options");
            }
​
            var binding = Saml2Binding.Get(request);
​
            if (binding != null)
            {
                string unpackedPayload = null;
                try
                {
                    unpackedPayload = binding.Unbind(request);
                    var samlResponse = Saml2Response.Read(unpackedPayload);

                    var relayStates = request.Form.First(x => string.Compare(x.Key, "RelayState", StringComparison.OrdinalIgnoreCase) == 0);
                    string returnURL = string.Empty;
                    returnURL = relayStates.Value;
/*                    foreach(var state in relayStates)
                    {
                        returnURL = state;
                        break;
                    }
                    */
​
                    return ProcessResponse(options, samlResponse, returnURL);
                }
                catch (FormatException ex)
                {
                    throw new BadFormatSamlResponseException(
                            "The SAML Response did not contain valid BASE64 encoded data.", ex);
                }
                catch (XmlException ex)
                {
                    var newEx = new BadFormatSamlResponseException(
                        "The SAML response contains incorrect XML", ex);

                    // Add the payload to the exception
                    newEx.Data["Saml2Response"] = unpackedPayload;
                    throw newEx;
                }
                catch (Exception ex)
                {
                    // Add the payload to the existing exception
                    ex.Data["Saml2Response"] = unpackedPayload;
                    throw;
                }
            }
​
            throw new NoSamlResponseFoundException();
        }
​
        private static CommandResult ProcessResponse(IOptions options, Saml2Response samlResponse, string returnURL)
        {
            var principal = new ClaimsPrincipal(samlResponse.GetClaims(options));
​
            principal = options.SPOptions.SystemIdentityModelIdentityConfiguration
                .ClaimsAuthenticationManager.Authenticate(null, principal);
​
            var requestState = samlResponse.GetRequestState(options);
            UriBuilder builder = new UriBuilder(requestState != null && requestState.ReturnUrl != null ? requestState.ReturnUrl : options.SPOptions.ReturnUrl);
            if (!string.IsNullOrEmpty(returnURL) && builder.Path.ToString().IndexOf(returnURL, StringComparison.OrdinalIgnoreCase) < 0)
            {
                builder = new UriBuilder(returnURL);
            }
​
            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = builder.Uri,
                Principal = principal,
                RelayData =
                    requestState == null
                    ? null
                    : requestState.RelayData
            };
        }
    }
}

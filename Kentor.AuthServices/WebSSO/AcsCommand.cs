using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Saml2P;
using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace Kentor.AuthServices.WebSso
{
    class AcsCommand : ICommand
    {
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var binding = Saml2Binding.Get(request);

            if (binding != null)
            {
                string unpackedPayload = null;
                try
                {
                    unpackedPayload = binding.Unbind(request);
                    var samlResponse = Saml2Response.Read(unpackedPayload);

                    return ProcessResponse(options, samlResponse);
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

            throw new NoSamlResponseFoundException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "returnUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SpOptions")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ReturnUrl")]
        private static CommandResult ProcessResponse(IOptions options, Saml2Response samlResponse)
        {
            var principal = new ClaimsPrincipal(samlResponse.GetClaims(options));

            principal = options.SPOptions.SystemIdentityModelIdentityConfiguration
                .ClaimsAuthenticationManager.Authenticate(null, principal);

            var requestState = samlResponse.GetRequestState(options);

            if(requestState == null && options.SPOptions.ReturnUrl == null)
            {
                throw new ConfigurationErrorsException(MissingReturnUrlMessage);
            }

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = requestState?.ReturnUrl ?? options.SPOptions.ReturnUrl,
                Principal = principal,
                RelayData =
                    requestState == null
                    ? null
                    : requestState.RelayData
            };
        }

        internal const string MissingReturnUrlMessage =
@"Unsolicited SAML response received, but no ReturnUrl is configured.

When receiving unsolicited SAML responses (i.e. IDP initiated login),
AuthServices will redirect the client to the configured ReturnUrl after
successful authentication, but it is not configured.

In code-based config, add a ReturnUrl by setting the
options.SpOptions.ReturnUrl property. In the config file, set the returnUrl
attribute of the <kentor.authServices> element.";
    }
}

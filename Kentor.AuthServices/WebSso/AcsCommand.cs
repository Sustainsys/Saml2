using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Exceptions;
using Kentor.AuthServices.Saml2P;
using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
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

            var binding = options.Notifications.GetBinding(request);

            if (binding != null)
            {
                UnbindResult unbindResult = null;
                try
                {
                    unbindResult = binding.Unbind(request, options);
                    options.Notifications.MessageUnbound(unbindResult);

                    var samlResponse = new Saml2Response(unbindResult.Data, request.StoredRequestState?.MessageId);

                    var result = ProcessResponse(options, samlResponse, request.StoredRequestState);
                    if(unbindResult.RelayState != null)
                    {
                        result.ClearCookieName = "Kentor." + unbindResult.RelayState;
                    }
                    options.Notifications.AcsCommandResultCreated(result, samlResponse);
                    return result;
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
                    if (unbindResult != null)
                    {
                        newEx.Data["Saml2Response"] = unbindResult.Data.OuterXml;
                    }
                    throw newEx;
                }
                catch (Exception ex)
                {
                    if (unbindResult != null)
                    {
                        // Add the payload to the existing exception
                        ex.Data["Saml2Response"] = unbindResult.Data.OuterXml;
                    }
                    throw;
                }
            }

            throw new NoSamlResponseFoundException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AuthenticationProperty")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RedirectUri")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "returnUrl")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "SpOptions")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ReturnUrl")]
        private static CommandResult ProcessResponse(
            IOptions options,
            Saml2Response samlResponse,
            StoredRequestState storedRequestState)
        {
            var principal = new ClaimsPrincipal(samlResponse.GetClaims(options));

            principal = options.SPOptions.SystemIdentityModelIdentityConfiguration
                .ClaimsAuthenticationManager.Authenticate(null, principal);

            if(options.SPOptions.ReturnUrl == null)
            {
                if (storedRequestState == null)
                {
                    throw new ConfigurationErrorsException(UnsolicitedMissingReturnUrlMessage);
                }
                if(storedRequestState.ReturnUrl == null)
                {
                    throw new ConfigurationErrorsException(SpInitiatedMissingReturnUrl);
                }
            }

            return new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = storedRequestState?.ReturnUrl ?? options.SPOptions.ReturnUrl,
                Principal = principal,
                RelayData = storedRequestState?.RelayData,
                SessionNotOnOrAfter = samlResponse.SessionNotOnOrAfter
            };
        }

        internal const string UnsolicitedMissingReturnUrlMessage =
@"Unsolicited SAML response received, but no ReturnUrl is configured.

When receiving unsolicited SAML responses (i.e. IDP initiated login),
AuthServices will redirect the client to the configured ReturnUrl after
successful authentication, but it is not configured.

In code-based config, add a ReturnUrl by setting the
options.SpOptions.ReturnUrl property. In the config file, set the returnUrl
attribute of the <kentor.authServices> element.";

        internal const string SpInitiatedMissingReturnUrl =
@"Successfully received and validated response from Idp, but don't know
where to redirect now. There was no return url specified when initiating
the request and there is no default return url configured.

When initiating a request, pass a ReturnUrl query parameter (case matters) or 
use the RedirectUri AuthenticationProperty for owin. Or add a default ReturnUrl
in the configuration.";
    }
}

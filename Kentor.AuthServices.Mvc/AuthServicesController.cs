using System;
using System.Net;
using System.Web.Mvc;
using System.IdentityModel.Services;
using Kentor.AuthServices.HttpModule;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.WebSso;
using System.Diagnostics.CodeAnalysis;

namespace Kentor.AuthServices.Mvc
{
    /// <summary>
    /// Mvc Controller that provides the authentication functionality.
    /// </summary>
    [AllowAnonymous]
    public class AuthServicesController : Controller
    {
        private static IOptions options = null;

        /// <summary>
        /// The options used by the controller. By default read from config, 
        /// but can be set.
        /// </summary>
        public static IOptions Options {
            get
            {
                if(options == null)
                {
                    options = Configuration.Options.FromConfiguration;
                }
                return options;
            }
            set
            {
                options = value;
            }
        }

        /// <summary>
        /// SignIn action that sends the AuthnRequest to the Idp.
        /// </summary>
        /// <returns>Redirect with sign in request</returns>
        public ActionResult SignIn()
        {
            return CommandFactory.GetCommand(CommandFactory.SignInCommandName).Run(
                Request.ToHttpRequestData(),
                Options)
                .ToActionResult();
        }

        /// <summary>
        /// Assertion consumer Url that accepts the incoming Saml response.
        /// </summary>
        /// <returns>Redirect to start page on success.</returns>
        /// <remarks>The action effectively accepts the SAMLResponse, but
        /// due to using common infrastructure it is read for the current
        /// http request.</remarks>
        public ActionResult Acs()
        {
            var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName).Run(
                Request.ToHttpRequestData(),
                Options);

            result.SignInSessionAuthenticationModule();
            return result.ToActionResult();
        }

        /// <summary>
        /// SignOut action that signs out the current user.
        /// </summary>
        /// <returns>Redirect to base url / </returns>
        // Exclude from code coverage as it a) is very simple and b) can't be
        // tested without shims that are only available in VSPremium.
        [ExcludeFromCodeCoverage]
        [HttpGet]
        public ActionResult SignOut()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            var result = CommandFactory.GetCommand(CommandFactory.SingleSignOutCommandName).Run(
                Request.ToHttpRequestData(),
                Options)
                .ToActionResult();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="samlResponse"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "samlResponse")]
        [HttpPost]
        public ActionResult SignOut(string samlResponse)
        {
            //todo: verify response
            return Redirect("~/");
        }

        /// <summary>
        /// Metadata of the service provider.
        /// </summary>
        /// <returns>ActionResult with Metadata</returns>
        public ActionResult Index()
        {
            var result = CommandFactory.GetCommand(CommandFactory.MetadataCommand).Run(
                Request.ToHttpRequestData(),
                Options);
            return result.ToActionResult();
        }
    }
}

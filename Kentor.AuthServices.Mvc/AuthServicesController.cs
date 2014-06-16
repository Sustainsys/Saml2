using System;
using System.Net;
using System.Web.Mvc;
using System.IdentityModel.Services;

namespace Kentor.AuthServices.Mvc
{
    /// <summary>
    /// Mvc Controller that provides the authentication functionality.
    /// </summary>
    [AllowAnonymous]
    public class AuthServicesController : Controller
    {
        /// <summary>
        /// SignIn action that sends the AuthnRequest to the Idp.
        /// </summary>
        /// <returns>Redirect with sign in request</returns>
        public ActionResult SignIn()
        {
            return CommandFactory.GetCommand("SignIn").Run(new HttpRequestData(Request)).ToActionResult();
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
            var result = CommandFactory.GetCommand("Acs").Run(new HttpRequestData(Request));
            result.SignInSessionAuthenticationModule();
            return result.ToActionResult();
        }

        /// <summary>
        /// SignOut action that signs out the current user.
        /// </summary>
        /// <returns>Redirect to base url / </returns>
        public ActionResult SignOut()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return Redirect(Url.Content("~/"));
        }

        /// <summary>
        /// Metadata of the service provider.
        /// </summary>
        /// <returns>ActionResult with Metadata</returns>
        public ActionResult Index()
        {
            var result = CommandFactory.GetCommand("").Run(new HttpRequestData(Request));
            return result.ToActionResult();
        }
    }
}

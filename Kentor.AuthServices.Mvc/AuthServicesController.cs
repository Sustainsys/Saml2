using System;
using System.Net;
using System.Web.Mvc;

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
        /// <returns>Sign in request</returns>
        public ActionResult SignIn()
        {
            return CommandFactory.GetCommand("SignIn").Run(Request).ToActionResult();
        }
    }
}

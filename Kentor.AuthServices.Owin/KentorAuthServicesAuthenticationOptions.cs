using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin
{
    /// <summary>
    /// Options for Kentor AuthServices Saml2 Authentication.
    /// </summary>
    public class KentorAuthServicesAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public KentorAuthServicesAuthenticationOptions()
            : base(Constants.DefaultAuthenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;
        }
    }
}

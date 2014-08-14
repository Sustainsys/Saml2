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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "KentorAuthServices")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Owin.Security.AuthenticationDescription.set_Caption(System.String)")]
        public KentorAuthServicesAuthenticationOptions()
            : base(Constants.DefaultAuthenticationType)
        {
            AuthenticationMode = AuthenticationMode.Passive;
            Description.Caption = Constants.DefaultAuthenticationType;
        }
    }
}

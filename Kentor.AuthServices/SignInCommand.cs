using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    class SignInCommand : ICommand
    {
        public CommandResult Run(HttpRequestBase request)
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;
            
            var authnRequest = idp.CreateAuthenticateRequest();

            return idp.Bind(authnRequest);
        }
    }
}

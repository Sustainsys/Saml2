using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    class SignInCommand : ICommand
    {
        public CommandResult Run(NameValueCollection formData = null)
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;
            
            var request = idp.CreateAuthenticateRequest();

            return idp.Bind(request);
        }
    }
}

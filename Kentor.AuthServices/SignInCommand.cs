using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    class SignInCommand : Command
    {
        public override CommandResult Run()
        {
            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;
            
            var request = idp.CreateAuthenticateRequest();

            return idp.Bind(request);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    abstract class Saml2Binding
    {
        public abstract CommandResult Bind(Saml2AuthenticationRequest request);

        private static IDictionary<Saml2BindingType, Saml2Binding> bindings = 
            new Dictionary<Saml2BindingType, Saml2Binding>()
            {
                { Saml2BindingType.HttpRedirect, new Saml2RedirectBinding() }
            };

        public static Saml2Binding Get(Saml2BindingType binding)
        {
            return bindings[binding];
        }
    }
}

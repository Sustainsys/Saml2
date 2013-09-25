using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices
{
    abstract class Saml2Binding
    {
        public virtual CommandResult Bind(Saml2AuthenticationRequest request)
        {
            throw new NotImplementedException();
        }

        public virtual Saml2Response Unbind(HttpRequestBase request)
        {
            throw new NotImplementedException();
        }

        protected virtual bool CanUnbind(HttpRequestBase request)
        {
            return false;
        }

        private static readonly IDictionary<Saml2BindingType, Saml2Binding> bindings = 
            new Dictionary<Saml2BindingType, Saml2Binding>()
            {
                { Saml2BindingType.HttpRedirect, new Saml2RedirectBinding() },
                { Saml2BindingType.HttpPost, new Saml2PostBinding() }
            };

        public static Saml2Binding Get(Saml2BindingType binding)
        {
            return bindings[binding];
        }

        public static Saml2Binding Get(HttpRequestBase request)
        {
            return bindings.FirstOrDefault(b => b.Value.CanUnbind(request)).Value;
        }
    }
}

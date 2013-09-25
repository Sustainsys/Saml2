using System;
using System.Linq;

namespace Kentor.AuthServices
{
    class Saml2PostBinding : Saml2Binding
    {
        public override CommandResult Bind(Saml2AuthenticationRequest request)
        {
            throw new NotImplementedException();
        }

        public override bool CanUnbind(System.Web.HttpRequestBase request)
        {
            return request.HttpMethod == "POST" 
                && request.Form.AllKeys.Contains("SAMLResponse");
        }
    }
}

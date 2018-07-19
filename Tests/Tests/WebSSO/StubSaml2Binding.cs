using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Tests.WebSSO
{
    class StubSaml2Binding : Saml2Binding
    {
        public override UnbindResult Unbind(HttpRequestData request, IOptions options)
        {
            throw new NotImplementedException(
                $"{nameof(StubSaml2Binding)}.{nameof(Unbind)} was called");
        }

        protected internal override bool CanUnbind(HttpRequestData request)
        {
            throw new NotImplementedException();
        }
    }
}

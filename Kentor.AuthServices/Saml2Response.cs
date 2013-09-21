using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    public class Saml2Response
    {
        public static Saml2Response Read(string xml)
        {
            throw new NotImplementedException();
        }

        private Saml2Response(string id)
        {
            this.id = id;
        }

        readonly string id;
        public string Id { get { return id; } }
    }
}

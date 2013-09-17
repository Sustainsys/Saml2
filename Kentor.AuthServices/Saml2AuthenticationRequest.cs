using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// An authentication request corresponding to section 3.4.1 in SAML Core specification.
    /// </summary>
    public class Saml2AuthenticationRequest : Saml2RequestBase
    {
        /// <summary>
        /// Serializes the request to a Xml message.
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            var x = new XElement(Saml2Namespaces.Saml2P + "AuthnRequest");

            x.Add(base.ToXNodes());

            return x;
        }
    }
}

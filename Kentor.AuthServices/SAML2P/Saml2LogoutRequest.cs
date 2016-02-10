using Kentor.AuthServices.Saml2P;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// A Saml2 LogoutRequest message (SAML core spec 3.7.1)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
    public class Saml2LogoutRequest : Saml2RequestBase
    {
        /// <summary>
        /// The SAML2 request name
        /// </summary>
        protected override string LocalName
        {
            get
            {
                return "LogoutRequest";
            }
        }

        /// <summary>
        /// Name id to logout.
        /// </summary>
        public Saml2NameIdentifier NameId { get; set; }

        /// <summary>
        /// Session index to logout.
        /// </summary>
        public string SessionIndex { get; set; }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public override string ToXml()
        {
            var x = new XElement(Saml2Namespaces.Saml2P + LocalName);

            x.Add(base.ToXNodes());
            var nameId = new XElement(Saml2Namespaces.Saml2 + "NameID",
                NameId.Value);
            nameId.AddAttributeIfNotNullOrEmpty("Format", NameId.Format);
            x.Add(nameId);

            x.Add(new XElement(Saml2Namespaces.Saml2P + "SessionIndex",
                SessionIndex));

            return x.ToString();
        }
    }
}

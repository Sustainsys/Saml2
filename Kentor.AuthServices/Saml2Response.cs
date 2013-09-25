using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices
{
    public class Saml2Response
    {
        public static Saml2Response Read(string xml)
        {
            var x = new XmlDocument();
            x.LoadXml(xml);

            if (x.FirstChild.LocalName != "Response"
                || x.FirstChild.NamespaceURI != Saml2Namespaces.Saml2P)
            {
                throw new XmlException("Expected a SAML2 assertion document");
            }

            if (x.FirstChild.Attributes["Version"].Value != "2.0")
            {
                throw new XmlException("Wrong or unsupported SAML2 version");
            }

            return new Saml2Response(x);
        }

        private Saml2Response(XmlDocument xml)
        {
            id = xml.FirstChild.Attributes["ID"].Value;

            issueInstant = DateTime.Parse(xml.FirstChild.Attributes["IssueInstant"].Value, 
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            var statusString = xml.FirstChild["Status", Saml2Namespaces.Saml2PName]
                ["StatusCode", Saml2Namespaces.Saml2PName].Attributes["Value"].Value;

            status = StatusCodeHelper.FromString(statusString);
        }

        readonly string id;
        public string Id { get { return id; } }

        readonly DateTime issueInstant;
        public DateTime IssueInstant { get { return issueInstant; } }

        readonly Saml2StatusCode status;
        public Saml2StatusCode Status { get { return status; } }
    }
}

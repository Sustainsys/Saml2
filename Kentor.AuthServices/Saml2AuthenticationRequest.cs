using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// An authentication request corresponding to section 3.4.1 in SAML Core specification.
    /// </summary>
    public class Saml2AuthenticationRequest : Saml2RequestBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Saml2AuthenticationRequest()
        {

        }

        /// <summary>
        /// Serializes the request to a Xml message.
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            var x = new XElement(Saml2Namespaces.Saml2P + "AuthnRequest");

            x.Add(base.ToXNodes());
            x.AddAttributeIfNotNullOrEmpty("AssertionConsumerServiceURL", AssertionConsumerServiceUrl);

            return x;
        }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public override string ToXml()
        {
            return ToXElement().ToString();
        }

        /// <summary>
        /// Read the supplied Xml and parse it into a authenticationrequest.
        /// </summary>
        /// <param name="xml">xml data.</param>
        /// <returns>Saml2Request</returns>
        /// <exception cref="XmlException">On xml errors or unexpected xml structure.</exception>
        public static Saml2AuthenticationRequest Read(string xml)
        {
            var x = new XmlDocument();
            x.PreserveWhitespace = true;
            x.LoadXml(xml);

            if (x.DocumentElement.LocalName != "AuthnRequest"
                || x.DocumentElement.NamespaceURI != Saml2Namespaces.Saml2P)
            {
                throw new XmlException("Expected a SAML2 authentication request document");
            }

            if (x.DocumentElement.Attributes["Version"].Value != "2.0")
            {
                throw new XmlException("Wrong or unsupported SAML2 version");
            }

            return new Saml2AuthenticationRequest(x);
        }

        private Saml2AuthenticationRequest(XmlDocument xml)
        {
            //var xmlDocument = xml;

            Id = xml.DocumentElement.Attributes["ID"].Value;

            Issuer = xml.DocumentElement["Issuer", Saml2Namespaces.Saml2Name].GetTrimmedTextIfNotNull();

            var AssertionConsumerServiceUriString = xml.DocumentElement.Attributes["AssertionConsumerServiceURL"].GetValueIfNotNull();

            if (AssertionConsumerServiceUriString != null)
            {
                AssertionConsumerServiceUrl = new Uri(AssertionConsumerServiceUriString);
            }
        }
        /// <summary>
        /// The assertion consumer url that the idp should send its response back to.
        /// </summary>
        public Uri AssertionConsumerServiceUrl { get; set; }
    }
}

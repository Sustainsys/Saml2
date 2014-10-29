using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Saml2P
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
        /// The SAML2 request name
        /// </summary>
        protected override string LocalName
        {
            get { return "AuthnRequest"; }
        }

        /// <summary>
        /// Serializes the request to a Xml message.
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            var x = new XElement(Saml2Namespaces.Saml2P + LocalName);

            x.Add(base.ToXNodes());
            x.AddAttributeIfNotNullOrEmpty("AssertionConsumerServiceURL", AssertionConsumerServiceUrl);
            x.AddAttributeIfNotNullOrEmpty("AttributeConsumingServiceIndex", AttributeConsumingServiceIndex);

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
            if (xml == null)
            {
                return null;
            }
            var x = new XmlDocument();
            x.PreserveWhitespace = true;
            x.LoadXml(xml);

            return new Saml2AuthenticationRequest(x);
        }

        private Saml2AuthenticationRequest(XmlDocument xml)
        {
            ReadBaseProperties(xml);

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

        /// <summary>
        /// Index to the SP metadata where the list of requested attributes is found.
        /// </summary>
        public int? AttributeConsumingServiceIndex { get; set; }
    }
}

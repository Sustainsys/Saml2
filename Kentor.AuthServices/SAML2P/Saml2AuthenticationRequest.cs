using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.WebSso;

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
            RelayState = SecureKeyGenerator.CreateRelayState();
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

            if (NameIdPolicy != null && 
                (NameIdPolicy.AllowCreate.HasValue || NameIdPolicy.Format != NameIdFormat.NotConfigured))
            {
                var nameIdPolicyElement = new XElement(Saml2Namespaces.Saml2P + "NameIDPolicy");

                if(NameIdPolicy.Format != NameIdFormat.NotConfigured)
                {
                    nameIdPolicyElement.Add(new XAttribute("Format",
                        NameIdPolicy.Format.GetString()));
                }

                if(NameIdPolicy.AllowCreate.HasValue)
                {
                    nameIdPolicyElement.Add(new XAttribute("AllowCreate",
                        NameIdPolicy.AllowCreate));
                }

                x.Add(nameIdPolicyElement);
            }

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
        /// <param name="relayState">Relay State attached to the message or null if not present.</param>
        /// <returns>Saml2Request</returns>
        /// <exception cref="XmlException">On xml errors or unexpected xml structure.</exception>
        public static Saml2AuthenticationRequest Read(string xml, string relayState)
        {
            if (xml == null)
            {
                return null;
            }
            var x = new XmlDocument();
            x.PreserveWhitespace = true;
            x.LoadXml(xml);

            return new Saml2AuthenticationRequest(x.DocumentElement, relayState);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="xml">Xml data</param>
        /// <param name="relayState">RelayState associateed with the message.</param>
        public Saml2AuthenticationRequest(XmlElement xml, string relayState)
        {
            ReadBaseProperties(xml);
            RelayState = relayState;

            var AssertionConsumerServiceUriString = xml.Attributes["AssertionConsumerServiceURL"].GetValueIfNotNull();

            if (AssertionConsumerServiceUriString != null)
            {
                AssertionConsumerServiceUrl = new Uri(AssertionConsumerServiceUriString);
            }

            var node = xml["NameIDPolicy", Saml2Namespaces.Saml2PName];
            if (node != null)
            {
                NameIdPolicy = new Saml2NameIdPolicy();
                var fullFormat = node.Attributes["Format"].GetValueIfNotNull();
                var format = fullFormat?.Split(':').LastOrDefault();
                if (format != null)
                {
                    NameIdFormat namedIdFormat;
                    if (Enum.TryParse(format, true, out namedIdFormat))
                    {
                        NameIdPolicy.Format = namedIdFormat;
                    }
                }

                var allowCreateStr = node.Attributes["AllowCreate"].GetValueIfNotNull();
                if (allowCreateStr != null)
                {
                    NameIdPolicy.AllowCreate = bool.Parse(allowCreateStr);
                }
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

        /// <summary>
        /// NameId policy.
        /// </summary>
        public Saml2NameIdPolicy NameIdPolicy { get; set; }
    }
}

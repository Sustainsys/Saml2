using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// A logout request corresponding to section 3.7.1 in SAML Core specification.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
    public class Saml2LogoutRequest : Saml2RequestBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Saml2LogoutRequest()
        {

        }

        /// <summary>
        /// The SAML2 logout request name
        /// </summary>
        protected override string LocalName
        {
            get { return "LogoutRequest"; }
        }

        /// <summary>
        /// The value of the nameidentifier claim.
        /// </summary>
        public string NameIdentifierValue { get; set; }

        /// <summary>
        ///  The format in which the nameidentifier is provided.
        /// </summary>
        public string NameIdentifierFormat { get; set; }

        /// <summary>
        /// Serializes the request to a Xml message.
        /// </summary>
        /// <returns>XElement</returns>
        public XElement ToXElement()
        {
            var x = new XElement(Saml2Namespaces.Saml2P + LocalName);

            x.Add(base.ToXNodes());

            var nameIdentifier = new XElement(Saml2Namespaces.Saml2 + "NameID", NameIdentifierValue);
            if (!string.IsNullOrEmpty(NameIdentifierFormat))
            {
                nameIdentifier.Add(new XAttribute("Format", NameIdentifierFormat));
            }
            x.Add(nameIdentifier);

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
        public static Saml2LogoutRequest Read(string xml)
        {
            if (xml == null)
            {
                return null;
            }
            var x = new XmlDocument();
            x.PreserveWhitespace = true;
            x.LoadXml(xml);

            return new Saml2LogoutRequest(x);
        }

        private Saml2LogoutRequest(XmlDocument xml)
        {
            ReadBaseProperties(xml);
        }

    }
}

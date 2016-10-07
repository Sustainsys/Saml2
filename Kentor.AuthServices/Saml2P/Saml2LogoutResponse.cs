using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// A Saml2 Logout Response.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout")]
    public class Saml2LogoutResponse : Saml2StatusResponseType
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="status">Status of the response.</param>
        public Saml2LogoutResponse(Saml2StatusCode status)
            : base(status)
        { }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public override string ToXml()
        {
            var doc = new XmlDocument();
            AppendTo(doc);
            return doc.DocumentElement.OuterXml;
        }

        /// <summary>
        /// Appends xml for the Saml2LogoutResponse to the given parent node.
        /// </summary>
        /// <param name="parentNode">Xml for the Saml2LogoutResponse is appended
        /// to the children of this node.</param>
        public void AppendTo(XmlNode parentNode)
        {
            parentNode.StartElement("LogoutResponse", Saml2Namespaces.Saml2PUri)
                .AddAttribute("ID", Id.Value)
                .AddAttribute("Version", "2.0")
                .AddAttribute("IssueInstant", IssueInstant.ToSaml2DateTimeString())
                .AddAttributeIfNotNull("InResponseTo", InResponseTo)
                .AddAttributeIfNotNull("Destination", DestinationUrl?.OriginalString)
                .If(Issuer != null, x => x.AddElement("Issuer", Saml2Namespaces.Saml2Uri, Issuer.Id))
                .StartElement("Status", Saml2Namespaces.Saml2PUri)
                    .StartElement("StatusCode", Saml2Namespaces.Saml2PUri)
                    .AddAttribute("Value", StatusCodeHelper.FromCode(Status));
        }

        /// <summary>
        /// Load values into 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static Saml2LogoutResponse FromXml(XmlElement xml)
        {
            if(xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            var status = StatusCodeHelper.FromString(
                xml["Status", Saml2Namespaces.Saml2PName]
                ["StatusCode", Saml2Namespaces.Saml2PName]
                .GetAttribute("Value"));
            return new Saml2LogoutResponse(status);
        }
    }
}

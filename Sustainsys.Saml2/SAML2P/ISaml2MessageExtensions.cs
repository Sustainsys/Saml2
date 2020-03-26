using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sustainsys.Saml2.Saml2P
{
    static class Saml2MessageExtensions
    {
        /// <summary>
        /// Serializes the message into wellformed XML.
        /// </summary>
        /// <param name="message">Saml2 message to transform to XML</param>
        /// <param name="xmlCreatedNotification">Notification allowing modification of XML tree before serialization.</param>
        /// <returns>string containing the Xml data.</returns>
        public static string ToXml<TMessage>(
            this TMessage message, Action<XDocument> xmlCreatedNotification)
            where TMessage : ISaml2Message
        {
            var xDocument = new XDocument(message.ToXElement());

            xmlCreatedNotification(xDocument);

            return xDocument.ToStringWithXmlDeclaration();
        }
    }
}

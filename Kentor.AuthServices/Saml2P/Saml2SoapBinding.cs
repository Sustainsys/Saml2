using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Saml2 Soap binding implementation.
    /// </summary>
    /// <remarks>
    /// This class does not follow the pattern of the other three bindings
    /// (Redirect, POST and Artifact) because it does not use the front channel
    /// with messages being passed over the user's browser.
    /// </remarks>
    public static class Saml2SoapBinding
    {
        const string soapFormatString = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body>{0}</SOAP-ENV:Body></SOAP-ENV:Envelope>";

        /// <summary>
        /// Create a SOAP body around a specified payload.
        /// </summary>
        /// <param name="payload">Payload of the message.</param>
        /// <returns></returns>
        public static string CreateSoapBody(string payload)
        {
            return string.Format(CultureInfo.InvariantCulture,
                soapFormatString, payload);
        }

        /// <summary>
        /// Extract the body of a SOAP message.
        /// </summary>
        /// <param name="xml">xml data</param>
        /// <returns>Parsed data.</returns>
        public static XmlElement ExtractBody(string xml)
        {
            var xmlDoc = new XmlDocument()
            {
                PreserveWhitespace = true
            };
            xmlDoc.LoadXml(xml);

            return xmlDoc.DocumentElement["Body", Saml2Namespaces.SoapEnvelopeName]
                .ChildNodes.OfType<XmlElement>().Single();
        }

        /// <summary>
        /// Send a SOAP request to the specified endpoint and return the result.
        /// </summary>
        /// <param name="payload">Message payload</param>
        /// <param name="destination">Destination endpoint</param>
        /// <returns>Response.</returns>
        public static XmlElement SendSoapRequest(string payload, Uri destination)
        {
            var message = CreateSoapBody(payload);

            using (var client = new WebClient())
            {
                client.Headers.Add("SOAPAction", "http://www.oasis-open.org/committees/security");
                var response = client.UploadString(destination, message);

                return ExtractBody(response);
            }
        }
    }
}

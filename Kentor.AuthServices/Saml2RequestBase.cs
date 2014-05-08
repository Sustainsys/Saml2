using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Base class for saml requests, corresponds to section 3.2.1 in SAML Core specification.
    /// </summary>
    public abstract class Saml2RequestBase : ISaml2Message
    {
        private readonly string id = "id" + Guid.NewGuid().ToString("N");

        /// <summary>
        /// The id of the request.
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Version of the SAML request. Always returns "2.0"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string Version
        {
            get
            {
                return "2.0";
            }
        }

        private readonly string issueInstant = 
            DateTime.UtcNow.ToSaml2DateTimeString();

        /// <summary>
        /// The instant that the request was issued (well actually, created).
        /// </summary>
        public string IssueInstant
        {
            get
            {
                return issueInstant;
            }
        }

        /// <summary>
        /// SAML message name for requests - hard coded to SAMLRequest.
        /// </summary>
        public string MessageName
        {
            get
            {
                return "SAMLRequest";
            }
        }

        /// <summary>
        /// The destination of the request.
        /// </summary>
        public Uri DestinationUri { get; set; }

        /// <summary>
        /// The issuer of the request.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Creates XNodes for the fields of the Saml2RequestBase class. These
        /// nodes should be added when creating XML out of derived classes.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<XObject> ToXNodes()
        {
            yield return new XAttribute(XNamespace.Xmlns + "saml2p", Saml2Namespaces.Saml2PName);
            yield return new XAttribute(XNamespace.Xmlns + "saml2", Saml2Namespaces.Saml2Name);
            yield return new XAttribute("ID", Id);
            yield return new XAttribute("Version", Version);
            yield return new XAttribute("IssueInstant", IssueInstant);

            if (DestinationUri != null)
            {
                yield return new XAttribute("Destination", DestinationUri);
            }

            if (!string.IsNullOrEmpty(Issuer))
            {
                yield return new XElement(Saml2Namespaces.Saml2 + "Issuer", Issuer);
            }
        }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public abstract string ToXml();
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Base class for saml requests, corresponds to section 3.2.1 in SAML Core specification.
    /// </summary>
    public abstract class Saml2RequestBase : ISaml2Message
    {
        private string id = "id" + Guid.NewGuid().ToString("N");

        /// <summary>
        /// The id of the request.
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
            protected set
            {
                id = value;
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
        public Uri DestinationUrl { get; set; }

        /// <summary>
        /// The issuer of the request.
        /// </summary>
        public EntityId Issuer { get; set; }

        /// <summary>
        /// The SAML2 request name
        /// </summary>
        protected abstract string LocalName { get; }

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

            if (DestinationUrl != null)
            {
                yield return new XAttribute("Destination", DestinationUrl);
            }

            if (Issuer != null && !string.IsNullOrEmpty(Issuer.Id))
            {
                yield return new XElement(Saml2Namespaces.Saml2 + "Issuer", Issuer.Id);
            }
        }

        /// <summary>
        /// Reads the request properties present in Saml2RequestBase
        /// Also validates basic properties of the request
        /// </summary>
        /// <param name="xml">The xml document to parse</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        protected void ReadBaseProperties(XmlDocument xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }
            ValidateCorrectDocument(xml);
            Id = xml.DocumentElement.Attributes["ID"].Value;

            Issuer = new EntityId(xml.DocumentElement["Issuer", Saml2Namespaces.Saml2Name].GetTrimmedTextIfNotNull());
        }

        private void ValidateCorrectDocument(XmlDocument xml)
        {
            if (xml.DocumentElement.LocalName != LocalName
               || xml.DocumentElement.NamespaceURI != Saml2Namespaces.Saml2P)
            {
                throw new XmlException("Expected a SAML2 authentication request document");
            }

            if (xml.DocumentElement.Attributes["Version"].Value != "2.0")
            {
                throw new XmlException("Wrong or unsupported SAML2 version");
            }
        }

        /// <summary>
        /// Serializes the message into wellformed Xml.
        /// </summary>
        /// <returns>string containing the Xml data.</returns>
        public abstract string ToXml();
    }
}

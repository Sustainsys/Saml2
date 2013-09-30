using Kentor.AuthServices.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Represents a SAML2 response according to 3.3.3
    /// </summary>
    public class Saml2Response
    {
        private readonly XmlDocument xmlDocument;

        /// <summary>
        /// Read the supplied Xml and parse it into a response.
        /// </summary>
        /// <param name="xml">xml data.</param>
        /// <returns>Saml2Response</returns>
        /// <exception cref="XmlException">On xml errors or unexpected xml structure.</exception>
        public static Saml2Response Read(string xml)
        {
            var x = new XmlDocument();
            x.LoadXml(xml);

            if (x.DocumentElement.LocalName != "Response"
                || x.DocumentElement.NamespaceURI != Saml2Namespaces.Saml2P)
            {
                throw new XmlException("Expected a SAML2 assertion document");
            }

            if (x.DocumentElement.Attributes["Version"].Value != "2.0")
            {
                throw new XmlException("Wrong or unsupported SAML2 version");
            }

            return new Saml2Response(x);
        }

        private Saml2Response(XmlDocument xml)
        {
            xmlDocument = xml;

            id = xml.DocumentElement.Attributes["ID"].Value;

            issueInstant = DateTime.Parse(xml.DocumentElement.Attributes["IssueInstant"].Value,
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            var statusString = xml.DocumentElement["Status", Saml2Namespaces.Saml2PName]
                ["StatusCode", Saml2Namespaces.Saml2PName].Attributes["Value"].Value;

            status = StatusCodeHelper.FromString(statusString);
        }

        readonly string id;

        /// <summary>
        /// Id of the response message.
        /// </summary>
        public string Id { get { return id; } }

        readonly DateTime issueInstant;

        /// <summary>
        /// Issue instant of the response message.
        /// </summary>
        public DateTime IssueInstant { get { return issueInstant; } }

        readonly Saml2StatusCode status;

        /// <summary>
        /// Status code of the message according to the SAML2 spec section 3.2.2.2
        /// </summary>
        public Saml2StatusCode Status { get { return status; } }

        /// <summary>
        /// Issuer (= sender) of the response.
        /// </summary>
        public string Issuer
        {
            get
            {
                return xmlDocument.DocumentElement["Issuer", Saml2Namespaces.Saml2Name].GetTrimmedTextIfNotNull();
            }
        }


        bool valid = false, validated = false;

        /// <summary>
        /// Validates the response.
        /// </summary>
        /// <param name="idpCertificate">Idp certificate that should have signed the reponse</param>
        /// <returns>Is the response signed by the Idp and fulfills other formal requirements?</returns>
        public bool Validate(X509Certificate2 idpCertificate)
        {
            if (!validated)
            {
                var signedXml = new SignedXml(xmlDocument);

                var signature = xmlDocument.DocumentElement["Signature", Saml2Namespaces.DsigName];

                if (signature != null)
                {
                    signedXml.LoadXml(signature);

                    valid = signedXml.CheckSignature(idpCertificate, true);
                }
                else
                {
                    valid = false;
                }
                validated = true;
            }
            return valid;
        }

        private void ThrowOnNotValid()
        {
            if (!validated)
            {
                throw new InvalidOperationException("The Saml2Response must be validated first.");
            }
            if (!valid)
            {
                throw new InvalidOperationException("The Saml2Response didn't pass validation");
            }
        }

        private IEnumerable<ClaimsIdentity> claimsIdentities;
        private Exception createClaimsException;

        /// <summary>
        /// Extract claims from the assertions contained in the response.
        /// </summary>
        /// <returns>ClaimsIdentities</returns>
        // Method might throw expections so make it a method and not a property.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<ClaimsIdentity> GetClaims()
        {
            if (createClaimsException != null)
            {
                throw createClaimsException;
            }

            if (claimsIdentities == null)
            {
                try
                {
                    claimsIdentities = CreateClaims().ToList();
                }
                catch (Exception ex)
                {
                    createClaimsException = ex;
                    throw;
                }
            }

            return claimsIdentities;
        }

        private IEnumerable<ClaimsIdentity> CreateClaims()
        {
            ThrowOnNotValid();

            foreach (XmlElement assertionNode in xmlDocument.DocumentElement.ChildNodes.Cast<XmlElement>()
                .Where(xe => xe.LocalName == "Assertion" && xe.NamespaceURI == Saml2Namespaces.Saml2Name))
            {
                using (var reader = new XmlNodeReader(assertionNode))
                {
                    MorePublicSaml2SecurityTokenHandler handler = MorePublicSaml2SecurityTokenHandler.DefaultInstance;

                    var token = (Saml2SecurityToken)MorePublicSaml2SecurityTokenHandler.DefaultInstance.ReadToken(reader);
                    handler.DetectReplayedToken(token);

                    var validateAudience = token.Assertion.Conditions.AudienceRestrictions.Count > 0;

                    handler.ValidateConditions(token.Assertion.Conditions, validateAudience);

                    yield return handler.CreateClaims(token);
                }
            }
        }
    }
}

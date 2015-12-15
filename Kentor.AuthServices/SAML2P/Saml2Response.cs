using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Security.Cryptography;
using System.IdentityModel.Services;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// Represents a SAML2 response according to 3.3.3. The class is immutable (to an
    /// external observer. Internal state is lazy initiated).
    /// </summary>
    public class Saml2Response : ISaml2Message
    {
        /// <summary>Holds all assertion element nodes</summary>
        private IEnumerable<XmlElement> allAssertionElementNodes;

        /// <summary>
        /// Read the supplied Xml and parse it into a response.
        /// </summary>
        /// <param name="xml">xml data.</param>
        /// <returns>Saml2Response</returns>
        /// <exception cref="XmlException">On xml errors or unexpected xml structure.</exception>
        public static Saml2Response Read(string xml)
        {
            var x = new XmlDocument();
            x.PreserveWhitespace = true;
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

            id = new Saml2Id(xml.DocumentElement.Attributes["ID"].Value);

            var parsedInResponseTo = xml.DocumentElement.Attributes["InResponseTo"].GetValueIfNotNull();
            if (parsedInResponseTo != null)
            {
                inResponseTo = new Saml2Id(parsedInResponseTo);
            }

            issueInstant = DateTime.Parse(xml.DocumentElement.Attributes["IssueInstant"].Value,
                CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            var statusString = xml.DocumentElement["Status", Saml2Namespaces.Saml2PName]
                ["StatusCode", Saml2Namespaces.Saml2PName].Attributes["Value"].Value;

            status = StatusCodeHelper.FromString(statusString);

            statusMessage = xml.DocumentElement["Status", Saml2Namespaces.Saml2PName]
                ["StatusMessage", Saml2Namespaces.Saml2PName].GetTrimmedTextIfNotNull();

            issuer = new EntityId(xmlDocument.DocumentElement["Issuer", Saml2Namespaces.Saml2Name].GetTrimmedTextIfNotNull());

            var destinationUrlString = xmlDocument.DocumentElement.Attributes["Destination"].GetValueIfNotNull();

            if (destinationUrlString != null)
            {
                destinationUrl = new Uri(destinationUrlString);
            }
        }

        /// <summary>
        /// Create a response with the supplied data.
        /// </summary>
        /// <param name="issuer">Issuer of the response.</param>
        /// <param name="issuerCertificate">The certificate to use when signing
        /// this response in XML form.</param>
        /// <param name="destinationUrl">The destination Uri for the message</param>
        /// <param name="inResponseTo">In response to id</param>
        /// <param name="claimsIdentities">Claims identities to be included in the 
        /// response. Each identity is translated into a separate assertion.</param>
        public Saml2Response(EntityId issuer, X509Certificate2 issuerCertificate,
            Uri destinationUrl, string inResponseTo, params ClaimsIdentity[] claimsIdentities)
        {
            this.issuer = issuer;
            this.claimsIdentities = claimsIdentities;
            this.issuerCertificate = issuerCertificate;
            this.destinationUrl = destinationUrl;
            if (inResponseTo != null)
            {
                this.inResponseTo = new Saml2Id(inResponseTo);
            }
            id = new Saml2Id("id" + Guid.NewGuid().ToString("N"));
            status = Saml2StatusCode.Success;
        }

        private readonly X509Certificate2 issuerCertificate;

        private XmlDocument xmlDocument;

        /// <summary>
        /// The response as an xml docuemnt. Either the original xml, or xml that is
        /// generated from supplied data.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public XmlDocument XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    CreateXmlDocument();
                }

                return xmlDocument;
            }
        }

        /// <summary>
        /// SAML Message name for responses, hard coded to SAMLResponse.
        /// </summary>
        public string MessageName
        {
            get
            {
                return "SAMLResponse";
            }
        }

        /// <summary>
        /// string representation of the Saml2Response serialized to xml.
        /// </summary>
        /// <returns>string containing xml.</returns>
        public string ToXml()
        {
            return XmlDocument.OuterXml;
        }

        private void CreateXmlDocument()
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));

            var responseElement = xml.CreateElement("saml2p", "Response", Saml2Namespaces.Saml2PName);

            if (DestinationUrl != null)
            {
                responseElement.SetAttributeNode("Destination", "").Value = DestinationUrl.ToString();
            }

            responseElement.SetAttributeNode("ID", "").Value = id.Value;
            responseElement.SetAttributeNode("Version", "").Value = "2.0";
            responseElement.SetAttributeNode("IssueInstant", "").Value =
                DateTime.UtcNow.ToSaml2DateTimeString();
            if (InResponseTo != null)
            {
                responseElement.SetAttributeNode("InResponseTo", "").Value = InResponseTo.Value;
            }
            xml.AppendChild(responseElement);

            var issuerElement = xml.CreateElement("saml2", "Issuer", Saml2Namespaces.Saml2Name);
            issuerElement.InnerText = issuer.Id;
            responseElement.AppendChild(issuerElement);

            var statusElement = xml.CreateElement("saml2p", "Status", Saml2Namespaces.Saml2PName);
            var statusCodeElement = xml.CreateElement("saml2p", "StatusCode", Saml2Namespaces.Saml2PName);
            statusCodeElement.SetAttributeNode("Value", "").Value = StatusCodeHelper.FromCode(Status);
            statusElement.AppendChild(statusCodeElement);
            responseElement.AppendChild(statusElement);

            foreach (var ci in claimsIdentities)
            {
                responseElement.AppendChild(xml.ReadNode(
                    ci.ToSaml2Assertion(issuer).ToXElement().CreateReader()));
            }

            xmlDocument = xml;

            xml.Sign(issuerCertificate, true);
        }

        readonly Saml2Id id;

        /// <summary>
        /// Id of the response message.
        /// </summary>
        public Saml2Id Id { get { return id; } }

        readonly Saml2Id inResponseTo;

        /// <summary>
        /// InResponseTo id.
        /// </summary>
        public Saml2Id InResponseTo { get { return inResponseTo; } }

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

        readonly string statusMessage;

        /// <summary>
        /// StatusMessage of the message according to the SAML2 spec section 3.2.2.1
        /// </summary>
        public string StatusMessage { get { return statusMessage; } }

        readonly EntityId issuer;

        /// <summary>
        /// Issuer (= sender) of the response.
        /// </summary>
        public EntityId Issuer
        {
            get
            {
                return issuer;
            }
        }

        readonly Uri destinationUrl;

        /// <summary>
        /// The destination of the response message.
        /// </summary>
        public Uri DestinationUrl
        {
            get
            {
                return destinationUrl;
            }
        }

        StoredRequestState requestState;

        /// <summary>
        /// State stored by a corresponding request
        /// </summary>
        public StoredRequestState GetRequestState(IOptions options)
        {
            Validate(options);
            return requestState;
        }

        /// <summary>Gets all assertion element nodes from this response message.</summary>
        /// <value>All assertion element nodes.</value>
        private IEnumerable<XmlElement> AllAssertionElementNodes
        {
            get { return allAssertionElementNodes ?? (allAssertionElementNodes = retrieveAssertionElements()); }
        }

        private IEnumerable<XmlElement> retrieveAssertionElements()
        {
            var assertions = new List<XmlElement>();

            assertions.AddRange(XmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>()
                .Where(node => node.NodeType == XmlNodeType.Element).Cast<XmlElement>()
                .Where(xe => xe.LocalName == "Assertion" && xe.NamespaceURI == Saml2Namespaces.Saml2Name));

            var encryptedAssertions = XmlDocument.DocumentElement.ChildNodes.Cast<XmlNode>()
                .Where(node => node.NodeType == XmlNodeType.Element).Cast<XmlElement>()
                .Where(xe => xe.LocalName == "EncryptedAssertion" && xe.NamespaceURI == Saml2Namespaces.Saml2Name);

            if (encryptedAssertions.Count() > 0)
            {
                if (serviceCertificate == null)
                {
                    throw new Saml2ResponseFailedValidationException("Encrypted Assertions encountered but Service Certificate was not provided.");
                }
                else if (!serviceCertificate.HasPrivateKey)
                {
                    throw new Saml2ResponseFailedValidationException("Encrypted Assertions encountered but Service Certificate does not contain private key.");
                }

                assertions.AddRange(encryptedAssertions.Decrypt(serviceCertificate.PrivateKey)
                    .Select(xe => (XmlElement)xe.GetElementsByTagName("Assertion", Saml2Namespaces.Saml2Name)[0]));
            }

            return assertions;
        }

        bool validated = false;
        Saml2ResponseFailedValidationException validationException;
        private X509Certificate2 serviceCertificate;

        private void Validate(IOptions options)
        {
            if (!validated)
            {
                serviceCertificate = options.SPOptions.ServiceCertificate;
                try
                {
                    ValidateInResponseTo(options);
                    ValidateSignature(options);
                }
                catch (Saml2ResponseFailedValidationException ex)
                {
                    validationException = ex;
                    throw;
                }
                finally
                {
                    validated = true;
                }
            }
            else
            {
                if (validationException != null)
                {
                    throw validationException;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "InResponseTo")]
        private void ValidateInResponseTo(IOptions options)
        {
            if (InResponseTo == null)
            {
                if (options.IdentityProviders[Issuer].AllowUnsolicitedAuthnResponse)
                {
                    return;
                }
                string msg = string.Format(CultureInfo.InvariantCulture,
                    "Unsolicited responses are not allowed for idp \"{0}\".", Issuer.Id);
                throw new Saml2ResponseFailedValidationException(msg);
            }
            else
            {
                StoredRequestState storedRequestState;
                bool knownInResponseToId = PendingAuthnRequests.TryRemove(InResponseTo, out storedRequestState);
                if (!knownInResponseToId)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture,
                        "Replayed or unknown InResponseTo \"{0}\".", InResponseTo);

                    throw new Saml2ResponseFailedValidationException(msg);
                }
                requestState = storedRequestState;
                if (requestState.Idp.Id != Issuer.Id)
                {
                    var msg = string.Format(CultureInfo.InvariantCulture,
                        "Expected response from idp \"{0}\" but received response from idp \"{1}\".",
                        requestState.Idp.Id, issuer.Id);
                    throw new Saml2ResponseFailedValidationException(msg);
                }
            }
        }

        private void ValidateSignature(IOptions options)
        {
            var idpKeys = options.IdentityProviders[Issuer].SigningKeys;

            // If the response message is signed, we check just this signature because the whole content has to be correct then
            var responseSignature = xmlDocument.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];
            if (responseSignature != null)
            {
                CheckSignature(XmlDocument.DocumentElement, idpKeys);
            }
            else
            {
                // If the response message is not signed, all assersions have to be signed correctly
                foreach (var assertionNode in AllAssertionElementNodes)
                {
                    CheckSignature(assertionNode, idpKeys);
                }
            }
        }

        private static readonly string[] allowedTransforms = new string[]
            {
            SignedXml.XmlDsigEnvelopedSignatureTransformUrl,
            SignedXml.XmlDsigExcC14NTransformUrl,
            SignedXml.XmlDsigExcC14NWithCommentsTransformUrl
            };

        /// <summary>Checks the signature.</summary>
        /// <param name="signedRootElement">The signed root element.</param>
        /// <param name="idpKeys">A list containing one ore more assymetric keys of a algorithm.</param>
        private static void CheckSignature(XmlElement signedRootElement, IEnumerable<AsymmetricAlgorithm> idpKeys)
        {
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(signedRootElement.OuterXml);

            var signature = xmlDocument.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];
            if (signature == null)
            {
                throw new Saml2ResponseFailedValidationException("The SAML Response is not signed and contains unsigned Assertions. Response cannot be trusted.");
            }

            var signedXml = new SignedXml(xmlDocument);
            signedXml.LoadXml(signature);

            var signedRootElementId = "#" + signedRootElement.GetAttribute("ID");

            if (signedXml.SignedInfo.References.Count == 0)
            {
                throw new Saml2ResponseFailedValidationException("No reference found in Xml signature, it doesn't validate the Xml data.");
            }

            if (signedXml.SignedInfo.References.Count != 1)
            {
                throw new Saml2ResponseFailedValidationException("Multiple references for Xml signatures are not allowed.");
            }

            var reference = signedXml.SignedInfo.References.Cast<Reference>().Single();

            if (reference.Uri != signedRootElementId)
            {
                throw new Saml2ResponseFailedValidationException("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
            }

            foreach (Transform transform in reference.TransformChain)
            {
                if (!allowedTransforms.Contains(transform.Algorithm))
                {
                    throw new Saml2ResponseFailedValidationException(
                        "Transform \"" + transform.Algorithm + "\" found in Xml signature is not allowed in SAML.");
                }
            }
            try
            {
                if (!idpKeys.Any(signedXml.CheckSignature))
                {
                    throw new Saml2ResponseFailedValidationException("Signature validation failed on SAML response or contained assertion.");
                }
            }
            catch (CryptographicException)
            {
                if (signedXml.SignatureMethod == Options.RsaSha256Namespace && CryptoConfig.CreateFromName(signedXml.SignatureMethod) == null)
                {
                    throw new Saml2ResponseFailedValidationException("SHA256 signatures require the algorithm to be registered at the process level. Call Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures() on startup to register.");
                }
                else
                {
                    throw;
                }
            }
        }

        private IEnumerable<ClaimsIdentity> claimsIdentities;
        private Exception createClaimsException;

        /// <summary>
        /// Extract claims from the assertions contained in the response.
        /// </summary>
        /// <param name="options">Service provider settings used when processing the response into claims.</param>
        /// <returns>ClaimsIdentities</returns>
        // Method might throw expections so make it a method and not a property.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<ClaimsIdentity> GetClaims(IOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (createClaimsException != null)
            {
                throw createClaimsException;
            }

            if (claimsIdentities == null)
            {
                try
                {
                    claimsIdentities = CreateClaims(options).ToList();
                }
                catch (Exception ex)
                {
                    createClaimsException = ex;
                    throw;
                }
            }

            return claimsIdentities;
        }

        private IEnumerable<ClaimsIdentity> CreateClaims(IOptions options)
        {
            Validate(options);

            if (status != Saml2StatusCode.Success)
            {
                throw new InvalidOperationException(string.Format("The Saml2Response must have status success to extract claims. Status: {0}.{1}"
                    , status.ToString(), statusMessage != null ? " Message: " + statusMessage + "." : string.Empty));
            }

            foreach (XmlElement assertionNode in AllAssertionElementNodes)
            {
                using (var reader = new FilteringXmlNodeReader(SignedXml.XmlDsigNamespaceUrl, "Signature", assertionNode))
                {
                    var handler = options.SPOptions.Saml2PSecurityTokenHandler;

                    var token = (Saml2SecurityToken)handler.ReadToken(reader);
                    handler.DetectReplayedToken(token);

                    var validateAudience = token.Assertion.Conditions.AudienceRestrictions.Count > 0;

                    handler.ValidateConditions(token.Assertion.Conditions, validateAudience);

                    yield return handler.CreateClaims(token);
                }
            }
        }
    }
}

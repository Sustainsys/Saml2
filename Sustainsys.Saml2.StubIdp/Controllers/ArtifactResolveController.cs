using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.WebSso;
using System;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace Sustainsys.Saml2.StubIdp.Controllers
{
    public class ArtifactResolveController : Controller
    {
        [HttpPost]
        public ActionResult Index()
        {
            var request = XElement.Load(Request.InputStream);

            var artifact = request
                .Element(Saml2Namespaces.SoapEnvelope + "Body")
                .Element(Saml2Namespaces.Saml2P + "ArtifactResolve")
                .Element(Saml2Namespaces.Saml2P + "Artifact")
                .Value;

            var requestId = request
                .Element(Saml2Namespaces.SoapEnvelope + "Body")
                .Element(Saml2Namespaces.Saml2P + "ArtifactResolve")
                .Attribute("ID").Value;

            var binaryArtifact = Convert.FromBase64String(artifact);

            ISaml2Message message = null;
            if(!Saml2ArtifactBinding.PendingMessages.TryRemove(binaryArtifact, out message))
            {
                throw new InvalidOperationException("Unknown artifact");
            }

            var xml = message.ToXml();

            if(message.SigningCertificate != null)
            {
                var xmlDoc = XmlHelpers.XmlDocumentFromString(xml);
                xmlDoc.Sign(message.SigningCertificate, true);
                xml = xmlDoc.OuterXml;
            }

            var response = string.Format(
                CultureInfo.InvariantCulture,
                ResponseFormatString,
                new Saml2Id().Value,
                requestId,
                DateTime.UtcNow.ToSaml2DateTimeString(),
                xml);

            return Content(response);
        }

        const string ResponseFormatString =
@"<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <samlp:ArtifactResponse
            xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID=""{0}"" Version=""2.0""
            InResponseTo = ""{1}""
            IssueInstant = ""{2}"">
            <Issuer>https://idp.example.com</Issuer>
            <samlp:Status>
                <samlp:StatusCode Value = ""urn:oasis:names:tc:SAML:2.0:status:Success"" />
            </samlp:Status>
            {3}
        </samlp:ArtifactResponse>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
    }
}
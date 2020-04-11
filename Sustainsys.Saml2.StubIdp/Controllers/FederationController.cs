using Sustainsys.Saml2.Metadata.Extensions;
using Sustainsys.Saml2.StubIdp.Models;
using System.Security.Cryptography.Xml;
using System.Web.Mvc;

namespace Sustainsys.Saml2.StubIdp.Controllers
{
    public class FederationController : Controller
    {
        // GET: Federation
        public ActionResult Index()
        {
            return Content(
                CreateMetadataString(),
                "application/samlmetadata+xml");
        }

        private static string CreateMetadataString()
        {
            return MetadataModel.CreateFederationMetadata().ToXmlString(
                CertificateHelper.SigningCertificate,
                SignedXml.XmlDsigRSASHA256Url);
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                CreateMetadataString(),
                "text/xml");
        }
    }
}
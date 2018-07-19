using Sustainsys.Saml2.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.Xml;

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
using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Kentor.AuthServices.Metadata;
using System.Security.Cryptography.Xml;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class MetadataController : Controller
    {
        // GET: Metadata
        public ActionResult Index()
        {
            return Content(
                CreateMetadataString(),
                "application/samlmetadata+xml");
        }

        private static string CreateMetadataString()
        {
            return MetadataModel.CreateIdpMetadata()
                            .ToXmlString(CertificateHelper.SigningCertificate,
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
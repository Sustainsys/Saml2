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

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class MetadataController : Controller
    {
        // GET: Metadata
        public ActionResult Index()
        {
            return Content(
                MetadataModel.CreateIdpMetadata()
                .ToXmlString(CertificateHelper.SigningCertificate),
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                MetadataModel.CreateIdpMetadata()
                .ToXmlString(CertificateHelper.SigningCertificate),
                "text/xml");
        }
    }
}
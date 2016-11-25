using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kentor.AuthServices.Metadata;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class FederationController : Controller
    {
        // GET: Federation
        public ActionResult Index()
        {
            return Content(
                MetadataModel.CreateFederationMetadata()
                .ToXmlString(CertificateHelper.SigningCertificate),
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                MetadataModel.CreateFederationMetadata()
                .ToXmlString(CertificateHelper.SigningCertificate),
                "text/xml");
        }
    }
}
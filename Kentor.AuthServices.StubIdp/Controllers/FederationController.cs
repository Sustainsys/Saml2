using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kentor.AuthServices.Metadata;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class FederationController : Controller
    {
        // GET: Federation
        public ActionResult Index()
        {
            var descriptor = MetadataModel.CreateFederationMetadata();
            descriptor.SigningCredentials = new X509SigningCredentials(
                CertificateHelper.SigningCertificate,
                SecurityAlgorithms.RsaSha1Signature,
                SecurityAlgorithms.Sha1Digest);

            return Content(descriptor.ToXmlString(),
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            var descriptor = MetadataModel.CreateFederationMetadata();
            descriptor.SigningCredentials = new X509SigningCredentials(
                CertificateHelper.SigningCertificate,
                SecurityAlgorithms.RsaSha1Signature,
                SecurityAlgorithms.Sha1Digest);

            return Content(descriptor.ToXmlString(),
                "text/xml");
        }
    }
}
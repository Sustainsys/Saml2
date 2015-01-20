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
using Kentor.AuthServices;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class MetadataController : Controller
    {
        // GET: Metadata
        public ActionResult Index()
        {
            var descriptor = MetadataModel.CreateIdpMetadata();
            descriptor.SigningCredentials = new X509SigningCredentials(
                CertificateHelper.SigningCertificate,
                SecurityAlgorithms.RsaSha1Signature,
                SecurityAlgorithms.Sha1Digest);

            return Content(descriptor.ToXmlString(),
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            var descriptor = MetadataModel.CreateIdpMetadata();
            descriptor.SigningCredentials = new X509SigningCredentials(
                CertificateHelper.SigningCertificate, 
                SecurityAlgorithms.RsaSha1Signature, 
                SecurityAlgorithms.Sha1Digest);

            return Content(descriptor.ToXmlString(),
                "text/xml");
        }
    }
}
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
    public class MetadataController : BaseController
    {
        // GET: Metadata
        public ActionResult Index(Guid? idpId)
        {
            var enforcePost = GetEnforcePostFromConfig(idpId);
            return Content(
                CreateMetadataString(enforcePost),
                "application/samlmetadata+xml");
        }        

        private static string CreateMetadataString(bool enforcePost)
        {
            return MetadataModel.CreateIdpMetadata(enforcePost)
                            .ToXmlString(CertificateHelper.SigningCertificate,
                            SignedXml.XmlDsigRSASHA256Url);
        }

        public ActionResult BrowserFriendly(Guid? idpId)
        {
            var enforcePost = GetEnforcePostFromConfig(idpId);
            return Content(
                CreateMetadataString(enforcePost),
                "text/xml");
        }
    }
}
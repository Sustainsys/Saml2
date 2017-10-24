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
            var postDefault = ReadCustomIdpConfig(idpId);
            return Content(
                CreateMetadataString(postDefault),
                "application/samlmetadata+xml");
        }

        private bool ReadCustomIdpConfig(Guid? idpId)
        {
            bool postDefault = false;

            if (idpId.HasValue)
            {
                var fileData = GetCachedConfiguration(idpId.Value);
                if (fileData != null)
                {
                    postDefault = fileData.EnforcePOST;
                }
            }

            return postDefault;
        }

        private static string CreateMetadataString(bool defaultPost)
        {
            return MetadataModel.CreateIdpMetadata(defaultPost)
                            .ToXmlString(CertificateHelper.SigningCertificate,
                            SignedXml.XmlDsigRSASHA256Url);
        }

        public ActionResult BrowserFriendly(Guid? idpId)
        {
            var postDefault = ReadCustomIdpConfig(idpId);
            return Content(
                CreateMetadataString(postDefault),
                "text/xml");
        }
    }
}
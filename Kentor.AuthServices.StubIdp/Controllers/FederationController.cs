using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kentor.AuthServices.Metadata;
using System.Security.Cryptography.Xml;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class FederationController : BaseController
    {
        // GET: Federation
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
                    postDefault = fileData.EnforceXmlns;
                }
            }

            return postDefault;
        }

        private static string CreateMetadataString(bool defaultPost)
        {
            return MetadataModel.CreateFederationMetadata(defaultPost).ToXmlString(
                CertificateHelper.SigningCertificate,
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
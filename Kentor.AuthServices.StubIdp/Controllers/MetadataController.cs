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

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class MetadataController : Controller
    {
        public static readonly TimeSpan CacheDuration = new TimeSpan(1, 0, 0);

        // GET: Metadata
        public ActionResult Index()
        {
            return Content(
                Metadata.IdpMetadata.ToXmlString(CacheDuration), 
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                Metadata.IdpMetadata.ToXmlString(CacheDuration),
                "text/xml");
        }
    }
}
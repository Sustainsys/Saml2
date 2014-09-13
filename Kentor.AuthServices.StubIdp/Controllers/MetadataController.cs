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
        // GET: Metadata
        public ActionResult Index()
        {
            return Content(
                Metadata.IdpMetadata.ToXmlString(3600), 
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                Metadata.IdpMetadata.ToXmlString(3600),
                "text/xml");
        }
    }
}
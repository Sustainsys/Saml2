using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class FederationController : Controller
    {
        // GET: Federation
        public ActionResult Index()
        {
            return Content(
                Metadata.FederationMetadata.ToXmlString(3600),
                "application/samlmetadata+xml");
        }

        public ActionResult BrowserFriendly()
        {
            return Content(
                Metadata.FederationMetadata.ToXmlString(3600),
                "text/xml");
        }
    }
}
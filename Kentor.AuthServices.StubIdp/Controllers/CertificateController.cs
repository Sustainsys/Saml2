using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class CertificateController : Controller
    {
        public ActionResult Index()
        {
            var path = HttpContext.Server.MapPath("~\\App_Data\\Kentor.AuthServices.StubIdp.cer");
            var disposition = new ContentDisposition { Inline = false, FileName = Path.GetFileName(path) };
            Response.AppendHeader("Content-Disposition", disposition.ToString());
            return File(path, MediaTypeNames.Text.Plain);
        }
    }
}
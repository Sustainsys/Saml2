using System.IO;
using System.Net.Mime;
using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kentor.AuthServices.Mvc;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = AssertionModel.CreateFromConfiguration();
            var request = Saml2AuthenticationRequest.Read(Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(new HttpRequestData(Request)));
            if (request != null)
            {
                model.InResponseTo = request.Id;
                model.AssertionConsumerServiceUrl = request.AssertionConsumerServiceUrl.ToString();
            }

            return View(model);
        }

        public ActionResult Certificate()
        {
            var path = HttpContext.Server.MapPath("~\\App_Data\\Kentor.AuthServices.StubIdp.cer");
            var disposition = new ContentDisposition { Inline = false, FileName = Path.GetFileName(path) };
            Response.AppendHeader("Content-Disposition", disposition.ToString());
            return File(path, MediaTypeNames.Text.Plain);
        }

        [HttpPost]
        public ActionResult Index(AssertionModel model)
        {
            if (ModelState.IsValid)
            {
                var response = model.ToSaml2Response();

                var commandResult = Saml2Binding.Get(Saml2BindingType.HttpPost)
                    .Bind(response);

                return commandResult.ToActionResult();
            }

            return View(model);
        }
    }
}
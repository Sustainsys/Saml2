using System.IO;
using System.Net.Mime;
using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kentor.AuthServices.Mvc;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Tokens;
using System.Configuration;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = AssertionModel.CreateFromConfiguration();

            var decodedXmlData = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(new HttpRequestData(Request));

            var request = Saml2AuthenticationRequest.Read(decodedXmlData);

            if (request != null)
            {
                model.InResponseTo = request.Id;
                model.AssertionConsumerServiceUrl = request.AssertionConsumerServiceUrl.ToString();
                model.AuthnRequestXml = decodedXmlData;
            }

            return View(model);
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
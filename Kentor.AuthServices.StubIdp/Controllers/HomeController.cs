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
            var model = AssertionModel.Default;
            var request = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind<Saml2AuthenticationRequest>(Request);
            if (request != null)
            {
                model.InResponseTo = request.Id;
                model.AssertionConsumerUrl = request.AssertionConsumerServiceUrl.ToString();
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
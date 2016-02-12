using Kentor.AuthServices.HttpModule;
using Kentor.AuthServices.Mvc;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.StubIdp.Models;
using Kentor.AuthServices.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            var unbindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(Request.ToHttpRequestData(), null);

            var request = new Saml2LogoutRequest(unbindResult.Data);

            var model = new LogoutModel
            {
                LogoutRequestXml = unbindResult.Data.OuterXml
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LogoutModel model)
        {
            var response = model.ToLogoutResponse();

            return Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(response)
                .ToActionResult();
        }
    }
}
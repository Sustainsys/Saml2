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
            var requestData = Request.ToHttpRequestData();

            var binding = Saml2Binding.Get(requestData);

            var model = new LogoutModel();

            if(binding != null)
            {
                var unbindResult = binding.Unbind(requestData, null);
                model.LogoutRequestXml = unbindResult.Data.OuterXml;
                model.InResponseTo = unbindResult.Data.GetAttribute("ID");
            }
            else
            {
                model.SessionIndex = AssertionModel.DefaultSessionIndex;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LogoutModel model)
        {
            ISaml2Message message;

            if(model.SessionIndex != null)
            {
                message = model.ToLogoutRequest();
            }
            else
            {
                message = model.ToLogoutResponse();
            }

            return Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message)
                .ToActionResult();
        }
    }
}
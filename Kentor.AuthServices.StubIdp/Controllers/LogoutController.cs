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

            if(binding != null)
            {
                var unbindResult = binding.Unbind(requestData, null);
                switch(unbindResult.Data.LocalName)
                {
                    case "LogoutRequest":
                        {
                            var model = new RespondToLogoutRequestModel()
                            {
                                LogoutRequestXml = unbindResult.Data.PrettyPrint(),
                                InResponseTo = unbindResult.Data.GetAttribute("ID")
                            };
                            return View("RespondToLogout", model);
                        }
                    case "LogoutResponse":
                            return View("ReceivedLogoutResponse", model: unbindResult.Data.PrettyPrint());
                    default:
                        throw new InvalidOperationException();
                }
            }
            {
                var model = new InitiateLogoutModel()
                {
                    SessionIndex = AssertionModel.DefaultSessionIndex
                };

                return View("InitiateLogout", model);
            }
        }

        [HttpPost]
        public ActionResult InitiateLogout(InitiateLogoutModel model)
        {
            return Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(model.ToLogoutRequest())
                .ToActionResult();
        }

        [HttpPost]
        public ActionResult RespondToLogoutRequest(RespondToLogoutRequestModel model)
        {
            return Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(model.ToLogoutResponse())
                .ToActionResult();
        }   
    }
}
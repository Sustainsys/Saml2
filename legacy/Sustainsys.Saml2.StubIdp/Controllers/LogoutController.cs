using Sustainsys.Saml2.HttpModule;
using Sustainsys.Saml2.Mvc;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.StubIdp.Models;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sustainsys.Saml2.StubIdp.Controllers
{
    public class LogoutController : Controller
    {
        public ActionResult Index()
        {
            var requestData = Request.ToHttpRequestData(true);

            var binding = Saml2Binding.Get(requestData);

            if(binding != null)
            {
                var unbindResult = binding.Unbind(requestData, null);
                switch(unbindResult.Data.LocalName)
                {
                    case "LogoutRequest":
                        {
                            var logoutRequest = Saml2LogoutRequest.FromXml(unbindResult.Data);

                            var model = new RespondToLogoutRequestModel()
                            {
                                LogoutRequestXml = unbindResult.Data.PrettyPrint(),
                                InResponseTo = logoutRequest.Id.Value,
                                DestinationUrl = new Uri(new Uri(logoutRequest.Issuer.Id + "/"), "Logout"),
                                RelayState = Request.QueryString["RelayState"]
                            };
                            return View("RespondToLogout", model);
                        }
                    case "LogoutResponse":
                        {
                            var model = new ResponseModel
                            {
                                Status = unbindResult.Data["Status", Saml2Namespaces.Saml2PName]
                                ["StatusCode", Saml2Namespaces.Saml2PName].GetAttribute("Value"),
                                ResponseXml = unbindResult.Data.PrettyPrint()
                            };
                            return View("ReceivedLogoutResponse", model);
                        }
                    default:
                        throw new InvalidOperationException();
                }
            }
            {
                var model = new InitiateLogoutModel();

                if (Request.QueryString.AllKeys.Any())
                {
                    TryUpdateModel(model);
                }
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
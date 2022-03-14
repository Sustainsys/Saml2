using System.IO;
using System.Net.Mime;
using Sustainsys.Saml2.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sustainsys.Saml2.Mvc;
using System.IdentityModel.Metadata;
using Sustainsys.Saml2.Configuration;
using System.IdentityModel.Tokens;
using System.Configuration;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.WebSso;
using Sustainsys.Saml2.HttpModule;
using System.Xml;

namespace Sustainsys.Saml2.StubIdp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(Guid? idpId)
        {
            var model = new HomePageModel
            {
                AssertionModel = AssertionModel.CreateFromConfiguration(),
            };

            ReadCustomIdpConfig(idpId, model);

            HandleReceivedAuthnReqest(model);

            return View(model);
        }

        private void ReadCustomIdpConfig(Guid? idpId, HomePageModel model)
        {
            if (idpId.HasValue)
            {
                var fileData = GetCachedConfiguration(idpId.Value);
                if (fileData != null)
                {
                    if (!string.IsNullOrEmpty(fileData.DefaultAssertionConsumerServiceUrl))
                    {
                        // Override default StubIdp Acs with Acs from IdpConfiguration
                        model.AssertionModel.AssertionConsumerServiceUrl = fileData.DefaultAssertionConsumerServiceUrl;
                    }
                    if (!string.IsNullOrEmpty(fileData.DefaultAssertionConsumerServiceUrl))
                    {
                        model.AssertionModel.Audience = fileData.DefaultAudience;
                    }

                    model.CustomDescription = fileData.IdpDescription;
                    model.AssertionModel.NameId = null;
                    model.HideDetails = fileData.HideDetails;
                }
            }
        }

        private bool HandleReceivedAuthnReqest(HomePageModel model)
        {
            var requestData = Request.ToHttpRequestData(true);
            var binding = Saml2Binding.Get(requestData);
            if (binding != null)
            {
                var extractedMessage = binding.Unbind(requestData, null);

                var request = new Saml2AuthenticationRequest(
                    extractedMessage.Data,
                    extractedMessage.RelayState);

                model.AssertionModel.InResponseTo = request.Id.Value;
                if(request.AssertionConsumerServiceUrl != null)
                {
                    model.AssertionModel.AssertionConsumerServiceUrl = 
                        request.AssertionConsumerServiceUrl.ToString();
                }
                model.AssertionModel.RelayState = extractedMessage.RelayState;
                model.AssertionModel.Audience = request.Issuer.Id;
                model.AssertionModel.AuthnRequestXml = extractedMessage.Data.PrettyPrint();

                // Suppress error messages from the model - what we received
                // in the post isn't even a model.
                ModelState.Clear();

                return true;
            }
            return false;
        }

        [HttpPost]
        public ActionResult Index(Guid? idpId, HomePageModel model)
        {
            if (ModelState.IsValid)
            {
                var response = model.AssertionModel.ToSaml2Response();

                return Saml2Binding.Get(model.AssertionModel.ResponseBinding)
                    .Bind(response).ToActionResult();
            }

            if (model.AssertionModel == null)
            {
                model.AssertionModel = AssertionModel.CreateFromConfiguration();
            };

            if (HandleReceivedAuthnReqest(model))
            {
                ReadCustomIdpConfig(idpId, model);
            }

            return View(model);
        }
    }
}
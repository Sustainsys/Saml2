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
using Kentor.AuthServices.HttpModule;
using System.Xml;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(Guid? idpId)
        {
            var model = new HomePageModel
            {
                AssertionModel = AssertionModel.CreateFromConfiguration(),
            };

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
                    if(!string.IsNullOrEmpty(fileData.DefaultAssertionConsumerServiceUrl))
                    {
                        model.AssertionModel.Audience = fileData.DefaultAudience;
                    }

                    model.CustomDescription = fileData.IdpDescription;
                    model.AssertionModel.NameId = null;
                    model.HideDetails = fileData.HideDetails;
                }
            }

            var requestData = Request.ToHttpRequestData(true);
            if (requestData.QueryString["SAMLRequest"].Any())
            {
                var extractedMessage = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                    .Unbind(requestData, null);

                var request = new Saml2AuthenticationRequest(
                    extractedMessage.Data,
                    extractedMessage.RelayState);

                model.AssertionModel.InResponseTo = request.Id.Value;
                model.AssertionModel.AssertionConsumerServiceUrl = request.AssertionConsumerServiceUrl.ToString();
                model.AssertionModel.RelayState = extractedMessage.RelayState;
                model.AssertionModel.Audience = request.Issuer.Id;
                model.AssertionModel.AuthnRequestXml = extractedMessage.Data.PrettyPrint();
            }

            return View(model);
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

            return View(model);
        }
    }
}
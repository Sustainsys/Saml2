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
                    model.CustomDescription = fileData.IdpDescription;
                    model.AssertionModel.NameId = null;
                    model.HideDetails = fileData.HideDetails;
                }
            }

            var requestData = Request.ToHttpRequestData();
            if (requestData.QueryString["SAMLRequest"].Any())
            {
                var decodedXmlData = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                    .Unbind(requestData);

                var request = Saml2AuthenticationRequest.Read(decodedXmlData);

                model.AssertionModel.InResponseTo = request.Id;
                model.AssertionModel.AssertionConsumerServiceUrl = request.AssertionConsumerServiceUrl.ToString();
                model.AssertionModel.AuthnRequestXml = decodedXmlData;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Guid? idpId, HomePageModel model)
        {
            if (ModelState.IsValid)
            {
                var response = model.AssertionModel.ToSaml2Response();

                var commandResult = Saml2Binding.Get(Saml2BindingType.HttpPost)
                    .Bind(response);

                return commandResult.ToActionResult();
            }

            return View(model);
        }
    }
}
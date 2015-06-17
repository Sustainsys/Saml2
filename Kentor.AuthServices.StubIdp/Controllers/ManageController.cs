using System.IO;
using System.Net.Mime;
using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
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
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class ManageController : Controller
    {
        private static ConcurrentDictionary<Guid, string> cachedConfigurations = new ConcurrentDictionary<Guid, string>();

        private string GetIdpFileNamePath(Guid idpId)
        {
            return Server.MapPath(string.Format("~/App_Data/{0}.json", idpId));
        }

        public ActionResult Index(Guid idpId)
        {
            var fileName = GetIdpFileNamePath(idpId);
            var model = new ManageIdpModel();
            if (System.IO.File.Exists(fileName))
            {
                model.JsonData = System.IO.File.ReadAllText(fileName);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Guid idpId, ManageIdpModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var schema = JSchema.Parse(System.IO.File.ReadAllText(Server.MapPath("~/Content/IdpConfigurationSchema.json")));

            JObject parsedJson;
            try
            {
                parsedJson = JObject.Parse(model.JsonData);
            }
            catch (Exception)
            {
                ModelState.AddModelError("JsonData", "Invalid Json");
                return View(model);
            }
            IList<string> errorMessages;
            if (!parsedJson.IsValid(schema, out errorMessages))
            {
                ModelState.AddModelError("JsonData", "Json does not match schema. " + string.Join(" ", errorMessages));
            }

            var fileName = GetIdpFileNamePath(idpId);

            model.JsonData = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

            System.IO.File.WriteAllText(fileName, model.JsonData);

            cachedConfigurations.AddOrUpdate(idpId, model.JsonData, (_, __) => model.JsonData);

            return RedirectToAction("Index");
        }

        public ActionResult CurrentConfiguration(Guid idpId)
        {
            string newFileData = null;
            var fileData = cachedConfigurations.GetOrAdd(idpId, _ =>
            {
                var fileName = GetIdpFileNamePath(idpId);
                if (System.IO.File.Exists(fileName))
                {
                    newFileData = System.IO.File.ReadAllText(fileName);
                    return newFileData;
                }
                return null;
            });
            fileData = fileData ?? newFileData; // get new value if nothing was returned from GetOrAdd
            if (fileData == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal server error, no IDP configured");
            }
            return Content(fileData, "application/json");
        }
    }
}
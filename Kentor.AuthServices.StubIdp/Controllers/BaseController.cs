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
using System.Net;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class BaseController : Controller
    {
        protected static ConcurrentDictionary<Guid, IdpConfigurationModel> cachedConfigurations = new ConcurrentDictionary<Guid, IdpConfigurationModel>();

        protected string GetIdpFileNamePath(Guid idpId)
        {
            return Server.MapPath(string.Format("~/App_Data/{0}.json", idpId));
        }

        protected IdpConfigurationModel GetCachedConfiguration(Guid idpId)
        {
            IdpConfigurationModel newFileData = null;
            var fileData = cachedConfigurations.GetOrAdd(idpId, _ =>
            {
                var fileName = GetIdpFileNamePath(idpId);
                if (System.IO.File.Exists(fileName))
                {
                    newFileData = new IdpConfigurationModel(System.IO.File.ReadAllText(fileName));
                    return newFileData;
                }
                return null;
            });
            fileData = fileData ?? newFileData; // get new value if nothing was returned from GetOrAdd
            return fileData;
        }

        // Based on http://stackoverflow.com/a/17658754/401728
        protected ActionResult TestETag(string content, string responseETag, string contentType)
        {
            var requestedETag = Request.Headers["If-None-Match"];
            if (requestedETag == responseETag)
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);

            Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            Response.Cache.SetETag(responseETag);
            return Content(content, contentType);
        }
    }
}
using Kentor.AuthServices.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kentor.AuthServices.StubIdp.Controllers
{
    public class DiscoveryServiceController : Controller
    {
        public ActionResult Index(DiscoveryServiceModel model)
        {
            if(model.isPassive || Request.HttpMethod == "POST")
            {
                string delimiter = model.@return.Contains("?") ? "&" : "?";

                return Redirect(string.Format(
                    "{0}{1}{2}={3}",
                    model.@return,
                    delimiter,
                    model.returnIDParam,
                    model.SelectedIdp));
            }

            return View(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Sustainsys.Saml2.StubIdp
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/Content/css-bundle")
                .Include(
                "~/Content/css/select2.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/js.cookie.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/select2.js",
                "~/Scripts/ICanHaz.js",
                "~/Scripts/ViewIndex.js"));
        }
    }
}
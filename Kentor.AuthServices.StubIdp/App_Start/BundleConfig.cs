using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Kentor.AuthServices.StubIdp.App_Start;

namespace Kentor.AuthServices.StubIdp
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/Content/css")
                {
                    Transforms = { new LessTransform() }
                }
                .Include(
                "~/Content/normalize.css",
                "~/Content/site.less"));
        }
    }
}
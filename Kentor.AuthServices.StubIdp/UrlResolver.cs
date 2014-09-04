using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kentor.AuthServices.StubIdp
{
    public static class UrlResolver
    {
        public static Uri RootUrl
        {
            get
            {
                return new Uri(HttpContext.Current.Request.Url, HttpContext.Current.Request.ApplicationPath);
            }
        }

        public static Uri SsoServiceUrl
        {
            get
            {
                return RootUrl;
            }
        }

        public static Uri MetadataUrl
        {
            get
            {
                return new Uri(RootUrl, "Metadata");
            }
        }
    }
}
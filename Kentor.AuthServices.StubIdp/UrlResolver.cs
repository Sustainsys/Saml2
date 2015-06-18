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
                var namedIdpSegment = HttpContext.Current.Request.Url.Segments.Skip(1).FirstOrDefault();
                Guid parsedGuid;
                if (!string.IsNullOrEmpty(namedIdpSegment) && Guid.TryParse(namedIdpSegment.TrimEnd('/'), out parsedGuid))
                {
                    return new Uri(HttpContext.Current.Request.Url, "/" + namedIdpSegment.TrimEnd('/'));
                }
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
                var rootUrl = RootUrl;
                if (rootUrl.ToString().EndsWith("/")) {
                    return new Uri(RootUrl + "Metadata");
                }
                return new Uri(RootUrl + "/Metadata");
            }
        }
    }
}
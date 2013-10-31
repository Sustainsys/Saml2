using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace Kentor.AuthServices
{
    class CommandResult
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public HttpCacheability Cacheability { get; set; }
        public Uri Location { get; set; }
        public ClaimsPrincipal Principal { get; set; }

        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Cacheability = HttpCacheability.NoCache;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HttpStatusCode")]
        public void Apply(HttpResponseBase response)
        {
            response.Cache.SetCacheability(Cacheability);

            if (HttpStatusCode == HttpStatusCode.SeeOther || Location != null)
            {
                if (Location == null)
                {
                    throw new InvalidOperationException("Missing Location on redirect.");
                }
                if (HttpStatusCode != HttpStatusCode.SeeOther)
                {
                    throw new InvalidOperationException("Invalid HttpStatusCode for redirect, but Location is specified");
                }

                response.Redirect(Location.ToString());
            }

            response.StatusCode = (int)HttpStatusCode;
        }
    }
}

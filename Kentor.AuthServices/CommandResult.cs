using System;
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
        public CommandResultErrorCode ErrorCode { get; set; }
        public ClaimsPrincipal Principal { get; set; }

        public CommandResult()
        {
            HttpStatusCode = HttpStatusCode.OK;
            Cacheability = HttpCacheability.NoCache;
        }

        public void Apply(HttpResponseBase response)
        {
            response.Cache.SetCacheability(Cacheability);

            if (HttpStatusCode == HttpStatusCode.SeeOther)
            {
                response.Redirect(Location.ToString());
            }

            response.StatusCode = (int)HttpStatusCode;
        }
    }
}

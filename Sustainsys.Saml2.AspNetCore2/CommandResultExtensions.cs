using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Http;

namespace Sustainsys.Saml2.AspNetCore2
{
    static class CommandResultExtensions
    {
        public static void Apply(
            this CommandResult commandResult,
            HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int)commandResult.HttpStatusCode;

            if(commandResult.Location != null)
            {
                httpContext.Response.Headers["Location"] = commandResult.Location.ToString();
            }
        }
    }
}

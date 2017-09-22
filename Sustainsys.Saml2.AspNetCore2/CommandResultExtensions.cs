using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Sustainsys.Saml2.AspNetCore2
{
    static class CommandResultExtensions
    {
        public static void Apply(
            this CommandResult commandResult,
            HttpContext httpContext,
            IDataProtector dataProtector)
        {
            httpContext.Response.StatusCode = (int)commandResult.HttpStatusCode;

            if(commandResult.Location != null)
            {
                httpContext.Response.Headers["Location"] = commandResult.Location.ToString();
            }

            if(!string.IsNullOrEmpty(commandResult.SetCookieName))
            {
                var cookieData = HttpRequestData.ConvertBinaryData(
                    dataProtector.Protect(commandResult.GetSerializedRequestState()));

                httpContext.Response.Cookies.Append(
                    commandResult.SetCookieName,
                    cookieData,
                    new CookieOptions() { HttpOnly = true } );
            }

            if(!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                httpContext.Response.Cookies.Append(
                    commandResult.ClearCookieName, null, new CookieOptions()
                    {
                        Expires = new DateTime(0, DateTimeKind.Utc)
                    });
            }

            if(!string.IsNullOrEmpty(commandResult.Content))
            {
                var buffer = Encoding.UTF8.GetBytes(commandResult.Content);
                httpContext.Response.ContentType = commandResult.ContentType;
                httpContext.Response.Body.Write(buffer, 0, buffer.Length);
            }
        }
    }
}

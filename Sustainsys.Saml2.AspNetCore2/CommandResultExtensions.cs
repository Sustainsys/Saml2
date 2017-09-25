using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.Authentication;
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
            IDataProtector dataProtector,
            string SignInScheme)
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
                httpContext.Response.Cookies.Delete(commandResult.ClearCookieName);
            }

            if(!string.IsNullOrEmpty(commandResult.Content))
            {
                var buffer = Encoding.UTF8.GetBytes(commandResult.Content);
                httpContext.Response.ContentType = commandResult.ContentType;
                httpContext.Response.Body.Write(buffer, 0, buffer.Length);
            }

            if(commandResult.Principal != null)
            {
                var authProps = new AuthenticationProperties(commandResult.RelayData)
                {
                    RedirectUri = commandResult.Location.OriginalString
                };
                httpContext.SignInAsync(SignInScheme, commandResult.Principal, authProps);
            }
        }
    }
}

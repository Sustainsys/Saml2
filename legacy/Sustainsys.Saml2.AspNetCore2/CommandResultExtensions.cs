using Sustainsys.Saml2.WebSso;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2
{
    static class CommandResultExtensions
    {
        public static async Task Apply(
            this CommandResult commandResult,
            HttpContext httpContext,
            IDataProtector dataProtector,
            ICookieManager cookieManager,
            string signInScheme,
            string signOutScheme,
            bool emitSameSiteNone)
        {
            httpContext.Response.StatusCode = (int)commandResult.HttpStatusCode;

            if(commandResult.Location != null)
            {
                httpContext.Response.Headers["Location"] = commandResult.Location.OriginalString;
            }

            if(!string.IsNullOrEmpty(commandResult.SetCookieName))
            {
                var cookieData = HttpRequestData.ConvertBinaryData(
                    dataProtector.Protect(commandResult.GetSerializedRequestState()));

                cookieManager.AppendResponseCookie(
                    httpContext,
                    commandResult.SetCookieName,
                    cookieData,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = commandResult.SetCookieSecureFlag,
                        // We are expecting a different site to POST back to us,
                        // so the ASP.Net Core default of Lax is not appropriate in this case
                        SameSite = emitSameSiteNone ? SameSiteMode.None : (SameSiteMode)(-1),
                        IsEssential = true
                    });
            }

            foreach(var h in commandResult.Headers)
            {
                httpContext.Response.Headers.Add(h.Key, h.Value);
            }

            if(!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                cookieManager.DeleteCookie(
                    httpContext,
                    commandResult.ClearCookieName,
                    new CookieOptions
                    {
                        Secure = commandResult.SetCookieSecureFlag
                    });
            }

            if(!string.IsNullOrEmpty(commandResult.Content))
            {
                var buffer = Encoding.UTF8.GetBytes(commandResult.Content);
                httpContext.Response.ContentType = commandResult.ContentType;
                await httpContext.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            }

            if(commandResult.Principal != null)
            {
                var authProps = new AuthenticationProperties(commandResult.RelayData)
                {
                    RedirectUri = commandResult.Location.OriginalString
                };
                await httpContext.SignInAsync(signInScheme, commandResult.Principal, authProps);
            }

            if(commandResult.TerminateLocalSession)
            {
                await httpContext.SignOutAsync(signOutScheme ?? signInScheme);
            }
        }
    }
}

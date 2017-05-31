using Kentor.AuthServices.WebSso;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Kentor.AuthServices.AspNetCore
{
    static class CommandResultExtensions
    {
        public static void Apply(this CommandResult commandResult,
            HttpContext context,
            IDataProtector dataProtector)
        {
            if(commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.OnStarting(async (state) =>
            {
                context.Response.ContentType = commandResult.ContentType;
                context.Response.StatusCode = (int)commandResult.HttpStatusCode;

                if(commandResult.Location != null)
                {
                    context.Response.Headers["Location"] = commandResult.Location.OriginalString;
                }

                if(commandResult.TerminateLocalSession)
                {
                    //await context.Authentication.SignOutAsync(string.Empty); // TODO: correct scheme
                }

                ApplyCookies(commandResult, context, dataProtector);

                // Write the content last, it causes the headers to be flushed
                // on some hosts.
                if(commandResult.Content != null)
                {
                    // Remove value set by other middleware and let the host calculate
                    // a new value. See issue #295 for discussion on this.
                    context.Response.ContentLength = null;
                    await context.Response.WriteAsync(commandResult.Content);
                }
            }, null);
        }

        private static void ApplyCookies(CommandResult commandResult, HttpContext context, IDataProtector dataProtector)
        {
            var serializedCookieData = commandResult.GetSerializedRequestState();

            if(serializedCookieData != null)
            {
                var protectedData = HttpRequestData.ConvertBinaryData(
                        dataProtector.Protect(serializedCookieData));

                context.Response.Cookies.Append(
                    commandResult.SetCookieName,
                    protectedData,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                    });
            }

            commandResult.ApplyClearCookie(context);
        }

        public static void ApplyClearCookie(this CommandResult commandResult, HttpContext context)
        {
            if(!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                context.Response.Cookies.Delete(
                    commandResult.ClearCookieName,
                    new CookieOptions
                    {
                        HttpOnly = true
                    });
            }
        }
    }
}

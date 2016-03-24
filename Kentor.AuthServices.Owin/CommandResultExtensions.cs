using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Owin
{
    static class CommandResultExtensions
    {
        public static void Apply(this CommandResult commandResult,
            IOwinContext context,
            IDataProtector dataProtector)
        {
            if (commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.ContentType = commandResult.ContentType;
            context.Response.StatusCode = (int)commandResult.HttpStatusCode;

            if (commandResult.Location != null)
            {
                context.Response.Headers["Location"] = commandResult.Location.OriginalString;
            }

            if (commandResult.Content != null)
            {
                // Remove value set by other middleware and let the host calculate
                // a new value. See issue #295 for discussion on this.
                context.Response.ContentLength = null;
                context.Response.Write(commandResult.Content);
            }

            if (commandResult.TerminateLocalSession)
            {
                context.Authentication.SignOut();
            }

            ApplyCookies(commandResult, context, dataProtector);
        }

        private static void ApplyCookies(CommandResult commandResult, IOwinContext context, IDataProtector dataProtector)
        {
            if (!string.IsNullOrEmpty(commandResult.SetCookieData))
            {
                var protectedData = HttpRequestData.EscapeBase64CookieValue(
                    Convert.ToBase64String(
                        dataProtector.Protect(
                            Encoding.UTF8.GetBytes(
                                commandResult.SetCookieData))));

                context.Response.Cookies.Append(
                    commandResult.SetCookieName,
                    protectedData,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                    });
            }

            if(!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                context.Response.Cookies.Delete(commandResult.ClearCookieName);
            }
        }
    }
}

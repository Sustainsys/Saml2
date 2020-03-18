using Sustainsys.Saml2.WebSso;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sustainsys.Saml2.Owin
{
    static class CommandResultExtensions
    {
        public static void Apply(this CommandResult commandResult,
            IOwinContext context,
            IDataProtector dataProtector,
            ICookieManager cookieManager,
            bool emitSameSiteNone)
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

            if (commandResult.TerminateLocalSession)
            {
                context.Authentication.SignOut();
            }

            ApplyCookies(commandResult, context, dataProtector, cookieManager, emitSameSiteNone);
            
            foreach(var h in commandResult.Headers)
            {
                context.Response.Headers[h.Key] = h.Value;
            }


            // Write the content last, it causes the headers to be flushed
            // on some hosts.
            if (commandResult.Content != null)
            {
                // Remove value set by other middleware and let the host calculate
                // a new value. See issue #295 for discussion on this.
                context.Response.ContentLength = null;
                context.Response.Write(commandResult.Content);
            }
        }

        private static void ApplyCookies(
            CommandResult commandResult,
            IOwinContext context,
            IDataProtector dataProtector,
            ICookieManager cookieManager,
            bool emitSameSiteNone)
        {
            var serializedCookieData = commandResult.GetSerializedRequestState();

            if (serializedCookieData != null && !string.IsNullOrEmpty(commandResult.SetCookieName))
            {
                var protectedData = HttpRequestData.ConvertBinaryData(
                        dataProtector.Protect(serializedCookieData));

                cookieManager.AppendResponseCookie(context,
                    commandResult.SetCookieName,
                    protectedData,
                    new CookieOptions()
                    {
                        HttpOnly = true,
                        Secure = commandResult.SetCookieSecureFlag,
                        SameSite = emitSameSiteNone ? SameSiteMode.None : default(SameSiteMode?)
                    });
            }

            commandResult.ApplyClearCookie(context, cookieManager);
        }

        public static void ApplyClearCookie(this CommandResult commandResult, IOwinContext context, ICookieManager cookieManager)
        {
            if (!string.IsNullOrEmpty(commandResult.ClearCookieName))
            {
                cookieManager.DeleteCookie(context,
                    commandResult.ClearCookieName,
                    new CookieOptions
                    {
                        Secure = commandResult.SetCookieSecureFlag
                    });
            }
        }
    }
}

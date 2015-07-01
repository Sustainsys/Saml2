using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices.Owin
{
    static class CommandResultExtensions
    {
        public static void Apply(this CommandResult commandResult, IOwinContext context)
        {
            if (commandResult == null)
            {
                throw new ArgumentNullException("commandResult");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.ContentType = commandResult.ContentType;
            context.Response.StatusCode = (int)commandResult.HttpStatusCode;

            if (commandResult.Location != null)
            {
                context.Response.Headers["Location"] = commandResult.Location.OriginalString;
            }

            if (commandResult.Content != null)
            {
                var bytes = ASCIIEncoding.UTF8.GetBytes(commandResult.Content);
                context.Response.ContentLength = bytes.Length;
                context.Response.Write(commandResult.Content);
            }
        }

    }
}

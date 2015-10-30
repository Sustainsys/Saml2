using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
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
        }
    }
}

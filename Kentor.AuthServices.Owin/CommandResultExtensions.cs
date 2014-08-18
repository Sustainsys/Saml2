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
                throw new ArgumentNullException("commandResult");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.ContentType = commandResult.ContentType;

            using(var writer = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
            {
                writer.Write(commandResult.Content);
            }
        }
    }
}

using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.TestHelpers
{
    public class OwinTestHelpers
    {
        public static OwinContext CreateOwinContext()
        {
            var context = new OwinContext();
            Action<Action<object>, object> onSendingHeaders = (Action, obj) => { };
            context.Environment["server.OnSendingHeaders"] = onSendingHeaders;
            context.Request.Scheme  = "http";
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Path = new PathString("/");
            context.Request.PathBase = new PathString();
            context.Response.Body = new MemoryStream();
            return context;
        }
    }
}

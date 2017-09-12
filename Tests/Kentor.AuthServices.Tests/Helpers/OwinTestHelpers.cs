using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.Helpers
{
    public class OwinTestHelpers
    {
        public static OwinContext CreateOwinContext()
        {
            var context = new ValidatingOwinContext();
            Action<Action<object>, object> onSendingHeaders = (Action, obj) => { };
            context.Environment["server.OnSendingHeaders"] = onSendingHeaders;
            context.Request.Scheme  = "http";
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Path = new PathString("/");
            context.Request.PathBase = new PathString();
            context.Response.Body = new MemoryStream();
            context.Request.Method = "GET";
            return context;
        }

        class ValidatingOwinContext : OwinContext
        {
            public ValidatingOwinContext()
            {
                typeof(OwinContext)
                    .GetProperty(nameof(Response))
                    .SetValue(this, new ValidatingOwinResponse(Environment));
            }
        }

        class ValidatingOwinResponse : OwinResponse
        {
            public ValidatingOwinResponse(IDictionary<string, object> environment)
                : base(environment)
            {

            }

            public override long? ContentLength
            {
                get
                {
                    return base.ContentLength;
                }

                set
                {
                    if(Body.Length != 0)
                    {
                        throw new InvalidOperationException("Don't reset ContentLength after data has been written.");
                    }

                    base.ContentLength = value;
                }
            }
        }
    }
}

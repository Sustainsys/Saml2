using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Owin
{
    static class OwinRequestExtensions
    {
        public static string GetUserAgent(this IOwinRequest request)
        {
            return request.Headers["User-Agent"] ?? "";
        }
    }
}

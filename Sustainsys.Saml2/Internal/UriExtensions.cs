using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Internal
{
    static class UriExtensions
    {
        public static bool IsHttps(this Uri uri)
        {
            return string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
        }
    }
}

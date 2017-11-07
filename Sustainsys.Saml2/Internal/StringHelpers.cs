using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    static class StringHelpers
    {
        public static string NullIfEmpty(this string source)
        {
            if(string.IsNullOrEmpty(source))
            {
                return null;
            }

            return source;
        }
    }
}

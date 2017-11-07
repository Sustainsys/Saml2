using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Internal
{
    static class DictionaryExtensions
    {
        public static string GetValueOrEmpty<T>(this IDictionary<T, string> dictionary, T key)
        {
            string value;
            if(dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return "";
        }
    }
}

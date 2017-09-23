using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    class StubCookieCollection : IRequestCookieCollection
    {
        IDictionary<string, string> cookies;

        public StubCookieCollection(IEnumerable<KeyValuePair<string, string>> cookies)
        {
            this.cookies = cookies.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public string this[string key] => cookies[key];

        public int Count => cookies.Count;

        public ICollection<string> Keys => cookies.Keys;

        public bool ContainsKey(string key) => cookies.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => cookies.GetEnumerator();

        public bool TryGetValue(string key, out string value)
            => cookies.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => cookies.GetEnumerator();
    }
}

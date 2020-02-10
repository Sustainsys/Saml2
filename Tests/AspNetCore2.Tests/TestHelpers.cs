using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Sustainsys.Saml2.AspNetCore2.Tests
{
    static class TestHelpers
    {
        class FormValues : IFormCollection
        {
            public StringValues this[string key] => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public ICollection<string> Keys => throw new NotImplementedException();

            public IFormFileCollection Files => throw new NotImplementedException();

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                return new[]
                {
                    new KeyValuePair<string, StringValues>("Input1", new StringValues("Value1")),
                    new KeyValuePair<string, StringValues>("Input2", new StringValues("Value2"))
                }.AsEnumerable().GetEnumerator();
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
        public static HttpContext CreateHttpContext()
        {
            var context = Substitute.For<HttpContext>();

            context.Request.HasFormContentType.Returns(true);
            context.Request.Form.Returns(new FormValues());
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Scheme = "https";
            context.Request.Path = "/somePath";
            context.Request.PathBase = "";
            context.Request.QueryString = new QueryString("?param=value");

            context.Response.Body.Returns(new MemoryStream());

            return context;
        }

        public static HttpContext CreateHttpGet()
        {
            var context = Substitute.For<HttpContext>();

            context.Request.Method = "GET";
            context.Request.Form.Returns(x => { throw new InvalidOperationException(); });
            context.Request.Host = new HostString("sp.example.com");
            context.Request.Scheme = "https";
            context.Request.Path = "/somePath";
            context.Request.PathBase = "";
            context.Request.QueryString = new QueryString("?param=value");

            context.Response.Body.Returns(new MemoryStream());

            return context;
        }

        public const string defaultSignInScheme = nameof(defaultSignInScheme);

        public const string defaultSignOutScheme = nameof(defaultSignOutScheme);

        public static IOptions<AuthenticationOptions> GetAuthenticationOptions()
        {
            var authOptions = Substitute.For<AuthenticationOptions>();
            authOptions.DefaultSignInScheme = defaultSignInScheme;
            authOptions.DefaultSignOutScheme = defaultSignOutScheme;

            var iOptions = Substitute.For<IOptions<AuthenticationOptions>>();

            iOptions.Value.Returns(authOptions);

            return iOptions;
        }
    }
}

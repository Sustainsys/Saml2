using FluentAssertions;
using Sustainsys.Saml2.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sustainsys.Saml2.Tests.WebSSO
{
    [TestClass]
    public class HttpRequestDataTests
    {
        [TestMethod]
        public void HttpRequestData_Ctor_FromParamsCalculatesApplicationUrl()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?name=DROP%20TABLE%20STUDENTS");
            string appPath = "/ApplicationPath";

            var subject = new HttpRequestData(
                 "GET",
                 url,
                 appPath,
                 new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("Key", new string[] { "Value" })
                },
                null,
                null);

            subject.ApplicationUrl.Should().Be(new Uri("http://example.com:42/ApplicationPath"));
        }

        [TestMethod]
        public void HttpRequestData_EscapeBase64CookieValue_Nullcheck()
        {
            Action a = () => HttpRequestData.ConvertBinaryData(null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("data");
        }

        [TestMethod]
        public void HttpRequestData_Ctor_RelayStateButNoCookie()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=Foo");
            string appPath = "/ApplicationPath";

            Action a = () => new HttpRequestData(
                 "GET",
                 url,
                 appPath,
                 new KeyValuePair<string, IEnumerable<string>>[]
                 {
                    new KeyValuePair<string, IEnumerable<string>>("Key", new string[] { "Value" })
                 },
                 Enumerable.Empty<KeyValuePair<string, string>>(),
                 null);

            a.Should().NotThrow();
        }
    }
}


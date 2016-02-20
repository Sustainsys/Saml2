using FluentAssertions;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Tests.WebSSO
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
                 new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                },
                null,
                null);

            subject.ApplicationUrl.Should().Be(new Uri("http://example.com:42/ApplicationPath"));
        }

        [TestMethod]
        public void HttpRequestData_EscapeBase64CookieValue_Nullcheck()
        {
            Action a = () => HttpRequestData.EscapeBase64CookieValue(null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("value");
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
                 new KeyValuePair<string, string[]>[]
                 {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                 },
                 Enumerable.Empty<KeyValuePair<string, string>>(),
                 null);

            a.ShouldNotThrow();
        }
    }
}

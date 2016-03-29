using FluentAssertions;
using Kentor.AuthServices.Tests.Owin;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
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
            Action a = () => HttpRequestData.ConvertBinaryData(null);

            a.ShouldThrow<ArgumentNullException>()
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
                 new KeyValuePair<string, string[]>[]
                 {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                 },
                 Enumerable.Empty<KeyValuePair<string, string>>(),
                 null);

            a.ShouldNotThrow();
        }

        [TestMethod]
        public void HttpRequestData_Ctor_Deserialize_StoredRequestState()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=Foo");
            string appPath = "/ApplicationPath";

            var storedRequestData = new StoredRequestState(
                    new EntityId("http://idp.example.com"),
                    new Uri("http://sp.example.com/loggedout"),
                    new Saml2Id("id123"),
                    null);

            var cookies = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(
                    "Kentor.Foo",
                    HttpRequestData.ConvertBinaryData(
                            StubDataProtector.Protect(storedRequestData.Serialize())))
            };

            var subject = new HttpRequestData(
                 "GET",
                 url,
                 appPath,
                 Enumerable.Empty<KeyValuePair<string, string[]>>(),
                 cookies,
                 StubDataProtector.Unprotect);

            subject.StoredRequestState.ShouldBeEquivalentTo(storedRequestData);
        }
    }
}


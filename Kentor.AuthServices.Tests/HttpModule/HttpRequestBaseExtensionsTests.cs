using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Kentor.AuthServices.HttpModule;
using FluentAssertions;
using NSubstitute;
using System.Collections.Specialized;
using Kentor.AuthServices.WebSso;
using System.Web.Security;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Kentor.AuthServices.Tests.HttpModule
{
    [TestClass]
    public class HttpRequestBaseExtensionsTests
    {
        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData_ShouldThrowOnNull()
        {
            HttpRequestBase request = null;
            Action a = () => request.ToHttpRequestData();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestBase");
        }

        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=SomeState");
            string appPath = "/ApplicationPath";

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);

            var cookieValue = HttpRequestData.EscapeBase64CookieValue(Convert.ToBase64String(
                MachineKey.Protect(Encoding.UTF8.GetBytes("CookieValue"), "Kentor.AuthServices")));
            request.Cookies.Returns(new HttpCookieCollection());
            request.Cookies.Add(new HttpCookie("Kentor.SomeState", cookieValue));

            var subject = request.ToHttpRequestData();

            var expected = new HttpRequestData(
                "GET",
                url,
                appPath,
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            subject.ShouldBeEquivalentTo(expected, opt => opt.Excluding(s => s.CookieData));
            subject.CookieData.Should().Be("CookieValue");
        }

        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData_IgnoreCookieFlag()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=SomeState");
            string appPath = "/ApplicationPath";

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);

            var cookieValue = "SomethingThatCannotBeDecrypted";
            request.Cookies.Returns(new HttpCookieCollection());
            request.Cookies.Add(new HttpCookie("Kentor.SomeState", cookieValue));

            var subject = request.ToHttpRequestData(true);

            var expected = new HttpRequestData(
                "GET",
                url,
                appPath,
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            subject.ShouldBeEquivalentTo(expected);
        }
    }
}

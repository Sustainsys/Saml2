using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using Kentor.AuthServices.WebSSO;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class HttpRequestDataTests
    {
        [TestMethod]
        public void HttpRequestData_Ctor_Nullcheck()
        {
            Action a = () => new HttpRequestData(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void HttpRequestData_Ctor_FromHttpRequest()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?name=DROP%20TABLE%20STUDENTS");
            string appPath = "/ApplicationPath";

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);

            var subject = new HttpRequestData(request);

            var expected = new HttpRequestData(
                "GET",
                url,
                appPath,
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                });

            subject.ShouldBeEquivalentTo(expected);
        }

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
                });

            subject.ApplicationUrl.Should().Be(new Uri("http://example.com:42/ApplicationPath"));
        }
    }
}

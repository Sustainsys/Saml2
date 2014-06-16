using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;

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
        public void HttpRequestData_Ctor()
        {
            var url = new Uri("http://example.com/someurl?name=DROP%20TABLE%20STUDENTS");

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });

            var subject = new HttpRequestData(request);

            var expected = new HttpRequestData(
                "GET", url, new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("Key", new string[] { "Value" })
                });

            subject.ShouldBeEquivalentTo(expected);
        }
    }
}

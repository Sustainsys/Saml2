using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Kentor.AuthServices.HttpModule;
using FluentAssertions;

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
    }
}

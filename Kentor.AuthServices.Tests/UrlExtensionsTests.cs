using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class UrlExtensionsTests
    {
        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_NullBaseUrl_ShouldReturns_Null()
        {
            var actual = UrlExtensions.AppendReturnUrl(null, "http://foo.com");
            
            actual.Should().Be(null);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_NullReturnUrl_ShouldReturns_BaseUrl()
        {
            var expected = new Uri("https://test.com");
            var actual = expected.AppendReturnUrl(null);
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_EmptyReturnUrl_ShouldReturns_BaseUrl()
        {
            var expected = new Uri("https://test.com");
            var actual = expected.AppendReturnUrl(string.Empty);
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_InvalidReturnUrl_ShouldReturns_BaseUrl()
        {
            var expected = new Uri("https://test.com");
            var url = new Uri("https://test.com");
            var actual = url.AppendReturnUrl("https:// foo.com/news");
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_NotEncodedReturnUrl_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com?returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com");
            var actual = url.AppendReturnUrl("https://foo.com/news");
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_NotEncodedReturnUrl_WithPort_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com:443?returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com:443");
            var actual = url.AppendReturnUrl("https://foo.com/news");
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_AppendReturnUrl_NotEncodedReturnUrl_AddToQuery_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com?q1=test&returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com?q1=test");
            var actual = url.AppendReturnUrl("https://foo.com/news");
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_GetReturnUrlWithResourcePath_EncodedReturnUrl_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com?returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com");
            var actual = url.AppendReturnUrl(Uri.EscapeUriString("https://foo.com/news"));
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ReturnUrlExtensions_GetReturnUrlWithResourcePath_EncodedReturnUrl_WithPort_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com:443?returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com:443");
            var actual = url.AppendReturnUrl(Uri.EscapeUriString("https://foo.com/news"));
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void UrlExtensions_GetReturnUrlWithResourcePath_EncodedReturnUrl_AddToQuery_ShouldReturns_ModifedBaseUrl()
        {
            var expected = new Uri("https://test.com?q1=test&returnUrl=https://foo.com/news");
            var url = new Uri("https://test.com?q1=test");
            var actual = url.AppendReturnUrl(Uri.EscapeUriString("https://foo.com/news "));
            actual.Should().Be(expected);
        }
    }
}
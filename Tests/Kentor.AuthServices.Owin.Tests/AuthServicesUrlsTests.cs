using FluentAssertions;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Owin.Tests
{
    [TestClass]
    public class AuthServicesUrlsTests
    {
        [TestMethod]
        public async Task AuthServicesUrls_Ctor_FromOwinHttpRequestData_PublicOrigin()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin:8443/"));
            var subject = await ctx.ToHttpRequestData(null);
            var urls = new AuthServicesUrls(subject, options);
            urls.AssertionConsumerServiceUrl.ShouldBeEquivalentTo("https://my.public.origin:8443/AuthServices/Acs");
            urls.SignInUrl.ShouldBeEquivalentTo("https://my.public.origin:8443/AuthServices/SignIn");
        }
    }
}

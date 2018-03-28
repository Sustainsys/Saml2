using FluentAssertions;
using Sustainsys.Saml2.TestHelpers;
using Sustainsys.Saml2.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Owin.Tests
{
    [TestClass]
    public class Saml2UrlsTests
    {
        [TestMethod]
        public async Task Saml2Urls_Ctor_FromOwinHttpRequestData_PublicOrigin()
        {
            var ctx = OwinTestHelpers.CreateOwinContext();
            var options = StubFactory.CreateOptionsPublicOrigin(new Uri("https://my.public.origin:8443/"));
            var subject = await ctx.ToHttpRequestData(null);
            var urls = new Saml2Urls(subject, options);
            urls.AssertionConsumerServiceUrl.Should().BeEquivalentTo(
				new Uri("https://my.public.origin:8443/Saml2/Acs"));
            urls.SignInUrl.Should().BeEquivalentTo(
				new Uri("https://my.public.origin:8443/Saml2/SignIn"));
        }
    }
}

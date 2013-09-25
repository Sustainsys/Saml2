using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using System.Linq;
using NSubstitute;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class SignInCommandTests
    {
        [TestMethod]
        public void SignInCommand_Run_ReturnsAuthnRequestForDefaultIdp()
        {
            var defaultDestination = IdentityProvider.ConfiguredIdentityProviders.First()
                .Value.DestinationUri;

            var subject = new SignInCommand().Run(Substitute.For<HttpRequestBase>());

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.Found,
                Cacheability = HttpCacheability.NoCache,
                Location = new Uri(defaultDestination + "?SAMLRequest=XYZ")
            };

            subject.ShouldBeEquivalentTo(expected, options => options.Excluding(cr => cr.Location));
            subject.Location.Host.Should().Be(defaultDestination.Host);

            var queries = HttpUtility.ParseQueryString(subject.Location.Query);

            queries.Should().HaveCount(1);
            queries.Keys[0].Should().Be("SAMLRequest");
            queries[0].Should().NotBeEmpty();
        }
    }
}

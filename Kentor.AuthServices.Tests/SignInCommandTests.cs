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
                HttpStatusCode = HttpStatusCode.SeeOther,
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

        [TestMethod]
        public void SignInCommand_Run_With_Issuer2_ReturnsAuthnRequestForSecondIdp()
        {
            var secondIdp = IdentityProvider.ConfiguredIdentityProviders.Skip(1).First().Value;
            var secondDestination = secondIdp.DestinationUri;
            var secondIssuer = secondIdp.Issuer;

            var requestSubstitute = Substitute.For<HttpRequestBase>();
            requestSubstitute["issuer"].Returns(HttpUtility.UrlEncode(secondIssuer));
            var subject = new SignInCommand().Run(requestSubstitute);

            subject.Location.Host.Should().Be(secondDestination.Host);
        }

        [TestMethod]
        public void SignInCommand_Run_With_InvalidIssuer_ThrowsException()
        {
            var requestSubstitute = Substitute.For<HttpRequestBase>();
            requestSubstitute["issuer"].Returns(HttpUtility.UrlEncode("no-such-idp-in-config"));
            Action a = () => new SignInCommand().Run(requestSubstitute);

            a.ShouldThrow<InvalidOperationException>().WithMessage("Unknown issuer");
        }
    }
}

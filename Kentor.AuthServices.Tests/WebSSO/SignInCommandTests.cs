using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using System.Linq;
using NSubstitute;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;
using Kentor.AuthServices.TestHelpers;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class SignInCommandTests
    {
        [TestMethod]
        public void SignInCommand_Run_ReturnsAuthnRequestForDefaultIdp()
        {
            var defaultDestination = Options.FromConfiguration.IdentityProviders.Default.SingleSignOnServiceUrl;

            var subject = new SignInCommand().Run(
                new HttpRequestData("GET", new Uri("http://example.com")),
                Options.FromConfiguration);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Cacheability = (Cacheability) HttpCacheability.NoCache,
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
        public void SignInCommand_Run_MapsReturnUrl()
        {
            var defaultDestination = Options.FromConfiguration.IdentityProviders.Default.SingleSignOnServiceUrl;

            var httpRequest = new HttpRequestData("GET", new Uri("http://localhost/signin?ReturnUrl=%2FReturn.aspx"));

            var subject = new SignInCommand().Run(httpRequest, Options.FromConfiguration);

            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var authnRequest = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var requestId = AuthnRequestHelper.GetRequestId(subject.Location);

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new System.IdentityModel.Tokens.Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUrl.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void SignInCommand_Run_With_Idp2_ReturnsAuthnRequestForSecondIdp()
        {
            var secondIdp = Options.FromConfiguration.IdentityProviders[1];
            var secondDestination = secondIdp.SingleSignOnServiceUrl;
            var secondEntityId = secondIdp.EntityId;

            var request = new HttpRequestData("GET",
                new Uri("http://sp.example.com?idp=" + Uri.EscapeDataString(secondEntityId.Id)));

            var subject = new SignInCommand().Run(request, Options.FromConfiguration);

            subject.Location.Host.Should().Be(secondDestination.Host);
        }

        [TestMethod]
        public void SignInCommand_Run_With_InvalidIdp_ThrowsException()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost/signin?idp=no-such-idp-in-config"));

            Action a = () => new SignInCommand().Run(request, Options.FromConfiguration);

            a.ShouldThrow<InvalidOperationException>().WithMessage("Unknown idp");
        }

        [TestMethod]
        public void SignInCommand_Run_NullCheckRequest()
        {
            Action a = () => new SignInCommand().Run(null, Options.FromConfiguration);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void SignInCommand_Run_NullCheckOptions()
        {
            Action a = () => new SignInCommand().Run(new HttpRequestData("GET", new Uri("http://localhost")), null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void SignInCommand_Run_ReturnsRedirectToDiscoveryService()
        {
            var dsUrl = new Uri("http://ds.example.com");

            var options = new Options(new SPOptions
                {
                    DiscoveryServiceUrl = dsUrl,
                    EntityId = new EntityId("https://github.com/KentorIT/authservices")
                });

            var request = new HttpRequestData("GET", new Uri("http://localhost/signin?ReturnUrl=%2FReturn%2FPath"));

            var result = new SignInCommand().Run(request, options);

            result.HttpStatusCode.Should().Be(HttpStatusCode.SeeOther);

            var queryString = string.Format("?entityID={0}&return={1}&returnIDParam=idp",
                Uri.EscapeDataString(options.SPOptions.EntityId.Id),
                Uri.EscapeDataString(
                    "http://localhost/AuthServices/SignIn?ReturnUrl="
                    + Uri.EscapeDataString("/Return/Path")));

            var expectedLocation = new Uri(dsUrl + queryString);

            result.Location.Should().Be(expectedLocation);
        }
    }
}
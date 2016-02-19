using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Web.Mvc;
using Kentor.AuthServices.Mvc;
using NSubstitute;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Web.Routing;
using Kentor.AuthServices.Tests.Helpers;
using System.Xml.Linq;
using Kentor.AuthServices.Configuration;
using System.IdentityModel.Metadata;
using System.Reflection;
using System.Threading;

namespace Kentor.AuthServices.Tests.Mvc
{
    [TestClass]
    public class AuthServicesControllerTests
    {
        static IOptions defaultOptions = Options.FromConfiguration;
        ClaimsPrincipal originalPrincipal;

        [TestInitialize]
        public void Initialize()
        {
            originalPrincipal = ClaimsPrincipal.Current;
        }

        [TestCleanup]
        public void Cleanup()
        {
            AuthServicesController.Options = defaultOptions;
            Thread.CurrentPrincipal = originalPrincipal;
        }

        private AuthServicesController CreateInstanceWithContext()
        {
            var controllerContext = Substitute.For<ControllerContext>();

            controllerContext.HttpContext = Substitute.For<HttpContextBase>();

            var request = Substitute.For<HttpRequestBase>();
            request.Url.Returns(new Uri("http://example.com"));
            request.Form.Returns(new NameValueCollection());
            controllerContext.HttpContext.Request.Returns(request);

            var response = Substitute.For<HttpResponseBase>();

            return new AuthServicesController()
            {
                ControllerContext = controllerContext
            };
        }

        [TestMethod]
        public void AuthServicesController_SignIn_Returns_SignIn()
        {
            var subject = CreateInstanceWithContext().SignIn();

            subject.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                .Should().Contain("?SAMLRequest");
        }

        [TestMethod]
        public void AuthServicesController_SignIn_Returns_DiscoveryService()
        {
            AuthServicesController.Options = new Options(new SPOptions
            {
                DiscoveryServiceUrl = new Uri("http://ds.example.com"),
                EntityId = new EntityId("https://github.com/KentorIT/authservices")
            });

            var subject = CreateInstanceWithContext();

            var result = subject.SignIn();

            result.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                    .Should().StartWith("http://ds.example.com/?entityID=https%3A%2F%2Fgithub.com%2FKentorIT%2Fauthservices");
        }

        [TestMethod]
        public void AuthServicesController_Acs_Works()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("POST");

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var formValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                SignedXmlHelper.SignXml(response)));

            request.Form.Returns(new NameValueCollection() { { "SAMLResponse", formValue } });
            request.Url.Returns(new Uri("http://url.example.com/url"));

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);

            var controller = new AuthServicesController();
            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            var expected = new { Permanent = false, Url = "http://localhost/LoggedIn" };

            controller.Acs().As<RedirectResult>().ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AuthServicesController_Metadata()
        {
            var subject = ((ContentResult)CreateInstanceWithContext().Index());

            subject.ContentType.Should().Contain("application/samlmetadata+xml");

            var xmlData = XDocument.Parse(subject.Content);

            xmlData.Root.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");
        }

        [TestMethod]
        public void AuthServicesController_SignIn_Returns_Public_Origin()
        {
            AuthServicesController.Options = new Options(new SPOptions
            {
                DiscoveryServiceUrl = new Uri("http://ds.example.com"),
                PublicOrigin = new Uri("https://my.public.origin:8443"),
                EntityId = new EntityId("https://github.com/KentorIT/authservices")
            });

            var subject = CreateInstanceWithContext();

            var result = subject.SignIn();

            result.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                    .Should().StartWith("http://ds.example.com/?entityID=https%3A%2F%2Fgithub.com%2FKentorIT%2Fauthservices&return=https%3A%2F%2Fmy.public.origin%3A8443%2F");
        }

        [TestMethod]
        public void AuthServicesController_Logout_Returns_LogoutRequest()
        {
            var subject = CreateInstanceWithContext();

            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var actual = subject.Logout().As<RedirectResult>();

            actual.Url.Should().StartWith("https://idp.example.com/logout?SAMLRequest=");

            var relayState = HttpUtility.ParseQueryString(new Uri(actual.Url).Query)["RelayState"];

            subject.Response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => c.Name == "Kentor." + relayState));
        }
    }
}

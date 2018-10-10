using System;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Xml.Linq;
using System.Reflection;
using System.Threading;
using Sustainsys.Saml2.HttpModule;
using Sustainsys.Saml2.TestHelpers;
using NSubstitute;
using System.Web.Mvc;
using System.Web;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System.Web.Security;
using System.Web.Routing;

namespace Sustainsys.Saml2.Mvc.Tests
{
    [TestClass]
    public class Saml2ControllerTests
    {
        ClaimsPrincipal originalPrincipal;

        [TestInitialize]
        public void Initialize()
        {
            originalPrincipal = ClaimsPrincipal.Current;
            Saml2Controller.Options = StubFactory.CreateOptions();
            Saml2Controller.Options.SPOptions.DiscoveryServiceUrl = null;
            Saml2Controller.Options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Thread.CurrentPrincipal = originalPrincipal;
        }

        private Saml2Controller CreateInstanceWithContext()
        {
            var controllerContext = Substitute.For<ControllerContext>();

            controllerContext.HttpContext = Substitute.For<HttpContextBase>();

            var request = Substitute.For<HttpRequestBase>();
            request.Url.Returns(new Uri("http://example.com"));
            request.Form.Returns(new NameValueCollection());
            controllerContext.HttpContext.Request.Returns(request);

            var response = Substitute.For<HttpResponseBase>();

            return new Saml2Controller()
            {
                ControllerContext = controllerContext
            };
        }

        [TestMethod]
        public void Saml2Controller_SignIn_Returns_SignIn()
        {
            var subject = CreateInstanceWithContext();
            
            var actual = subject.SignIn().As<RedirectResult>();

            var relayState = HttpUtility.ParseQueryString(new Uri(actual.Url).Query)["RelayState"];

            actual.Url.Should().Contain("?SAMLRequest");

            subject.Response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => c.Name == StoredRequestState.CookieNameBase + relayState));
        }

        [TestMethod]
        public void Saml2Controller_SignIn_Throws_On_CommandResultHandled()
        {
            var subject = CreateInstanceWithContext();
            Saml2Controller.Options.Notifications.SignInCommandResultCreated = (cr, r) =>
            {
                cr.HandledResult = true;
            };

            var actual = subject.Invoking(s => s.SignIn())
                .Should().Throw<NotSupportedException>();
        }

        [TestMethod]
        public void Saml2Controller_SignIn_Returns_DiscoveryService()
        {
            Saml2Controller.Options = new Options(new SPOptions
            {
                DiscoveryServiceUrl = new Uri("http://ds.example.com"),
                EntityId = new EntityId("https://github.com/SustainsysIT/Saml2")
            });

            var subject = CreateInstanceWithContext();

            var result = subject.SignIn();

            result.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                    .Should().StartWith("http://ds.example.com/?entityID=https%3A%2F%2Fgithub.com%2FSustainsysIT%2FSaml2");
        }

        [TestMethod]
        public void Saml2Controller_Acs_Works()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("POST");

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
                InResponseTo=""InResponseToId"">
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

            var relayState = "rs1234";

            request.Form.Returns(new NameValueCollection()
            {
                { "SAMLResponse", formValue },
                { "RelayState", relayState }
            });
            request.Url.Returns(new Uri("http://url.example.com/url"));
            request.Cookies.Returns(new HttpCookieCollection());
            request.Cookies.Add(new HttpCookie(StoredRequestState.CookieNameBase + relayState,
                HttpRequestData.ConvertBinaryData(
                    MachineKey.Protect(
                        new StoredRequestState(null, null, new Saml2Id("InResponseToId"), null).Serialize(),
                        HttpRequestBaseExtensions.ProtectionPurpose))));

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);

            var controller = new Saml2Controller();
            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            var expected = new
            {
                Permanent = false,
                Url = Saml2Controller.Options.SPOptions.ReturnUrl.OriginalString
            };

            controller.Acs().As<RedirectResult>().Should().BeEquivalentTo(expected);

            controller.Response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => c.Expires.Year == 1970));
        }

        [TestMethod]
        public void Saml2Controller_Acs_Throws_On_CommandResultHandled()
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

            request.Form.Returns(new NameValueCollection()
            {
                { "SAMLResponse", formValue }
            });
            request.Url.Returns(new Uri("http://url.example.com/url"));

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);

            var subject = new Saml2Controller();
            subject.ControllerContext = new ControllerContext(httpContext, new RouteData(), subject);

            Saml2Controller.Options.Notifications.AcsCommandResultCreated = (cr, r) =>
            {
                cr.HandledResult = true;
            };

            subject.Invoking(s => s.Acs())
                .Should().Throw<NotSupportedException>();
        }

        [TestMethod]
        public void Saml2Controller_Metadata()
        {
            var subject = CreateInstanceWithContext();

            var result = (ContentResult)subject.Index();

            result.ContentType.Should().Contain("application/samlmetadata+xml");

            subject.Response.Received().AddHeader("Content-Disposition", "attachment; filename=\"github.com_Sustainsys_Saml2.xml\"");

            var xmlData = XDocument.Parse(result.Content);

            xmlData.Root.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");
        }

        [TestMethod]
        public void Saml2Controller_Metadata_Throws_On_CommandResultHandled()
        {
            var subject = CreateInstanceWithContext();
            Saml2Controller.Options.Notifications.MetadataCommandResultCreated = cr =>
            {
                cr.HandledResult = true;
            };

            subject.Invoking(s => s.Index())
                .Should().Throw<NotSupportedException>();
        }

        [TestMethod]
        public void Saml2Controller_SignIn_Returns_Public_Origin()
        {
            Saml2Controller.Options = new Options(new SPOptions
            {
                DiscoveryServiceUrl = new Uri("http://ds.example.com"),
                PublicOrigin = new Uri("https://my.public.origin:8443"),
                EntityId = new EntityId("https://github.com/SustainsysIT/Saml2")
            });

            var subject = CreateInstanceWithContext();

            var result = subject.SignIn();

            result.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                    .Should().StartWith("http://ds.example.com/?entityID=https%3A%2F%2Fgithub.com%2FSustainsysIT%2FSaml2&return=https%3A%2F%2Fmy.public.origin%3A8443%2F");
        }

        [TestMethod]
        public void Saml2Controller_Logout_Returns_LogoutRequest()
        {
            var subject = CreateInstanceWithContext();

            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var actual = subject.Logout().As<RedirectResult>();

            actual.Url.Should().StartWith("https://idp.example.com/logout?SAMLRequest=");

            var relayState = HttpUtility.ParseQueryString(new Uri(actual.Url).Query)["RelayState"];

            subject.Response.Received().SetCookie(
                Arg.Is<HttpCookie>(c => c.Name == StoredRequestState.CookieNameBase + relayState));
        }

        [TestMethod]
        public void Saml2Controller_Logout_Throws_On_CommandResultHandled()
        {
            var subject = CreateInstanceWithContext();

            Saml2Controller.Options.Notifications.LogoutCommandResultCreated = cr =>
            {
                cr.HandledResult = true;
            };

            subject.Invoking(s => s.Logout())
                .Should().Throw<NotSupportedException>();
        }

        [TestMethod]
        public void Saml2Controller_Options_LoadedIfNotSet()
        {
            Saml2Controller.Options = null;
            Saml2Controller.Options.Should().NotBeNull();
        }
    }
}

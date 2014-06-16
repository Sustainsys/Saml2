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
using Kentor.AuthServices.TestHelpers;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AuthServicesControllerTests
    {
        [TestMethod]
        public void AuthServicesController_SignIn_Returns_SignIn()
        {
            var subject = new AuthServicesController().SignIn();

            subject.Should().BeOfType<RedirectResult>().And
                .Subject.As<RedirectResult>().Url
                .Should().Contain("?SAMLRequest");
        }

        [NotReRunnable]
        [TestMethod]
        public void AuthServicesController_Acs_Works()
        {
            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("POST");

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""AuthServicesController_Acs_Should_SetIdentity"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""AuthServicesController_Acs_Should_SetIdentity_Assertion1""
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

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);

            var ids = new ClaimsIdentity[]
                { new ClaimsIdentity("Federation"), new ClaimsIdentity("ClaimsAuthenticationManager") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            ids[1].AddClaim(new Claim(ClaimTypes.Role, "RoleFromClaimsAuthManager", null, "ClaimsAuthenticationManagerMock"));

            var controller = new AuthServicesController();
            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            var expected = new { Permanent = false, Url = "http://localhost/LoggedIn"};

            controller.Acs().As<RedirectResult>().ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AuthServicesController_Metadata()
        {
            var subject = ((ContentResult)new AuthServicesController().Index());

            subject.ContentType.Should().Contain("application/samlmetadata+xml");

            var xmlData = XDocument.Parse(subject.Content);

            xmlData.Root.Name.Should().Be(Saml2Namespaces.Saml2Metadata + "EntityDescriptor");
        }
    }
}

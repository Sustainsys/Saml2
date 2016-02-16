using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Exceptions;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.Helpers;
using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kentor.AuthServices.Tests.WebSSO
{
    [TestClass]
    public class LogoutCommandTests
    {
        private IPrincipal principal;

        [TestInitialize]
        public void SaveCurrentPrincipal()
        {
            principal = Thread.CurrentPrincipal;
        }

        [TestCleanup]
        public void RestoreCurrentPrincipal()
        {
            Thread.CurrentPrincipal = principal;
        }

        [TestMethod]
        public void LogoutCommand_Run_NullcheckRequest()
        {
            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(null, StubFactory.CreateOptions()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void LogoutCommand_Run_NullcheckOptions()
        {
            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(new HttpRequestData("GET", new Uri("http://localhost")), null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/Logout"));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true
                // Deliberately not comparing Location.
            };

            actual.ShouldBeEquivalentTo(expected, opt => opt.Excluding(cr => cr.Location));
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest_PrefersAuthServicesLogoutNameId()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId"),
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, "Saml2NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/Logout"));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true
                // Deliberately not comparing Location.
            };

            actual.ShouldBeEquivalentTo(expected, opt => opt.Excluding(cr => cr.Location));
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("http://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET", bindResult.Location,
                "http://sp-internal.example.com/path/AuthServices", null);

            var options = StubFactory.CreateOptions();
            ((SPOptions)options.SPOptions).PublicOrigin = new Uri("https://sp.example.com/path/");

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                Location = new Uri("https://sp.example.com/path/"),
                HttpStatusCode = HttpStatusCode.SeeOther
            };

            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutRequest()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID"
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData("GET", bindResult.Location);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(httpRequest, options);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true
                // Deliberately not comparing Location
            };

            HttpUtility.ParseQueryString(actual.Location.Query)["Signature"]
                .Should().NotBeNull("LogoutResponse should be signed");

            actual.ShouldBeEquivalentTo(expected, opt => opt.Excluding(cr => cr.Location));

            var actualMessage = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(new HttpRequestData("GET", actual.Location), options).Data;

            var expectedMessage = XmlHelpers.FromString(
                $@"<samlp:LogoutResponse xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""
                    Destination=""https://idp.example.com/logout""
                    Version=""2.0"">
                    <Issuer>{options.SPOptions.EntityId.Id}</Issuer>
                    <samlp:Status>
                        <samlp:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success""/>
                    </samlp:Status>
                </samlp:LogoutResponse>").DocumentElement;

            // Set generated attributes to actual values.
            expectedMessage.SetAttribute("ID", actualMessage.GetAttribute("ID"));
            expectedMessage.SetAttribute("IssueInstant", actualMessage.GetAttribute("IssueInstant"));
            expectedMessage.SetAttribute("InResponseTo", request.Id.Value);

            actualMessage.Should().BeEquivalentTo(expectedMessage);
        }

        [TestMethod]
        public void LogoutCommand_Run_IncomingRequest_ThrowsOnNoConfiguredSigningCert()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID"
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData("GET", bindResult.Location);

            var options = StubFactory.CreateOptions();

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .ShouldThrow<ConfigurationErrorsException>()
                .WithMessage("Received a Single Logout request from \"https://idp.example.com\" but cannot reply because single logout responses must be signed and there is no signing certificate configured. Looks like the idp is configured for Single Logout despite AuthServices not exposing that functionality in the metadata.");
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnLogoutResponseStatusNonSuccess()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("http://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET", bindResult.Location,
                "http://sp-internal.example.com/path/AuthServices", null);

            var options = StubFactory.CreateOptions();
            ((SPOptions)options.SPOptions).PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, options))
                .ShouldThrow<UnsuccessfulSamlOperationException>()
                .And.Message.Should().Be("Idp returned status \"Requester\", indicating that the single logout failed. The local session has been successfully terminated.");
        }
    }
}

using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Exceptions;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.Helpers;
using Kentor.AuthServices.Tests.Owin;
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
        public void LogoutCommand_InstanceRun_NullcheckRequest()
        {
            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(null, StubFactory.CreateOptions()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void LogoutCommand_StaticRun_NullcheckRequest()
        {
            Action a = () => LogoutCommand.Run(null, null, StubFactory.CreateOptions());

            a.ShouldThrow<ArgumentNullException>()
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

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/AuthServices/Logout"));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            ((SPOptions)(options.SPOptions)).PublicOrigin = new Uri("https://sp.example.com/");

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true,
                // Deliberately not comparing Location.
                // Deliberately not comparing SetCookieName.
                SetCookieData = "https://sp.example.com/"
            };

            actual.ShouldBeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName));

            var relayState = HttpUtility.ParseQueryString(actual.Location.Query)["RelayState"];
            actual.SetCookieName.Should().Be("Kentor." + relayState);
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_PreservesReturnUrl()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/Logout?ReturnUrl=%2FLoggedOut"));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.SetCookieData.Should().Be("http://sp.example.com/LoggedOut");
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
                TerminateLocalSession = true,
                // Deliberately not comparing Location.
                SetCookieData = "http://sp.example.com/",
            };

            actual.ShouldBeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName));
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse()
        {
            var relayState = "MyRelayState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                RelayState = relayState
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var cookies = new KeyValuePair<string, string>[] 
            {
                new KeyValuePair<string, string>(
                    "Kentor." + relayState,
                    StubDataProtector.Protect("http://loggedout.example.com", false))
            };

            var request = new HttpRequestData("GET",
                bindResult.Location,
                "http://sp-internal.example.com/path/AuthServices",
                null,
                cookies,
                StubDataProtector.Unprotect);
            
            var options = StubFactory.CreateOptions();
            ((SPOptions)options.SPOptions).PublicOrigin = new Uri("https://sp.example.com/path/");

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                Location = new Uri("http://loggedout.example.com"),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = "Kentor." + relayState
            };

            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutRequest_ReceivedThroughRedirectBinding()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData("GET", bindResult.Location);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            
            // We're using unbind to verify the created message and UnBind
            // expects the issuer to be a known Idp for signature validation.
            // Add a dummy with the right issuer name and key.
            var dummyIdp = new IdentityProvider(options.SPOptions.EntityId, options.SPOptions);
            dummyIdp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);
            options.IdentityProviders.Add(dummyIdp);

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

            var actualUnbindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(new HttpRequestData("GET", actual.Location), options);

            var actualMessage = actualUnbindResult.Data;

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

            actualUnbindResult.RelayState.Should().Be(request.RelayState);
            actualUnbindResult.TrustLevel.Should().Be(TrustLevel.Signature);
        }

        [TestMethod]
        public void LogoutCommand_Run_DetectsSignatureInLogoutRequestReceivedThroughPostBinding()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
            };

            var xml = XmlHelpers.FromString(request.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));
            
            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(httpRequest, options);

            HttpUtility.ParseQueryString(actual.Location.Query)
                .Keys.Should().Contain("SAMLResponse", "if the request was properly detected a response should be generated");
        }


        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnSignatureInLogoutRequestReceivedThroughPostBindingIfCertificateIsntValid_WhenCertificateValidationIsConfigured()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
            };

            var xml = XmlHelpers.FromString(request.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));

            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            ((SPOptions)options.SPOptions).ValidateCertificates = true;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("The signature was valid, but the verification of the certificate failed. Is it expired or revoked? Are you sure you really want to enable ValidateCertificates (it's normally not needed)?");
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
                .WithMessage("Received a LogoutRequest from \"https://idp.example.com\" but cannot reply because single logout responses must be signed and there is no signing certificate configured. Looks like the idp is configured for Single Logout despite AuthServices not exposing that functionality in the metadata.");
        }

        [TestMethod]
        public void LogoutCommand_Run_IncomingRequest_ThroughRedirectBinding_ThrowsOnMissingSignature()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID"
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData("GET", bindResult.Location);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .ShouldThrow<UnsuccessfulSamlOperationException>()
                .WithMessage("Received a LogoutRequest from https://idp.example.com that cannot be processed because it is not signed.");
        }


        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnLogoutResponseStatusNonSuccess()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET", bindResult.Location,
                "http://sp-internal.example.com/path/AuthServices", null, null, null);

            var options = StubFactory.CreateOptions();
            ((SPOptions)options.SPOptions).PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, options))
                .ShouldThrow<UnsuccessfulSamlOperationException>()
                .And.Message.Should().Be("Idp returned status \"Requester\", indicating that the single logout failed. The local session has been successfully terminated.");
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfUnknownNameIdIssuer()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "Someone", null, "NotEvenAUri")
                    }));

            LogoutCommand_Run_LocalLogout();
        }

        private void LogoutCommand_Run_LocalLogout()
        {
            var subject = CommandFactory.GetCommand(CommandFactory.LogoutCommandName);

            var options = StubFactory.CreateOptions();
            options.SPOptions.SigningServiceCertificate.Should().BeNull("this helper is used for test of behaviour when no certificate is configured");
            
            var actual = subject.Run(
                new HttpRequestData("GET", new Uri("http://localhost/Logout?ReturnUrl=LoggedOut")),
                options);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri("http://localhost/LoggedOut"),
                TerminateLocalSession = true
            };

            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoNameId()
        {
            ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier)
                .Should().BeNull("this is a test for the case where there is no NameIdentifier");

            LogoutCommand_Run_LocalLogout();
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoSessionId()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com")
                }));

            LogoutCommand_Run_LocalLogout();
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfIdpHasNoLogoutEndpoint()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp2.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp2.example.com")
                }));

            LogoutCommand_Run_LocalLogout();
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfThereIsNoSigninCertificateForTheSP()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }));

            LogoutCommand_Run_LocalLogout();
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnUnknownMessageRecevied()
        {
            var msg = new Saml2MessageImplementation
            {
                MessageName = "SAMLRequest",
                SigningCertificate = SignedXmlHelper.TestCert,
                DestinationUrl = new Uri("http://localhost"),
                XmlData = $"<Unknown><Issuer xmlns=\"{Saml2Namespaces.Saml2Name}\">https://idp.example.com</Issuer></Unknown>"                
            };

            var url = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(msg).Location;

            var request = new HttpRequestData("GET", url);

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, StubFactory.CreateOptions()))
                .ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnMissingIssuerInReceivedMessage()
        {
            var msg = new Saml2MessageImplementation
            {
                MessageName = "SAMLRequest",
                SigningCertificate = SignedXmlHelper.TestCert,
                DestinationUrl = new Uri("http://localhost"),
                XmlData = "<Xml />"
            };

            var url = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(msg).Location;

            var request = new HttpRequestData("GET", url);

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, StubFactory.CreateOptions()))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("There is no Issuer element in the message, so there is no way to know what certificate to use to validate the signature.");
        }
    }
}

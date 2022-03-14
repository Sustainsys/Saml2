using FluentAssertions;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Exceptions;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.TestHelpers;
using Sustainsys.Saml2.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.IdentityModel.Tokens.Saml2;
using Sustainsys.Saml2.Metadata.Exceptions;
using Sustainsys.Saml2.Metadata.Tokens;

namespace Sustainsys.Saml2.Tests.WebSso
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
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void LogoutCommand_StaticRun_NullcheckRequest()
        {
            Action a = () => LogoutCommand.Run(null, null, StubFactory.CreateOptions());

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void LogoutCommand_StaticRun_NullcheckOptions()
        {
            Action a = () => LogoutCommand.Run(new HttpRequestData("GET", new Uri("http://localhost")), null, null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void LogoutCommand_Run_NullcheckOptions()
        {
            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(new HttpRequestData("GET", new Uri("http://localhost")), null))
                .Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/Saml2/Logout"))
            {
                User = user
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/");

            CommandResult notifiedCommandResult = null;
            options.Notifications.LogoutCommandResultCreated = cr =>
            {
                notifiedCommandResult = cr;
            };

            Saml2LogoutRequest logoutRequest = null;
            options.Notifications.LogoutRequestCreated = (lr, u, idp) =>
            {
                logoutRequest = lr;
                u.Identities.Single().FindFirst(Saml2ClaimTypes.SessionIndex).Value.Should().Be("SessionId");
                idp.EntityId.Id.Should().Be("https://idp.example.com");
            };

            var logoutRequestXmlCreatedCalled = false;
            options.Notifications.LogoutRequestXmlCreated = (lr, xd, bt) =>
            {
                logoutRequestXmlCreatedCalled = true;
                xd.Root.Attribute("ID").Value.Should().Be(lr.Id.Value);
                bt.Should().Be(Saml2BindingType.HttpRedirect);
            };

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.Should().BeSameAs(notifiedCommandResult);
            logoutRequest.Should().NotBeNull();
            logoutRequestXmlCreatedCalled.Should().BeTrue();

            var expected = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true,
                // Deliberately not comparing Location.
                // Deliberately not comparing SetCookieName.
                RequestState = new StoredRequestState(
                    new EntityId("https://idp.example.com"),
                    new Uri("https://sp.example.com/"),
                    null,
                    null),
                SetCookieSecureFlag = true
            };

            actual.Should().BeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName)
                .Excluding(cr => cr.RelayState)
                .Excluding(cr => cr.RequestState.MessageId));

            var relayState = HttpUtility.ParseQueryString(actual.Location.Query)["RelayState"];
            actual.SetCookieName.Should().Be(StoredRequestState.CookieNameBase + relayState);
            actual.RelayState.Should().Be( relayState );
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest_POST()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/Saml2/Logout"))
            {
                User = user
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.IdentityProviders[new EntityId("https://idp.example.com")]
                .SingleLogoutServiceBinding = Saml2BindingType.HttpPost;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void LogoutCommand_Run_NoCookieName_WhenLogoutStateDisabled()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/Saml2/Logout"))
            {
                User = user
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/");
            options.SPOptions.Compatibility.DisableLogoutStateCookie = true;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var relayState = HttpUtility.ParseQueryString(actual.Location.Query)["RelayState"];
            actual.SetCookieName.Should().Be(null);
            actual.RelayState.Should().Be( relayState );
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_PreservesReturnUrl()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/Saml2/Logout?ReturnUrl=%2FLoggedOut"))
            {
                User = user
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.RequestState.ReturnUrl.OriginalString.Should().Be("/LoggedOut");
        }

        [TestMethod]
        public void LogoutCommand_Run_ChecksForLocalReturnUrl()
        {
            var absoluteUri = HttpUtility.UrlEncode("http://google.com");
            var request = new HttpRequestData("GET", new Uri($"http://sp.example.com/Saml2/Logout?ReturnUrl={absoluteUri}"));
            var options = StubFactory.CreateOptions();

            Action a = () => CommandFactory.GetCommand(CommandFactory.LogoutCommandName).Run(request, options);
            a.Should().Throw<InvalidOperationException>().WithMessage("Return Url must be a relative Url.");
        }

        [TestMethod]
        public void LogoutCommand_Run_ChecksForLocalReturnUrlProtocolRelative()
        {
            var absoluteUri = HttpUtility.UrlEncode("//google.com");
            var request = new HttpRequestData("GET", new Uri($"http://sp.example.com/Saml2/Logout?ReturnUrl={absoluteUri}"));
            var options = StubFactory.CreateOptions();

            Action a = () => CommandFactory.GetCommand(CommandFactory.LogoutCommandName).Run(request, options);
            a.Should().Throw<InvalidOperationException>().WithMessage("Return Url must be a relative Url.");
        }

        [TestMethod]
        public void LogoutCommand_Run_Calls_NotificationForAbsoluteUrl()
        {
            var absoluteUri = HttpUtility.UrlEncode("http://google.com");
            var request = new HttpRequestData("GET", new Uri($"http://sp.example.com/Saml2/Logout?ReturnUrl={absoluteUri}"));
            var options = StubFactory.CreateOptions();

            var validateAbsoluteReturnUrlCalled = false;

            options.Notifications.ValidateAbsoluteReturnUrl =
                (url) =>
                {
                    validateAbsoluteReturnUrlCalled = true;
                    return true;
                };

            Action a = () => CommandFactory.GetCommand(CommandFactory.LogoutCommandName).Run(request, options);
            a.Should().NotThrow<InvalidOperationException>("the ValidateAbsoluteReturnUrl notification returns true");
            validateAbsoluteReturnUrlCalled.Should().BeTrue("the ValidateAbsoluteReturnUrl notification should have been called");
        }

        [TestMethod]
        public void LogoutCommand_Run_DoNotCalls_NotificationForRelativeUrl()
        {
            var relativeUri = HttpUtility.UrlEncode("/");
            var request = new HttpRequestData("GET", new Uri($"http://sp.example.com/Saml2/Logout?ReturnUrl={relativeUri}"));
            var options = StubFactory.CreateOptions();

            var validateAbsoluteReturnUrlCalled = false;

            options.Notifications.ValidateAbsoluteReturnUrl =
                (url) =>
                {
                    validateAbsoluteReturnUrlCalled = true;
                    return true;
                };

            Action a = () => CommandFactory.GetCommand(CommandFactory.LogoutCommandName).Run(request, options);
            a.Should().NotThrow<InvalidOperationException>("the ReturnUrl is relative");
            validateAbsoluteReturnUrlCalled.Should().BeFalse("the ValidateAbsoluteReturnUrl notification should not have been called");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest_IgnoresThreadPrincipal()
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "PrincipalWithNoSession"),
                }, "Federation"));

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "ApplicationNameId"),
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,Saml2NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/Saml2/Logout"))
            {
                User = user
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                TerminateLocalSession = true,
                // Deliberately not comparing Location.
                RequestState = new StoredRequestState(
                    new EntityId("https://idp.example.com"),
                    new Uri("http://sp.example.com/"),
                    null,
                    null)
            };

            actual.Should().BeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName)
                .Excluding(cr => cr.RelayState)
                .Excluding(cr => cr.RequestState.MessageId));
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse()
        {
            var relayState = "MyRelayState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                RelayState = relayState
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET",
                bindResult.Location,
                "http://sp-internal.example.com/path/Saml2",
                null,
                new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null));
            
            var options = StubFactory.CreateOptions();
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandResult notifiedCommandResult = null;
            options.Notifications.LogoutCommandResultCreated = cr =>
            {
                notifiedCommandResult = cr;
            };
            var responseUnboundCalled = false;
            options.Notifications.MessageUnbound = ur =>
            {
                ur.Should().NotBeNull();
                responseUnboundCalled = true;
            };

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.Should().BeSameAs(notifiedCommandResult);
            responseUnboundCalled.Should().BeTrue("the ResponseUnbound notification should have been called.");

            var expected = new CommandResult
            {
                Location = new Uri("http://loggedout.example.com"),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = StoredRequestState.CookieNameBase + relayState,
                SetCookieSecureFlag = true
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_RejectsUnsignedLogoutResponse()
        {
            var relayState = "MyRelayState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                RelayState = relayState
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET",
                bindResult.Location,
                "http://sp-internal.example.com/path/Saml2",
                null,
                new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null));

            var options = StubFactory.CreateOptions();

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, options))
                .Should().Throw<UnsuccessfulSamlOperationException>();
        }

        [TestMethod]
        public void LogoutCommand_Run_AcceptsUnsignedLogoutResponseIfCompatFlagSet()
        {
            var relayState = "MyRelayState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                RelayState = relayState
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET",
                bindResult.Location,
                "http://sp-internal.example.com/path/Saml2",
                null,
                new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null));

            var options = StubFactory.CreateOptions();
            options.SPOptions.Compatibility.AcceptUnsignedLogoutResponses = true;

            // Should not throw.
            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse_InPost()
        {
            var relayState = "TestState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var xml = XmlHelpers.XmlDocumentFromString(response.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var responseData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));

            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new[] { responseData }),
                    new KeyValuePair<string, IEnumerable<string>>("RelayState", new[] { relayState })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null)
            {
                StoredRequestState = new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null)
            };

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(httpRequest, options);

            var expected = new CommandResult
            {
                Location = new Uri("http://loggedout.example.com"),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = StoredRequestState.CookieNameBase + relayState,
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutRequest_ReceivedThroughRedirectBinding()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData(
                "GET",
                bindResult.Location,
                "/",
                null,
                cookieName => null,
                null,
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(Saml2ClaimTypes.SessionIndex, "SessionID") })));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            Saml2LogoutResponse logoutResponse = null;
            options.Notifications.LogoutResponseCreated = (resp, req, u, idp) =>
            {
                logoutResponse = resp;
                req.Id.Value.Should().Be(request.Id.Value);
                u.Identities.First().FindFirst(Saml2ClaimTypes.SessionIndex).Value.Should().Be("SessionID");
                idp.EntityId.Id.Should().Be(request.Issuer.Id);
            };

            bool xmlCreatedCalled = false;
            options.Notifications.LogoutResponseXmlCreated = (resp, xml, bt) =>
            {
                xmlCreatedCalled = true;
                resp.Should().BeSameAs(logoutResponse);
                xml.Root.Attribute("ID").Value.Should().BeSameAs(resp.Id.Value);
                bt.Should().Be(Saml2BindingType.HttpRedirect);
            };

            CommandResult notifiedCommandResult = null;
            options.Notifications.LogoutCommandResultCreated = cr =>
            {
                notifiedCommandResult = cr;
            };
            
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

            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(cr => cr.Location));
            actual.Should().BeSameAs(notifiedCommandResult);
            logoutResponse.InResponseTo.Value.Should().Be(request.Id.Value);
            xmlCreatedCalled.Should().BeTrue();

            var actualUnbindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(new HttpRequestData("GET", actual.Location), options);

            var actualMessage = actualUnbindResult.Data;

            var expectedMessage = XmlHelpers.XmlDocumentFromString(
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
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
            };

            var xml = XmlHelpers.XmlDocumentFromString(request.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));
            
            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLRequest", new[] { requestData })
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
        public void LogoutCommand_Run_ChecksSignatureAlgorithmStrength()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature, // Ignored
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID"
            };

            var xml = XmlHelpers.XmlDocumentFromString(request.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));

            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.MinIncomingSigningAlgorithm = SecurityAlgorithms.RsaSha384Signature;
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("*weak*");
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnSignatureInLogoutRequestReceivedThroughPostBindingIfCertificateIsntValid_WhenCertificateValidationIsConfigured()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID",
            };

            var xml = XmlHelpers.XmlDocumentFromString(request.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));

            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.SPOptions.ValidateCertificates = true;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("The signature was valid, but the verification of the certificate failed. Is it expired or revoked? Are you sure you really want to enable ValidateCertificates (it's normally not needed)?");
        }

        [TestMethod]
        public void LogoutCommand_Run_IncomingRequest_ThrowsOnNoConfiguredSigningCert()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                NameId = new Saml2NameIdentifier("NameId"),
                SessionIndex = "SessionID"
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(request);

            var httpRequest = new HttpRequestData("GET", bindResult.Location);

            var options = StubFactory.CreateOptions();

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(httpRequest, options))
                .Should().Throw<ConfigurationErrorsException>()
                .WithMessage("Received a LogoutRequest from \"https://idp.example.com\" but cannot reply because single logout responses must be signed and there is no signing certificate configured. Looks like the idp is configured for Single Logout despite Saml2 not exposing that functionality in the metadata.");
        }

        [TestMethod]
        public void LogoutCommand_Run_IncomingRequest_ThrowsOnNoConfiguredLogoutEndPointOnIdp()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp2.example.com"),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
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
                .Should().Throw<InvalidOperationException>()
                .WithMessage("*LogoutRequest*\"https://idp2.example.com\"*cannot reply*logout endpoint*idp*");
        }

        [TestMethod]
        public void LogoutCommand_Run_IncomingRequest_ThroughRedirectBinding_ThrowsOnMissingSignature()
        {
            var request = new Saml2LogoutRequest()
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
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
                .Should().Throw<UnsuccessfulSamlOperationException>()
                .WithMessage("Received a LogoutRequest from https://idp.example.com that cannot be processed because it is not signed.");
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnLogoutResponseStatusNonSuccess()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET", bindResult.Location,
                "http://sp-internal.example.com/path/Saml2", null, null, null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, options))
                .Should().Throw<UnsuccessfulSamlOperationException>()
                .And.Message.Should().Be("Idp returned status \"Requester\", indicating that the single logout failed. The local session has been successfully terminated.");
        }

        [TestMethod]
        public void LogoutCommand_Run_LetsNotificationHandleStatus()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Requester)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var request = new HttpRequestData("GET", bindResult.Location,
                "http://sp-internal.example.com/path/Saml2", null, null, null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/path/");
            options.Notifications.ProcessSingleLogoutResponseStatus = (logoutResponse, requestState) => true;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName).Run(request, options);
            actual.Location.Should().BeEquivalentTo(options.SPOptions.PublicOrigin);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoUser()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user: null);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfUnknownNameLogoutNameIdIssuer()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://unknown.invalid"),
                    }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user);
        }

        private void LogoutCommand_Run_LocalLogout(IOptions options, ClaimsPrincipal user)
        {
            var subject = CommandFactory.GetCommand(CommandFactory.LogoutCommandName);
                       
            var actual = subject.Run(
                new HttpRequestData("GET", new Uri("http://localhost/Logout?ReturnUrl=%2FLoggedOut"))
                {
                    User = user
                },
                options);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri("/LoggedOut", UriKind.RelativeOrAbsolute),
                TerminateLocalSession = true
            };

            actual.Should().BeEquivalentTo(expected);
        }

        // Meta test to ensure that the test helper does logout if enabled.
        [TestMethod]
        public void LogoutCommand_Run_FederatedLogoutIfEverythingIsConfigured()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            Action a = () => LogoutCommand_Run_LocalLogout(options, user);

            a.Should().Throw<AssertFailedException>();
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoLogoutNameId()
        {
			var user = new ClaimsPrincipal(
				new ClaimsIdentity());
            user.FindFirst(Saml2ClaimTypes.LogoutNameIdentifier)
                .Should().BeNull("this is a test for the case where there is no NameIdentifier");

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoSessionId()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfIdpHasNoLogoutEndpoint()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp2.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp2.example.com")
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfLogoutRequestDisabled()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var idpEntityId = new EntityId("https://idp.example.com");
            options.IdentityProviders[idpEntityId].DisableOutboundLogoutRequests = true;

            LogoutCommand_Run_LocalLogout(options, user);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfThereIsNoSigninCertificateForTheSP()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(Saml2ClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(Saml2ClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.SigningServiceCertificate.Should().BeNull("this helper is used for test of behaviour when no certificate is configured");

            LogoutCommand_Run_LocalLogout(options, user);
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
                .Should().Throw<NotImplementedException>();
        }

        [TestMethod]
        public void LogoutCommand_Run_ThrowsOnMissingIssuerInReceivedMessage()
        {
            var msg = new Saml2MessageImplementation
            {
                MessageName = "SAMLRequest",
                SigningCertificate = SignedXmlHelper.TestCert,
                DestinationUrl = new Uri("http://localhost"),
                XmlData = "<samlp:LogoutRequest xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"/>"
            };

            var url = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(msg).Location;

            var request = new HttpRequestData("GET", url);

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, StubFactory.CreateOptions()))
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("There is no Issuer element in the message, so there is no way to know what certificate to use to validate the signature.");
        }

        [TestMethod]
        public void LogoutCommand_Run_UsesBindingFromNotification()
        {
            var subject = new LogoutCommand();
            var options = StubFactory.CreateOptions();
            options.Notifications.GetBinding = r => new StubSaml2Binding();

            var request = new HttpRequestData("GET", new Uri("http://host"));

            subject.Invoking(s => s.Run(request, options))
                .Should().Throw<NotImplementedException>()
                .WithMessage("StubSaml2Binding.*");
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse_UsesApplicationPathWhenStateDisabled()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                RelayState = null
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var applicationPath = "http://sp-internal.example.com/path/Saml2/";
            var request = new HttpRequestData("GET", bindResult.Location, applicationPath, null, null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.Compatibility.DisableLogoutStateCookie = true;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            var expected = new CommandResult
            {
                Location = new Uri(applicationPath),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = null
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse_UsesReturnPathWhenStateDisabled()
        {
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/Saml2/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature,
                RelayState = null
            };

            var bindResult = Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(response);

            var applicationPath = "http://sp-internal.example.com/path/Saml2/";
            var returnPath = "http://sp-internal.example.com/path/anotherpath";
            var request = new HttpRequestData("GET", bindResult.Location, applicationPath, null, null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.Compatibility.DisableLogoutStateCookie = true;

            var actual = LogoutCommand.Run(request, returnPath, options);

            var expected = new CommandResult
            {
                Location = new Uri( returnPath ),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = null
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_InitiateLogout_NullcheckRequest()
        {
            Action a = () => LogoutCommand.InitiateLogout(null, null, null, false);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void LogoutCommand_InitiateLogout_NullcheckOptions()
        {
            Action a = () => LogoutCommand.InitiateLogout(
                new HttpRequestData("GET", new Uri("http://l")),
                null,
                null,
                false);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}

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
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/AuthServices/Logout"));
            request.User = user;

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/");

            CommandResult notifiedCommandResult = null;
            options.Notifications.LogoutCommandResultCreated = cr =>
            {
                notifiedCommandResult = cr;
            };

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.Should().BeSameAs(notifiedCommandResult);

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
                    null)
            };

            actual.ShouldBeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName)
                .Excluding(cr => cr.RequestState.MessageId));

            var relayState = HttpUtility.ParseQueryString(actual.Location.Query)["RelayState"];
            actual.SetCookieName.Should().Be("Kentor." + relayState);
            actual.Location.GetLeftPart(UriPartial.Path).Should().Be("https://idp.example.com/logout");
        }

        [TestMethod]
        public void LogoutCommand_Run_ReturnsLogoutRequest_POST()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp-internal.example.com/AuthServices/Logout"));
            request.User = user;

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);
            options.IdentityProviders[new EntityId("https://idp.example.com")]
                .SingleLogoutServiceBinding = Saml2BindingType.HttpPost;

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void LogoutCommand_Run_PreservesReturnUrl()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/Logout?ReturnUrl=%2FLoggedOut"));
            request.User = user;

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.RequestState.ReturnUrl.OriginalString.Should().Be("/LoggedOut");
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
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,Saml2NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }, "Federation"));

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com/AuthServices/Logout"));
            request.User = user;

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

            actual.ShouldBeEquivalentTo(expected, opt => opt
                .Excluding(cr => cr.Location)
                .Excluding(cr => cr.SetCookieName)
                .Excluding(cr => cr.RequestState.MessageId));
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

            var request = new HttpRequestData("GET",
                bindResult.Location,
                "http://sp-internal.example.com/path/AuthServices",
                null,
                new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null));
            
            var options = StubFactory.CreateOptions();
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandResult notifiedCommandResult = null;
            options.Notifications.LogoutCommandResultCreated = cr =>
            {
                notifiedCommandResult = cr;
            };

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(request, options);

            actual.Should().BeSameAs(notifiedCommandResult);

            var expected = new CommandResult
            {
                Location = new Uri("http://loggedout.example.com"),
                HttpStatusCode = HttpStatusCode.SeeOther,
                ClearCookieName = "Kentor." + relayState
            };

            actual.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void LogoutCommand_Run_HandlesLogoutResponse_InPost()
        {
            var relayState = "TestState";
            var response = new Saml2LogoutResponse(Saml2StatusCode.Success)
            {
                DestinationUrl = new Uri("http://sp.example.com/path/AuthServices/logout"),
                Issuer = new EntityId("https://idp.example.com"),
                InResponseTo = new Saml2Id(),
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var xml = XmlHelpers.FromString(response.ToXml());
            xml.Sign(SignedXmlHelper.TestCert);

            var responseData = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml.OuterXml));
            
            var httpRequest = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLResponse", new[] { responseData }),
                    new KeyValuePair<string, string[]>("RelayState", new[] { relayState })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            httpRequest.StoredRequestState = new StoredRequestState(null, new Uri("http://loggedout.example.com"), null, null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            var actual = CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Run(httpRequest, options);

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

            actual.ShouldBeEquivalentTo(expected, opt => opt.Excluding(cr => cr.Location));
            actual.Should().BeSameAs(notifiedCommandResult);

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
            options.SPOptions.ValidateCertificates = true;

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
            options.SPOptions.PublicOrigin = new Uri("https://sp.example.com/path/");

            CommandFactory.GetCommand(CommandFactory.LogoutCommandName)
                .Invoking(c => c.Run(request, options))
                .ShouldThrow<UnsuccessfulSamlOperationException>()
                .And.Message.Should().Be("Idp returned status \"Requester\", indicating that the single logout failed. The local session has been successfully terminated.");
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoUser()
        {
            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            LogoutCommand_Run_LocalLogout(options, user: null);
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfUnknownNameIdIssuer()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "Someone", null, "NotEvenAUri")
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

            actual.ShouldBeEquivalentTo(expected);
        }

        // Meta test to ensure that the test helper does logout if enabled.
        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfEverythingIsConfigured()
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]
                {
                    new Claim(AuthServicesClaimTypes.LogoutNameIdentifier, ",,,,NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
                }));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificates.Add(SignedXmlHelper.TestCert);

            Action a = () => LogoutCommand_Run_LocalLogout(options, user);

            a.ShouldThrow<AssertFailedException>();
        }

        [TestMethod]
        public void LogoutCommand_Run_LocalLogoutIfNoNameId()
        {
            var user = ClaimsPrincipal.Current;
            user.FindFirst(ClaimTypes.NameIdentifier)
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
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com")
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
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp2.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp2.example.com")
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
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
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
                    new Claim(ClaimTypes.NameIdentifier, "NameId", null, "https://idp.example.com"),
                    new Claim(AuthServicesClaimTypes.SessionIndex, "SessionId", null, "https://idp.example.com")
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

        [TestMethod]
        public void LogoutCommand_Run_UsesBindingFromNotification()
        {
            var subject = new LogoutCommand();
            var options = StubFactory.CreateOptions();
            options.Notifications.GetBinding = r => new StubSaml2Binding();

            var request = new HttpRequestData("GET", new Uri("http://host"));

            subject.Invoking(s => s.Run(request, options))
                .ShouldThrow<NotImplementedException>()
                .WithMessage("StubSaml2Binding.*");
        }
    }
}

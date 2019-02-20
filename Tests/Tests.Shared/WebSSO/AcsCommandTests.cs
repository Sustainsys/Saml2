using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.WebSso;
using System.Reflection;
using System.Configuration;
using Sustainsys.Saml2.Exceptions;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Tests.WebSSO;
using Sustainsys.Saml2.TestHelpers;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Runtime.CompilerServices;

namespace Sustainsys.Saml2.Tests.WebSso
{
    [TestClass]
    public class AcsCommandTests
    {
        [TestMethod]
        public void AcsCommand_Run_NullCheckRequest()
        {
            Action a = () => new AcsCommand().Run(null, StubFactory.CreateOptions());

            // Verify exception is thrown and that it is thrown directly by the Run()
            // method and not by some method being called by Run().
            a.Should().Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "request")
                .Where(e => e.TargetSite.Name == "Run");
        }

        [TestMethod]
        public void AcsCommand_Run_NullCheckOptions()
        {
            Action a = () => new AcsCommand().Run(new HttpRequestData("GET", new Uri("http://localhost")), null);

            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnNoSamlResponseFound()
        {
            Action a = () => new AcsCommand().Run(
                new HttpRequestData("GET", new Uri("http://localhost")),
                Options.FromConfiguration);

            a.Should().Throw<NoSamlResponseFoundException>()
                .WithMessage("No Saml2 Response found in the http request.");
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnNotBase64InFormResponse()
        {
            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { "#¤!2" })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            Action a = () => new AcsCommand().Run(r, Options.FromConfiguration);

            a.Should().Throw<BadFormatSamlResponseException>()
                .WithMessage("The SAML Response did not contain valid BASE64 encoded data.")
                .WithInnerException<FormatException>();
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnIncorrectXml()
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("<foo />"));
            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { encoded })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            Action a = () => new AcsCommand().Run(r, Options.FromConfiguration);

            a.Should().Throw<BadFormatSamlResponseException>()
                .WithMessage("The SAML response contains incorrect XML")
                .Where(ex => ex.Data["Saml2Response"] as string == "<foo />")
                .WithInnerException<XmlException>();
        }

        [TestMethod]
        public void AcsCommand_Run_ResponseIncludedInException()
        {
            string payload =
                "<saml2p:Response xmlns:saml2p=\"urn:oasis:names:tc:SAML:2.0:protocol\" "
                + "xmlns:saml2=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\""
                + MethodBase.GetCurrentMethod().Name + "\" Version=\"2.0\" />";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { encoded })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            Action a = () => new AcsCommand().Run(r, Options.FromConfiguration);

            a.Should().Throw<Exception>()
                .And.Data["Saml2Response"].Should().Be(payload);
        }

        [TestMethod]
        public void AcsCommand_Run_HandlesXmlExceptionWhenUnbindResultIsStillNull()
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("Not Xml"));

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { encoded })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            Action a = () => new AcsCommand().Run(r, Options.FromConfiguration);

            a.Should().Throw<BadFormatSamlResponseException>();
        }

        [TestMethod]
        public void AcsCommand_Run_HandlesExceptionWhenUnbindResultIsStillNull()
        {
            var issuer = new EntityId("http://bad.idp.example.com");
            var artifact = Saml2ArtifactBinding.CreateArtifact(issuer, 0);

            // Just spoil it to force an exception.
            artifact[3] = 5;

            var artifactString = Convert.ToBase64String(artifact);

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLart", new string[] { artifactString })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            Action a = () => new AcsCommand().Run(r, Options.FromConfiguration);

            // The real exception was masked by a NullRef in the exception
            // handler in AcsCommand.Run
            a.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void AcsCommand_Run_SuccessfulResult()
        {
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

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            var ids = new ClaimsIdentity[] { new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            var expected = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri("https://localhost/returnUrl"),
            };

            var options = StubFactory.CreateOptions();

            new AcsCommand().Run(r, options)
                .Should().BeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void AcsCommand_Run_WithReturnUrl_SuccessfulResult()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" InResponseTo = ""InResponseToId"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var responseFormValue = Convert.ToBase64String
                (Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)));
            var relayStateFormValue = "rs1234";

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { responseFormValue }),
                    new KeyValuePair<string, IEnumerable<string>>("RelayState", new string[] { relayStateFormValue })
                },
                new StoredRequestState(
                    new EntityId("https://idp.example.com"),
                    new Uri("http://localhost/testUrl.aspx"),
                    new Saml2Id("InResponseToId"),
                    null)
                );

            var ids = new ClaimsIdentity[] { new ClaimsIdentity("Federation") };
            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            var expected = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri("http://localhost/testUrl.aspx"),
                ClearCookieName = StoredRequestState.CookieNameBase + relayStateFormValue
            };

            new AcsCommand().Run(r, StubFactory.CreateOptions())
                .Should().BeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void AcsCommand_Run_WithReturnUrl_SuccessfulResult_NoConfigReturnUrl()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + MethodBase.GetCurrentMethod().Name + @""" InResponseTo = ""InResponseToId"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var responseFormValue = Convert.ToBase64String
                (Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)));
            var relayStateFormValue = "rs1234";

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { responseFormValue }),
                    new KeyValuePair<string, IEnumerable<string>>("RelayState", new string[] { relayStateFormValue })
                },
                new StoredRequestState(
                    new EntityId("https://idp.example.com"),
                    new Uri("http://localhost/testUrl.aspx"),
                    new Saml2Id("InResponseToId"),
                    null)
                );

            var options = StubFactory.CreateOptions();
            options.SPOptions.ReturnUrl = null;

            new AcsCommand().Invoking(c => c.Run(r, options))
                .Should().NotThrow();
        }

        [TestMethod]
        public void AcsCommand_Run_UnsolicitedResponse_ThrowsOnNoConfiguredReturnUrl()
        {
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
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion""
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

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null);

            var options = StubFactory.CreateOptions();
            options.SPOptions.ReturnUrl = null;

            new AcsCommand().Invoking(a => a.Run(r, options))
                .Should().Throw<ConfigurationErrorsException>().WithMessage(AcsCommand.UnsolicitedMissingReturnUrlMessage);
        }

        [TestMethod]
        public void AcsCommand_Run_Response_ThrowsOnNoStoredNorConfiguredReturnUrl()
        {
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
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion""
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

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                new StoredRequestState(new EntityId("https://idp.example.com"), null, new Saml2Id("InResponseToId"), null));

            var options = StubFactory.CreateOptions();
            options.SPOptions.ReturnUrl = null;

            new AcsCommand().Invoking(a => a.Run(r, options))
                .Should().Throw<ConfigurationErrorsException>().WithMessage(AcsCommand.SpInitiatedMissingReturnUrl);
        }

        [TestMethod]
        public void AcsCommand_Run_UsesBindingFromNotification()
        {
            var options = StubFactory.CreateOptions();
            options.Notifications.GetBinding = r => new StubSaml2Binding();

            var subject = new AcsCommand();
            subject.Invoking(s => s.Run(new HttpRequestData("GET", new Uri("http://host")), options))
                .Should().Throw<NotImplementedException>()
                .WithMessage("StubSaml2Binding.*");
        }

        [TestMethod]
        public void AcsCommand_Run_CallsNotifications()
        {
            var messageId = MethodBase.GetCurrentMethod().Name;
            var response =
             @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + messageId + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + messageId + @"_Assertion""
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

            var requestData = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                null);

            var options = StubFactory.CreateOptions();

            var responseUnboundCalled = false;
            options.Notifications.MessageUnbound = ur =>
            {
                ur.Should().NotBeNull();
                responseUnboundCalled = true;
            };

            CommandResult notifiedCommandResult = null;
            options.Notifications.AcsCommandResultCreated = (cr, r) =>
            {
                notifiedCommandResult = cr;
                r.Id.Value.Should().Be(messageId);
            };

            var getIdentityProviderCalled = false;
            options.Notifications.GetIdentityProvider = (idpEntityId, relayData, opt) =>
            {
                idpEntityId.Id.Should().Be("https://idp.example.com");
                relayData.Should().BeNull();
                getIdentityProviderCalled = true;
                return opt.IdentityProviders[idpEntityId];
            };

            new AcsCommand().Run(requestData, options)
                .Should().BeSameAs(notifiedCommandResult);

            responseUnboundCalled.Should().BeTrue("the ResponseUnbound notification should have been called.");
            getIdentityProviderCalled.Should().BeTrue("the GetIdentityProvider notification should have been called.");
        }

        [TestMethod]
        public void AcsCommand_Run_ExtractsSessionNotOnOrAfter()
        {
            var messageId = MethodBase.GetCurrentMethod().Name;
            var response =
             $@"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""{messageId}"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""{messageId}_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AuthnStatement AuthnInstant=""{DateTime.UtcNow.ToSaml2DateTimeString()}"" SessionNotOnOrAfter = ""2200-01-01T00:00:00Z"">
                        <saml2:AuthnContext>
                            <saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml2:AuthnContextClassRef>
                        </saml2:AuthnContext>
                    </saml2:AuthnStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var formValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                SignedXmlHelper.SignXml(response)));

            var requestData = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                null);

            var options = StubFactory.CreateOptions();

            var subject = new AcsCommand();
            var actual = subject.Run(requestData, options);

            actual.SessionNotOnOrAfter.Should().Be(new DateTime(2200, 01, 01, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void AcsCommand_Run_SessionNotOnOrAfterNullIfNotSpecifiedInResponse()
        {
            var messageId = MethodBase.GetCurrentMethod().Name;
            var response =
             $@"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""{messageId}"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""{messageId}_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AuthnStatement AuthnInstant=""{DateTime.UtcNow.ToSaml2DateTimeString()}"">
                        <saml2:AuthnContext>
                            <saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml2:AuthnContextClassRef>
                        </saml2:AuthnContext>
                    </saml2:AuthnStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var formValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                SignedXmlHelper.SignXml(response)));

            var requestData = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                null);

            var options = StubFactory.CreateOptions();

            var subject = new AcsCommand();
            var actual = subject.Run(requestData, options);

            actual.SessionNotOnOrAfter.Should().NotHaveValue();
        }

        [TestMethod]
        public void AcsCommand_Run_UsesIdpFromNotification()
        {
            var messageId = MethodBase.GetCurrentMethod().Name;
            var response =
             $@"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = ""{messageId}"" Version=""2.0"" InResponseTo=""InResponseToID"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://other.idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""{messageId}_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://other.idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AuthnStatement AuthnInstant=""{DateTime.UtcNow.ToSaml2DateTimeString()}"">
                        <saml2:AuthnContext>
                            <saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml2:AuthnContextClassRef>
                        </saml2:AuthnContext>
                    </saml2:AuthnStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var formValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                SignedXmlHelper.SignXml(response)));

            var relayData = new Dictionary<string, string>
            {
                { "key", "value" }
            };

            var requestData = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { formValue })
                },
                new StoredRequestState(
                    new EntityId("https://other.idp.example.com"),
                    new Uri("http://localhost/testUrl.aspx"),
                    new Saml2Id("InResponseToID"),
                    relayData));

            var options = StubFactory.CreateOptions();

            options.Notifications.GetIdentityProvider = (idpEntityId, rd, opt) =>
            {
                idpEntityId.Id.Should().Be("https://other.idp.example.com");
                rd["key"].Should().Be("value");

                var idp = new IdentityProvider(new EntityId("https://other.idp.example.com"), options.SPOptions);

                idp.SigningKeys.AddConfiguredKey(SignedXmlHelper.TestCert);

                return idp;
            };

            var subject = new AcsCommand();
            var actual = subject.Run(requestData, options);

            actual.Principal.Claims.First().Issuer.Should().Be("https://other.idp.example.com");
        }

        private void RelayStateAsReturnUrl(string relayState, IOptions options, [CallerMemberName] string caller = null)
        {
            if(string.IsNullOrEmpty(caller))
            {
                throw new ArgumentNullException(nameof(caller));
            }

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                ID = """ + caller + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp5.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + caller + @"_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp5.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var responseFormValue = Convert.ToBase64String
                (Encoding.UTF8.GetBytes(SignedXmlHelper.SignXml(response)));

            var formData = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] { responseFormValue }),
            };
            if(relayState != null)
            {
                formData.Add(new KeyValuePair<string, IEnumerable<string>>("RelayState", new string[] { relayState }));
            }

            var r = new HttpRequestData(
                "POST",
                new Uri("http://localhost"),
                "/ModulePath",
                formData,
                null);

            var ids = new ClaimsIdentity[] { new ClaimsIdentity("Federation") };

            ids[0].AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp5.example.com"));

            var expected = new CommandResult()
            {
                Principal = new ClaimsPrincipal(ids),
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = relayState != null ? new Uri(relayState, UriKind.RelativeOrAbsolute) : null,
            };

            new AcsCommand().Run(r, options)
                .Location.OriginalString.Should().Be(relayState);
        }

        [TestMethod]
        public void AcsCommand_Run_WithRelayStateUsedAsReturnUrl_Success()
        {
            RelayStateAsReturnUrl("/someUrl", StubFactory.CreateOptions());
        }

        [TestMethod]
        public void AcsCommand_Run_WithRelayStateUsedAsReturnUrl_Missing()
        {
            this.Invoking(t => t.RelayStateAsReturnUrl(null, StubFactory.CreateOptions()))
                .Should().Throw<ConfigurationErrorsException>();
        }

        [TestMethod]
        public void AcsCommand_Run_WithRelayStateUserAsReturnUrl_AbsolutUrlThrows()
        {
            this.Invoking(t => t.RelayStateAsReturnUrl("https://absolute.example.com/something", StubFactory.CreateOptions()))
                .Should().Throw<InvalidOperationException>().WithMessage("*relative*");
        }

        [TestMethod]
        public void AcsCommand_Run_WithRelayStateUserAsReturnUrl_AbsolutUrlValidatesThroughNotification()
        {
            var options = StubFactory.CreateOptions();

            bool called = false;
            options.Notifications.ValidateAbsoluteReturnUrl = url =>
            {
                called = true;
                return true;
            };

            // Should not throw this time.
            RelayStateAsReturnUrl("https://absolute.example.com/something", options);

            called.Should().BeTrue("Notifaction should have been called");
        }
    }
}
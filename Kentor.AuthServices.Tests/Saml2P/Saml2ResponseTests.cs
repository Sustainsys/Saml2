using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.IO;
using Kentor.AuthServices.Internal;
using Kentor.AuthServices.Saml2P;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Kentor.AuthServices.Tests.Saml2P
{
    [TestClass]
    public class Saml2ResponseTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            // Clean up after tests that globally activate SHA256 support. There
            // is no official API for removing signature algorithms, so let's
            // do some reflection.

            var internalSyncObject = typeof(CryptoConfig)
                .GetProperty("InternalSyncObject", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            lock (internalSyncObject)
            {
                var appNameHT = (IDictionary<string, Type>)typeof(CryptoConfig)
                    .GetField("appNameHT", BindingFlags.Static | BindingFlags.NonPublic)
                    .GetValue(null);

                appNameHT.Remove("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
            }
        }

        [TestMethod]
        public void Saml2Response_Read_BasicParams()
        {
            string response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = ""InResponseToId""
            Destination=""http://destination.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                    <saml2p:StatusMessage>Unable to encrypt assertion</saml2p:StatusMessage>
                </saml2p:Status>
            </saml2p:Response>";

            var expected = new
            {
                Id = new Saml2Id(MethodBase.GetCurrentMethod().Name),
                IssueInstant = new DateTime(2013, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                Status = Saml2StatusCode.Requester,
                StatusMessage = "Unable to encrypt assertion",
                Issuer = new EntityId(null),
                DestinationUrl = new Uri("http://destination.example.com"),
                MessageName = "SAMLResponse",
                InResponseTo = new Saml2Id("InResponseToId"),
                RequestState = (StoredRequestState)null,
            };

            Saml2Response.Read(response).ShouldBeEquivalentTo(expected,
                opt => opt.Excluding(s => s.XmlDocument));
        }

        [TestMethod]
        public void Saml2Response_Read_ThrowsOnNonXml()
        {
            Action a = () => Saml2Response.Read("not xml");

            a.ShouldThrow<XmlException>();
        }

        [TestMethod]
        public void Saml2Response_Read_ThrowsWrongRootNodeName()
        {
            Action a = () => Saml2Response.Read("<saml2p:NotResponse xmlns:saml2p=\"urn:oasis:names:tc:SAML:2.0:protocol\" />");

            a.ShouldThrow<XmlException>()
                .WithMessage("Expected a SAML2 assertion document");
        }

        [TestMethod]
        public void Saml2Response_Read_ThrowsWrongRootNamespace()
        {
            Action a = () => Saml2Response.Read("<saml2p:Response xmlns:saml2p=\"something\" /> ");
            a.ShouldThrow<XmlException>()
                .WithMessage("Expected a SAML2 assertion document");
        }

        [TestMethod]
        public void Saml2Response_Read_ThrowsOnWrongVersion()
        {
            Action a = () => Saml2Response.Read("<saml2p:Response xmlns:saml2p=\""
                + Saml2Namespaces.Saml2P + "\" Version=\"wrong\" />");

            a.ShouldThrow<XmlException>()
                .WithMessage("Wrong or unsupported SAML2 version");

        }

        [TestMethod]
        public void Saml2Response_Read_Issuer()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
            <saml2:Issuer>
                https://some.issuer.example.com
            </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            Saml2Response.Read(response).Issuer.Id.Should().Be("https://some.issuer.example.com");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowOnMissingSignatureInResponseAndAnyAssertion()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
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

            Action a = () => Saml2Response.Read(response).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("The SAML Response is not signed and contains unsigned Assertions. Response cannot be trusted.");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedResponseMessage()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
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

            var signedResponse = SignedXmlHelper.SignXml(response);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedResponseMessageSecondaryKey()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://twokeys.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://twokeys.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            var options = StubFactory.CreateOptions();

            var idp = new IdentityProvider(
                new EntityId("https://twokeys.example.com"), options.SPOptions)
            {
                AllowUnsolicitedAuthnResponse = true
            };

            idp.SigningKeys.AddConfiguredItem(SignedXmlHelper.TestKey2);
            idp.SigningKeys.AddConfiguredItem(SignedXmlHelper.TestKey);

            options.IdentityProviders.Add(idp);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(options);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedResponseMessage_WithAuthnStatement()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AuthnStatement AuthnInstant=""2013-09-25T00:00:00Z"" SessionIndex=""" + MethodBase.GetCurrentMethod().Name + @""" >
                        <saml2:AuthnContext>
                            <saml2:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml2:AuthnContextClassRef>
                            <saml2:AuthnContextDeclRef>http://custom/password/form/consumer</saml2:AuthnContextDeclRef>
                        </saml2:AuthnContext>
                    </saml2:AuthnStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedSingleAssertionInResponseMessage()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";


            var signedAssertion = SignedXmlHelper.SignXml(assertion);
            var signedResponse = string.Format(response, signedAssertion);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedSingleAssertion_WithKeyInfo_InResponseMessage()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion = SignedXmlHelper.SignXml(assertion, true, false);
            var signedResponse = string.Format(response, signedAssertion);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedMultipleAssertionInResponseMessage()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
                {1}
            </saml2p:Response>";

            var assertion1 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var assertion2 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";


            var signedAssertion1 = SignedXmlHelper.SignXml(assertion1);
            var signedAssertion2 = SignedXmlHelper.SignXml(assertion2);
            var signedResponse = string.Format(response, signedAssertion1, signedAssertion2);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectSignedMultipleAssertion_WithKeyInfo_InResponseMessage()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
                {1}
            </saml2p:Response>";

            var assertion1 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var assertion2 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion1 = SignedXmlHelper.SignXml(assertion1, true, false);
            var signedAssertion2 = SignedXmlHelper.SignXml(assertion2, true, false);
            var signedResponse = string.Format(response, signedAssertion1, signedAssertion2);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnMultipleAssertionInUnsignedResponseMessageButNotAllSigned()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
                {1}
            </saml2p:Response>";

            var assertion1 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var assertion2 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";


            var signedAssertion1 = SignedXmlHelper.SignXml(assertion1);
            var signedResponse = string.Format(response, signedAssertion1, assertion2);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("The SAML Response is not signed and contains unsigned Assertions. Response cannot be trusted.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnTamperedAssertionWithMessageSignature()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
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

            var signedResponse = SignedXmlHelper.SignXml(response).Replace("SomeUser", "SomeOtherUser");

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnTamperedAssertionWithAssertionSignature()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
                {1}
            </saml2p:Response>";

            var assertion1 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var assertion2 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion1 = SignedXmlHelper.SignXml(assertion1);
            var signedAssertion2 = SignedXmlHelper.SignXml(assertion2).Replace("SomeUser2", "SomeOtherUser");
            var signedResponse = string.Format(response, signedAssertion1, signedAssertion2);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnAssertionInjectionWithAssertionSignature()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
                {1}
            </saml2p:Response>";

            var assertion1 = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var assertionToInject = @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser2</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion1 = SignedXmlHelper.SignXml(assertion1);

            var signedAssertion1Doc = new XmlDocument { PreserveWhitespace = true };
            signedAssertion1Doc.LoadXml(signedAssertion1);

            var signatureToCopy = signedAssertion1Doc.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];

            var assertionToInjectDoc = new XmlDocument { PreserveWhitespace = true };
            assertionToInjectDoc.LoadXml(assertionToInject);

            assertionToInjectDoc.DocumentElement.AppendChild(assertionToInjectDoc.ImportNode(signatureToCopy, true));

            var signedAssertionToInject = assertionToInjectDoc.OuterXml;

            var signedResponse = string.Format(response, signedAssertion1, signedAssertionToInject);

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Incorrect reference on Xml signature. The reference must be to the root element of the element containing the signature.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnDualReferencesInSignature()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = (RSACryptoServiceProvider)SignedXmlHelper.TestCert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var ref1 = new Reference { Uri = "#" + MethodBase.GetCurrentMethod().Name };
            ref1.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            ref1.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(ref1);

            var ref2 = new Reference { Uri = "#" + MethodBase.GetCurrentMethod().Name };
            ref2.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            ref2.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(ref2);

            signedXml.ComputeSignature();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signedXml.GetXml(), true));

            Action a = () => Saml2Response.Read(xmlDoc.OuterXml).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Multiple references for Xml signatures are not allowed.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnIncorrectTransformsInSignature()
        {
            // SAML2 Core 5.4.4 states that signatures SHOULD NOT contain other transforms than
            // the enveloped signature or exclusive canonicalization transforms and that a verifier
            // of a signature MAY reject signatures with other transforms. We'll reject them to
            // mitigate the risk of transforms opening up for assertion injections.

            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = (RSACryptoServiceProvider)SignedXmlHelper.TestCert.PrivateKey;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            var reference = new Reference { Uri = "#" + MethodBase.GetCurrentMethod().Name };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform()); // The allowed transform is XmlDsigExcC14NTransform
            signedXml.AddReference(reference);

            signedXml.ComputeSignature();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signedXml.GetXml(), true));

            Action a = () => Saml2Response.Read(xmlDoc.OuterXml).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Transform \"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" found in XML signature is not allowed in SAML.");
        }

        [TestMethod]
        public void Saml2Response_Validate_ThrowsOnMissingReferenceInSignature()
        {
            var signedWithoutReference = @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"" ID=""Saml2Response_Validate_FalseOnMissingReference"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""><saml2:Issuer>https://idp.example.com</saml2:Issuer><saml2p:Status><saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" /></saml2p:Status><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315"" /><SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1"" /></SignedInfo><SignatureValue>tYFIoYmrzmp3H7TXm9IS8DW3buBZIb6sI2ycrn+AOnVcdYnPTJpk3ntHlqQKXNEyXgXZNdqEuFpgI1I0P0TlhM+C3rBJnflkApkxZkak5RwnJzDWTHpsSDjYcm+/XgBy3JVZJuMWb2YPaV8GB6cjBMDrENUEaoKRg+FpzPUZO1EOMcqbocXp5cHie1CkPnD1OtT/cuzMBUMpBGZMxjZwdFpOO7R3CUXh/McxKfoGUQGC3DVpt5T8uGkpj4KqZVPS/qTCRhbPRDjg73BdWbdkFpFWge8G/FgkYxr9LBE1TsrxptppO9xoA5jXwJVZaWndSMvo6TuOjUgqY2w5RTkqhA==</SignatureValue></Signature></saml2p:Response>";

            var samlResponse = Saml2Response.Read(signedWithoutReference);

            Action a = () => samlResponse.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("No reference found in Xml signature, it doesn't validate the Xml data.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ReturnsExistingResultOnSecondGetClaimsCall()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            var samlResponse = Saml2Response.Read(signedResponse);

            Action a = () => samlResponse.GetClaims(Options.FromConfiguration);

            a.ShouldNotThrow();
            a.ShouldNotThrow();
        }

        [NotReRunnable]
        [TestMethod]
        public void Saml2Response_GetClaims_CorrectEncryptedSingleAssertion_SignedResponse()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";
                
            var assertion =
            @"<saml2:Assertion Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>UserIDInsideEncryptedAssertion</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(assertion);
            var signedResponse = SignedXmlHelper.SignXml(string.Format(response, encryptedAssertion));

            // Using options factory in this test to get code coverage on ServiceCertificate setter.
            var options = StubFactory.CreateOptions();
            options.SPOptions.ServiceCertificate = SignedXmlHelper.TestCert2;

            var claims = Saml2Response.Read(signedResponse).GetClaims(options);
            claims.Count().Should().Be(1);
            claims.First().FindFirst(ClaimTypes.NameIdentifier).Value.Should().Be("UserIDInsideEncryptedAssertion");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectEncryptedSingleAssertion_SignedAssertion()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion = SignedXmlHelper.SignXml(assertion);
            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(signedAssertion);
            var responseWithAssertion = string.Format(response, encryptedAssertion);

            var claims = Saml2Response.Read(responseWithAssertion).GetClaims(Options.FromConfiguration);
            claims.Count().Should().Be(1);
            claims.First().FindFirst(ClaimTypes.NameIdentifier).Value.Should().Be("SomeUser");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectEncryptedSingleAssertion_OAEP()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion = SignedXmlHelper.SignXml(assertion);
            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(signedAssertion, useOaep: true);
            var responseWithAssertion = string.Format(response, encryptedAssertion);

            var claims = Saml2Response.Read(responseWithAssertion).GetClaims(Options.FromConfiguration);
            claims.Count().Should().Be(1);
            claims.First().FindFirst(ClaimTypes.NameIdentifier).Value.Should().Be("SomeUser");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_CorrectEncryptedSingleAssertion_UsingWIF()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion = new Saml2Assertion(new Saml2NameIdentifier("https://idp.example.com"));
            assertion.Subject = new Saml2Subject(new Saml2NameIdentifier("WIFUser"));
            assertion.Subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:bearer")));
            assertion.Conditions = new Saml2Conditions { NotOnOrAfter = new DateTime(2100, 1, 1) };

            var token = new Saml2SecurityToken(assertion);
            var handler = new Saml2SecurityTokenHandler();

            assertion.SigningCredentials = new X509SigningCredentials(SignedXmlHelper.TestCert,
                signatureAlgorithm: SecurityAlgorithms.RsaSha1Signature, 
                digestAlgorithm: SecurityAlgorithms.Sha1Digest);

            assertion.EncryptingCredentials = new EncryptedKeyEncryptingCredentials(
                SignedXmlHelper.TestCert2,
                keyWrappingAlgorithm: SecurityAlgorithms.RsaOaepKeyWrap,
                keySizeInBits: 256,
                encryptionAlgorithm: SecurityAlgorithms.Aes192Encryption);

            string assertionXml = String.Empty;
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    handler.WriteToken(xw, token);
                }
                assertionXml = sw.ToString();
            }
            var responseWithAssertion = string.Format(response, assertionXml);

            var claims = Saml2Response.Read(responseWithAssertion).GetClaims(Options.FromConfiguration);
            claims.Count().Should().Be(1);
            claims.First().FindFirst(ClaimTypes.NameIdentifier).Value.Should().Be("WIFUser");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_ThrowsOnEncryptedAssertionWithoutSignature()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(assertion);
            var responseWithAssertion = string.Format(response, encryptedAssertion);

            Action a = () => Saml2Response.Read(responseWithAssertion).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("The SAML Response is not signed and contains unsigned Assertions. Response cannot be trusted.");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_ThrowsOnTamperedSignatureInEncryptedAssertion()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var signedAssertion = SignedXmlHelper.SignXml(assertion);
            var tamperedAssertion = signedAssertion.Replace("SomeUser", "AnotherUser");
            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(tamperedAssertion);
            var responseWithAssertion = string.Format(response, encryptedAssertion);

            Action a = () => Saml2Response.Read(responseWithAssertion).GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");
        }

        [NotReRunnable]
        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnEncryptedAssertionAndNoServiceCert()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>UserIDInsideEncryptedAssertion</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(assertion);
            var signedResponse = SignedXmlHelper.SignXml(string.Format(response, encryptedAssertion));

            var options = Options.FromConfiguration;
            options.SPOptions.ServiceCertificate = null;

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(options);
            a.ShouldThrow<Saml2ResponseFailedValidationException>();
        }

        [NotReRunnable]
        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnEncryptedAssertionAndNoPrivateKey()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                {0}
            </saml2p:Response>";

            var assertion =
            @"<saml2:Assertion Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"" xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>UserIDInsideEncryptedAssertion</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>";

            var encryptedAssertion = SignedXmlHelper.EncryptAssertion(assertion);
            var signedResponse = SignedXmlHelper.SignXml(string.Format(response, encryptedAssertion));

            var options = Options.FromConfiguration;
            options.SPOptions.ServiceCertificate = new X509Certificate2(SignedXmlHelper.TestCert2.Export(X509ContentType.Cert));

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(options);
            a.ShouldThrow<Saml2ResponseFailedValidationException>();
        }


        [NotReRunnable]
        [TestMethod]
        public void Saml2Response_GetClaims_CreateIdentities()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeOtherUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            var c1 = new ClaimsIdentity("Federation");
            c1.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            var c2 = new ClaimsIdentity("Federation");
            c2.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeOtherUser", null, "https://idp.example.com"));

            var expected = new ClaimsIdentity[] { c1, c2 };

            var r = Saml2Response.Read(SignedXmlHelper.SignXml(response));

            r.GetClaims(StubFactory.CreateOptions())
                .ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_SavesBootstrapContext()
        {
            var assertion =
            @"<saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""" + MethodBase.GetCurrentMethod().Name + @"_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2:Subject>
                    <saml2:NameID>SomeUser</saml2:NameID>
                    <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                </saml2:Subject>
                <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
            </saml2:Assertion>";

            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>"
            + assertion +
            "</saml2p:Response>";

            var options = StubFactory.CreateOptions();

            options.SPOptions.Saml2PSecurityTokenHandler.Configuration.SaveBootstrapContext = true;

            var expected = options.SPOptions.Saml2PSecurityTokenHandler.ReadToken(XmlReader.Create(new StringReader(assertion)));

            var r = Saml2Response.Read(SignedXmlHelper.SignXml(response));

            var subject = r.GetClaims(options).Single().BootstrapContext;

            subject.As<BootstrapContext>().SecurityToken.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2Response_GetRequestState_ThrowsOnResponseNotValid()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
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
                </saml2:Assertion>
            </saml2p:Response>";

            response = SignedXmlHelper.SignXml(response);
            response = response.Replace("2013-09-25", "2013-09-26");

            var r = Saml2Response.Read(response);

            Action a = () => r.GetRequestState(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");

            // Test that it throws again on subsequent calls.
            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");
        }

        [NotReRunnable]
        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnWrongAudience()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
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
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" >
                        <saml2:AudienceRestriction>
                            <saml2:Audience>https://example.com/wrong/audience</saml2:Audience>
                        </saml2:AudienceRestriction>
                    </saml2:Conditions>
                </saml2:Assertion>
            </saml2p:Response>";

            response = SignedXmlHelper.SignXml(response);

            var r = Saml2Response.Read(response);

            Action a = () => r.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<AudienceUriValidationFailedException>();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnExpired()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
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
                    <saml2:Conditions NotOnOrAfter=""2013-06-30T08:00:00Z"" />
                </saml2:Assertion>
            </saml2p:Response>";

            response = SignedXmlHelper.SignXml(response);
            var r = Saml2Response.Read(response);

            Action a = () => r.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<SecurityTokenExpiredException>();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_CorrectInResponseTo()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = """ + request.Id + @""">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            Action a = () => response.GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_FalseOnMissingInResponseTo_IfDisallowed()
        {
            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp2.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            Action a = () => response.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Unsolicited responses are not allowed for idp \"https://idp2.example.com\".");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_MissingInResponseTo_IfAllowed()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            Action a = () => response.GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnIncorrectInResponseTo()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = ""anothervalue"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            Action a = () => response.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Replayed or unknown InResponseTo \"anothervalue\".");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnReplayedInResponseTo()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = """ + request.Id + @""">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            Action a = () =>
            {
                var response = Saml2Response.Read(responseXML);
                response.GetClaims(Options.FromConfiguration);
            };

            a.ShouldNotThrow();
            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Replayed or unknown InResponseTo \"" + request.Id + "\".");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnTamperedMessage()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = """ + request.Id + @""">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);
            responseXML = responseXML.Replace("2013-01-01", "2015-01-01"); // Break signature.

            var response = Saml2Response.Read(responseXML);

            Action a = () =>
            {
                response.GetClaims(Options.FromConfiguration);
            };

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");

            // With an incorrect signature, a signature validation should be
            // thrown - even if we response is validate twice. In case
            // GetClaims/Validate doesn't cache the result it will instead
            // report a replay exception the second time because the replay
            // detection is done before the signature validation.

            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnResponseFromWrongIdp()
        {
            // A valid response is received, but it is not from the idp that we
            // did send the AuthnRequest to.
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(null, StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = """ + request.Id + @""">
                <saml2:Issuer>https://idp.anotheridp.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            Action a = () => response.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<Saml2ResponseFailedValidationException>().And
                .Message.Should().Be("Expected response from idp \"https://idp.example.com\" but received response from idp \"https://idp.anotheridp.com\".");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_ThrowsOnReplayAssertionId()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
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

            response = SignedXmlHelper.SignXml(response);
            var r1 = Saml2Response.Read(response);
            r1.GetClaims(Options.FromConfiguration);

            var r2 = Saml2Response.Read(response);

            Action a = () => r2.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<SecurityTokenReplayDetectedException>();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnEmptyOptions()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
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

            var xml = SignedXmlHelper.SignXml(response);

            var subject = Saml2Response.Read(xml);

            Action a = () => subject.GetClaims(null);
            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnStatusFailure()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
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

            var xml = SignedXmlHelper.SignXml(response);

            var subject = Saml2Response.Read(xml);

            Action a = () => subject.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<InvalidOperationException>()
                .WithMessage("The Saml2Response must have status success to extract claims. Status: Requester.");

        }

        [TestMethod]
        public void Saml2Response_DisplayStatusMessageInExceptionText()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                    <saml2p:StatusMessage>A status message</saml2p:StatusMessage>
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

            var xml = SignedXmlHelper.SignXml(response);

            var subject = Saml2Response.Read(xml);

            Action a = () => subject.GetClaims(Options.FromConfiguration);

            a.ShouldThrow<InvalidOperationException>()
                .WithMessage("The Saml2Response must have status success to extract claims. Status: Requester. Message: A status message.");

        }

        [TestMethod]
        public void Saml2Response_Ctor_FromData()
        {
            var issuer = new EntityId("http://idp.example.com");
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe") 
            });
            var response = new Saml2Response(issuer, null, null, null, identity);

            response.Issuer.Should().Be(issuer);
            response.GetClaims(Options.FromConfiguration)
                .Single()
                .ShouldBeEquivalentTo(identity);
        }

        [TestMethod]
        public void Saml2Response_Xml_FromData_ContainsBasicData()
        {
            var issuer = new EntityId("http://idp.example.com");
            var nameId = "JohnDoe";
            var destination = "http://destination.example.com/";

            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, nameId) 
            });

            // Grab current time both before and after generating the response
            // to avoid heisenbugs if the second counter is updated while creating
            // the response.
            string before = DateTime.UtcNow.ToSaml2DateTimeString();
            var response = new Saml2Response(issuer, SignedXmlHelper.TestCert,
                new Uri(destination), null, identity);
            string after = DateTime.UtcNow.ToSaml2DateTimeString();

            var xml = response.XmlDocument;

            xml.FirstChild.OuterXml.Should().StartWith("<?xml version=\"1.0\"");
            xml.DocumentElement.LocalName.Should().Be("Response");
            xml.DocumentElement.NamespaceURI.Should().Be(Saml2Namespaces.Saml2PName);
            xml.DocumentElement.Prefix.Should().Be("saml2p");
            xml.DocumentElement["Issuer", Saml2Namespaces.Saml2Name].InnerText.Should().Be(issuer.Id);
            xml.DocumentElement["Assertion", Saml2Namespaces.Saml2Name]
                ["Subject", Saml2Namespaces.Saml2Name]["NameID", Saml2Namespaces.Saml2Name]
                .InnerText.Should().Be(nameId);
            xml.DocumentElement.GetAttribute("Destination").Should().Be(destination);
            xml.DocumentElement.GetAttribute("ID").Should().NotBeNullOrWhiteSpace();
            xml.DocumentElement.GetAttribute("Version").Should().Be("2.0");
            xml.DocumentElement.GetAttribute("IssueInstant").Should().Match(
                i => i == before || i == after);
        }

        [TestMethod]
        public void Saml2Response_Xml_FromData_ContainsStatus_Success()
        {
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe") 
            });

            var response = new Saml2Response(new EntityId("issuer"), SignedXmlHelper.TestCert,
                new Uri("http://destination.example.com"), null, identity);

            var xml = response.XmlDocument;

            var subject = xml.DocumentElement["Status", Saml2Namespaces.Saml2PName];

            subject["StatusCode", Saml2Namespaces.Saml2PName].GetAttribute("Value")
                .Should().Be("urn:oasis:names:tc:SAML:2.0:status:Success");
        }

        [TestMethod]
        public void Saml2Response_Xml_FromData_ContainsInResponseTo()
        {
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe") 
            });

            var response = new Saml2Response(new EntityId("issuer"), SignedXmlHelper.TestCert,
                new Uri("http://destination.example.com"), "InResponseToID", identity);

            var xml = response.XmlDocument;

            xml.DocumentElement.GetAttribute("InResponseTo").Should().Be("InResponseToID");
        }

        [TestMethod]
        public void Saml2Response_Xml_FromData_IsSigned()
        {
            var issuer = new EntityId("http://idp.example.com");
            var nameId = "JohnDoe";
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.NameIdentifier, nameId) 
            });

            var response = new Saml2Response(issuer, SignedXmlHelper.TestCert,
                null, null, claimsIdentities: identity);

            var xml = response.XmlDocument;

            var signedXml = new SignedXml(xml);
            var signature = xml.DocumentElement["Signature", SignedXml.XmlDsigNamespaceUrl];
            signedXml.LoadXml(signature);

            signature.Should().NotBeNull();
            signedXml.KeyInfo.Count.Should().Be(1);
            signedXml.CheckSignature(SignedXmlHelper.TestCert, true).Should().BeTrue();
        }

        [TestMethod]
        public void Saml2Response_ToXml()
        {
            string response = @"<?xml version=""1.0"" encoding=""UTF-8""?><saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol"" ID=""Saml2Response_ToXml"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""><saml2p:Status><saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" /></saml2p:Status></saml2p:Response>";

            var subject = Saml2Response.Read(response).ToXml();

            subject.Should().Be(response);
        }

        [TestMethod]
        public void Saml2Response_MessageName()
        {
            var subject = new Saml2Response(new EntityId("issuer"), null, null, null);

            subject.MessageName.Should().Be("SAMLResponse");
        }

        [TestMethod]
        public void Saml2Response_FromRequest_Remembers_ReturnUrl()
        {
            var idp = Options.FromConfiguration.IdentityProviders.Default;

            var request = idp.CreateAuthenticateRequest(new Uri("http://localhost/testUrl.aspx"), StubFactory.CreateAuthServicesUrls());

            var responseXML =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            InResponseTo = """ + request.Id + @""">
                <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            responseXML = SignedXmlHelper.SignXml(responseXML);

            var response = Saml2Response.Read(responseXML);

            response.GetRequestState(Options.FromConfiguration)
                .ReturnUrl.Should().Be("http://localhost/testUrl.aspx");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_ThrowsInformativeExceptionForSha256()
        {
            var signedResponse =
                @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                    ID = """ + MethodBase.GetCurrentMethod().Name + @"""
                    Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2p:Status>
                        <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                    </saml2p:Status>
                    <Assertion ID=""Saml2Response_GetClaims_ThrowsInformativeExceptionForSha256"" IssueInstant=""2015-03-13T20:43:07.330Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""#Saml2Response_GetClaims_ThrowsInformativeExceptionForSha256""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>F+E7u3vqMC07ipvP9AowsMqP7y6CsAC0GeEIxNSwDEI=</DigestValue></Reference></SignedInfo><SignatureValue>GmiXn24Ccnr64TbmDd1/nLM+891z0FtRHSpU8+75uOqbpNK/ZZGrltFf2YZ5u9b9O0HfbFFsZ0i28ocwAZOv2UfxQrCtOGf3ss7Q+t2Zmc6Q/3ES7HIa15I5BbaSdNfpOMlX6N1XXhMprRGy2YWMr5IAIhysFG1A2oHaC3yFiesfUrawN/lXUYuI22Kf4A5bmnIkKijnwX9ewnhRj6569bw+c6q+tVZSHQzI+KMU9KbKN4NsXxAmv6dM1w2qOiX9/CO9LzwEtlhA9yo3sl0uWP8z5GwK9qgOlsF2NdImAQ5f0U4Uv26doFn09W+VExFwNhcXhewQUuPBYBr+XXzdww==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>
                </saml2p:Response>";

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("SHA256 signatures require the algorithm to be registered at the process level. Call Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures() on startup to register.");

        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_ChecksSha256WhenEnabled()
        {
            Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures();

            var signedResponse =
                @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                    ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                        <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                        <saml2p:Status>
                            <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                        </saml2p:Status>
                        <Assertion ID=""" + MethodBase.GetCurrentMethod().Name + @""" IssueInstant=""2015-03-13T20:43:33.466Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""#Saml2Response_GetClaims_ChecksSha256WhenEnabled""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>8s5HDYeicqbNwESGyrvYYXinJeJJgl4t6O27KGE0ejc=</DigestValue></Reference></SignedInfo><SignatureValue>mS2TFErenJHyvUbyIDUItOvH6AavUNGg5zL3hVueWDGjhaft2mlWSlQIFm9ajVQKrZq2Q/V4oZYGTQ8muTfrhdCL3fyu453nEWcNgQ+gm1H1e89N75XWonfL+UQDl73O95SX0dD4DjqQAC4MlSwMOkwOR7GakhjPbSzRct7lFbRx/3k+TUZNj9rfV4uzlf79ebkw9EaaSfu0tR6bAfGyrefFaNTZs2NeRICfD/GKn7HRo9zSdVPBHfEW2UUy0x/aWREG4GgUs7qObWL4uhDZ6oyy5FbsRcrUJMiXCFNXA8dr9EtZ2VafHz3d4kJFLiq63xjqpjGk/ng2gP+47F/9Rw==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>
                    </saml2p:Response>";

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldNotThrow();
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_DoesNotThrowSha256MessageForOtherProblem()
        {
            Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures();

            // Here I've specified an invalid digest algorithm. Want to be sure it
            // does NOT flag this as a problem with the sha256 signature algorithm 
            // (they both throw CryptographicException)
            var signedResponse =
                @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                    ID = """ + MethodBase.GetCurrentMethod().Name + @"""
                    Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2p:Status>
                        <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                    </saml2p:Status>
                    <Assertion ID=""Saml2Response_GetClaims_DoesNotThrowSha256MessageForOtherProblem_Assertion"" IssueInstant=""2015-03-13T20:43:07.330Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#SomeInvalidName"" /><Reference URI=""#Saml2Response_GetClaims_DoesNotThrowSha256MessageForOtherProblem_Assertion""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>F+E7u3vqMC07ipvP9AowsMqP7y6CsAC0GeEIxNSwDEI=</DigestValue></Reference></SignedInfo><SignatureValue>GmiXn24Ccnr64TbmDd1/nLM+891z0FtRHSpU8+75uOqbpNK/ZZGrltFf2YZ5u9b9O0HfbFFsZ0i28ocwAZOv2UfxQrCtOGf3ss7Q+t2Zmc6Q/3ES7HIa15I5BbaSdNfpOMlX6N1XXhMprRGy2YWMr5IAIhysFG1A2oHaC3yFiesfUrawN/lXUYuI22Kf4A5bmnIkKijnwX9ewnhRj6569bw+c6q+tVZSHQzI+KMU9KbKN4NsXxAmv6dM1w2qOiX9/CO9LzwEtlhA9yo3sl0uWP8z5GwK9qgOlsF2NdImAQ5f0U4Uv26doFn09W+VExFwNhcXhewQUuPBYBr+XXzdww==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>
                </saml2p:Response>";

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldThrow<CryptographicException>()
                .WithMessage("SignatureDescription could not be created for the signature algorithm supplied.");
        }

        [TestMethod]
        [NotReRunnable]
        public void Saml2Response_GetClaims_FailsSha256WhenChanged()
        {
            Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures();

            var signedResponse =
                @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                    xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                    ID = """ + MethodBase.GetCurrentMethod().Name + @""" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                        <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                        <saml2p:Status>
                            <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                        </saml2p:Status>
                        <Assertion ID=""" + MethodBase.GetCurrentMethod().Name + @""" IssueInstant=""2015-03-13T20:44:00.791Z"" Version=""2.0"" xmlns=""urn:oasis:names:tc:SAML:2.0:assertion""><Issuer>https://idp.example.com</Issuer><Signature xmlns=""http://www.w3.org/2000/09/xmldsig#""><SignedInfo><CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><Reference URI=""#Saml2Response_GetClaims_FailsSha256WhenChanged""><Transforms><Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></Transforms><DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><DigestValue>BKRyWqweAczLA8fgRcx6zzMDiP0qT0TwqU/X4VgLiXM=</DigestValue></Reference></SignedInfo><SignatureValue>iK8s+MkLlixSSQu5Q/SHRZLhfnj4jlyPLAD6C2n9zmQu4CosZME7mxiNFiWyOE8XRGd+2LJle+NjJrkZFktVb03JaToq7w4Q8GfJ2oUUjNCweoaJ6NzsnwkFoXhyh0dfOixl/Ifa3qDX50/Hv2twF/QXfDs08GZTxZKehKsVDITyVd6nytF8VUb0+nU7UMWPn1XeHM7YNI/1mkVbCRx/ci5ZRxwjAX40xttd4JL6oBnp5oaaMgWpAa2cVb+t/9HhCRThEho1etbPHx/+E9ElL1PhKqKX6nh2GSH1TFJkwEXIPPZKqCs3YDINLBZpLfl626zbV4cGOGyWUAroVsk2uw==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIDIzCCAg+gAwIBAgIQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUtlbnRvci5BdXRoU2VydmljZXMuVGVzdHMwHhcNMTMwOTI1MTMzNTQ0WhcNMzkxMjMxMjM1OTU5WjAkMSIwIAYDVQQDExlLZW50b3IuQXV0aFNlcnZpY2VzLlRlc3RzMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwVGpfvK9N//MnA5Jo1q2liyPR24406Dp25gv7LB3HK4DWgqsb7xXM6KIV/WVOyCV2g/O1ErBlB+HLhVZ4XUJvbqBbgAJqFO+TZwcCIe8u4nTEXeU660FdtkKClA17sbtMrAGdDfOPwVBHSuavdHeD7jHNI4RUDGKnEW13/0EvnHDilIetwODRxrX/+41R24sJThFbMczByS3OAL2dcIxoAynaGeM90gXsVYow1QhJUy21+cictikb7jW4mW6dvFCBrWIceom9J295DcQIHoxJy5NoZwMir/JV00qs1wDVoN20Ve1DC5ImwcG46XPF7efQ44yLh2j5Yexw+xloA81dwIDAQABo1kwVzBVBgNVHQEETjBMgBAWIahoZhXVUogbAqkS7zwfoSYwJDEiMCAGA1UEAxMZS2VudG9yLkF1dGhTZXJ2aWNlcy5UZXN0c4IQg7mOjTf994NAVxZu4jqXpzAJBgUrDgMCHQUAA4IBAQA2aGzmuKw4AYXWMhrGj5+i8vyAoifUn1QVOFsUukEA77CrqhqqaWFoeagfJp/45vlvrfrEwtF0QcWfmO9w1VvHwm7sk1G/cdYyJ71sU+llDsdPZm7LxQvWZYkK+xELcinQpSwt4ExavS+jLcHoOYHYwIZMBn3U8wZw7Kq29oGnoFQz7HLCEl/G9i3QRyvFITNlWTjoScaqMjHTzq6HCMaRsL09DLcY3KB+cedfpC0/MBlzaxZv0DctTulyaDfM9DCYOyokGN/rQ6qkAR0DDm8fVwknbJY7kURXNGoUetulTb5ow8BvD1gncOaYHSD0kbHZG+bLsUZDFatEr2KW8jbG</X509Certificate></X509Data></KeyInfo></Signature><Subject><NameID>SomeUser</NameID><SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" /></Subject><Conditions NotOnOrAfter=""2100-01-01T05:00:00.000Z"" /></Assertion>
                    </saml2p:Response>";

            signedResponse = signedResponse.Replace("SomeUser", "AnotherUser");

            Action a = () => Saml2Response.Read(signedResponse).GetClaims(Options.FromConfiguration);
            a.ShouldThrow<Saml2ResponseFailedValidationException>()
                .WithMessage("Signature validation failed on SAML response or contained assertion.");

        }
    }
}

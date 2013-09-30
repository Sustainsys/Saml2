using System;
using System.Security.Cryptography.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Security.Claims;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2ResponseTests
    {
        [TestMethod]
        public void Saml2Response_Read_BasicParams()
        {
            string response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Read_BasicParams"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var expected = new
            {
                Id = "Saml2Response_Read_BasicParams",
                IssueInstant = new DateTime(2013, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                Status = Saml2StatusCode.Requester,
                Issuer = (string)null
            };

            Saml2Response.Read(response).ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2Response_Read_ThrowsOnNonXml()
        {
            Action a = () => Saml2Response.Read("not xml");

            a.ShouldThrow<XmlException>()
                .WithMessage("Data at the root level is invalid. Line 1, position 1.");
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
            ID = ""Saml2Respons_Read_Issuer"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
            <saml2:Issuer xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion"">
                https://some.issuer.example.com
            </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            Saml2Response.Read(response).Issuer.Should().Be("https://some.issuer.example.com");
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnMissingSignature()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Validates_FalseOnMissingSignature"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            Saml2Response.Read(response).Validate(null).Should().BeFalse();
        }

        [TestMethod]
        public void Saml2Response_Validate_TrueOnCorrectMessage()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Validate_TrueOnCorrectMessage"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            Saml2Response.Read(signedResponse).Validate(SignedXmlHelper.TestCert).Should().BeTrue();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnTamperedMessage()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Validate_FalseOnTamperedMessage"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            signedResponse = signedResponse.Replace("2013-01-01", "2013-01-02");

            Saml2Response.Read(signedResponse).Validate(SignedXmlHelper.TestCert).Should().BeFalse();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_CreateIdentities()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_CreateIdentities"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentities1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                </saml2:Assertion>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentities2""
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
            r.Validate(SignedXmlHelper.TestCert);
            
            r.GetClaims().ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnNotValidated()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_ThrowsOnNotValidated"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_ThrowsOnNotValidated_Assertion""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                </saml2:Assertion>
            </saml2p:Response>";

            Action a = () => Saml2Response.Read(response).GetClaims();

            a.ShouldThrow<InvalidOperationException>()
                .WithMessage("The Saml2Response must be validated first.");
        
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnResponseNotValid()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_ThrowsOnResponseNotValid"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_ThrowsOnResponseNotValid_Assertion""
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
            r.Validate(SignedXmlHelper.TestCert);
            Action a = () => r.GetClaims();

            a.ShouldThrow<InvalidOperationException>()
                .WithMessage("The Saml2Response didn't pass validation");
        }

        [TestMethod]
        [Ignore]
        public void Saml2Response_GetClaims_ThrowsOnWrongAudience()
        {
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnExpired()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_ThrowsOnExpired"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_ThrowsOnExpired_Assertion""
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
            r.Validate(SignedXmlHelper.TestCert);

            Action a = () => r.GetClaims();

            a.ShouldThrow<SecurityTokenExpiredException>();
        }

        [TestMethod]
        [Ignore]
        public void Saml2Response_Validate_FalseOnInvalidInResponseTo()
        {
        }

        [TestMethod]
        [Ignore]
        public void Saml2Response_Validate_FalseOnSecondInResponseTo()
        {
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnReplayAssertionId()
        {
            var response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_ThrowsOnReplayAssertionId"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_ThrowsOnReplayAssertionId_Assertion""
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
            r1.Validate(SignedXmlHelper.TestCert).Should().BeTrue();
            r1.GetClaims();

            var r2 = Saml2Response.Read(response);
            r2.Validate(SignedXmlHelper.TestCert).Should().BeTrue();

            Action a = () => r2.GetClaims();
            
            a.ShouldThrow<SecurityTokenReplayDetectedException>();
        }

        [TestMethod]
        [Ignore]
        public void Saml2Response_Validate_FalseOnIncorrectInReplyTo()
        {
        }
    }
}

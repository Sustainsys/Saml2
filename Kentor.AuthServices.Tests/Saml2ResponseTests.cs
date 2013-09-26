using System;
using System.Security.Cryptography.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2ResponseTests
    {
        [TestMethod]
        public void Saml2Response_Read_BasicParams()
        {
            string response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
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
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Respons_Read_Issuer"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
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
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
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
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Validate_TrueOnCorrectMessage"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var signedResponse = SignXml(response);

            Saml2Response.Read(signedResponse).Validate(testCert).Should().BeTrue();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnTamperedMessage()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_Validate_FalseOnTamperedMessage"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var signedResponse = SignXml(response);

            signedResponse = signedResponse.Replace("2013-01-01", "2013-01-02");

            Saml2Response.Read(signedResponse).Validate(testCert).Should().BeFalse();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_CreateIdentities()
        {
            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""Saml2Response_GetClaims_CreateIdentity"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z""
            Issuer = ""https://some.issuer.example.com"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion1""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                </saml2:Assertion>
                <saml2:Assertion xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion2""
                IssueInstant=""2013-09-25T00:00:00Z"">
                    <saml2:Issuer>https://idp.example.com</saml2:Issuer>
                    <saml2:Subject>
                        <saml2:NameID>SomeOtherUser</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                </saml2:Assertion>            
            </saml2p:Response>";

            var c1 = new ClaimsIdentity("Federation");
            c1.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));
            var c2 = new ClaimsIdentity("Federation");
            c2.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeOtherUser", null, "https://idp.example.com"));

            var expected = new ClaimsIdentity[] { c1, c2 };

            Saml2Response.Read(SignXml(response)).GetClaims()
                .ShouldBeEquivalentTo(expected, opt => opt.IgnoringCyclicReferences());
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnResponseNotValid()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnWrongAudience()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_GetClaims_ThrowsOnExpired()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnInvalidInResponseTo()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnSecondInResponseTo()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnReplay()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Saml2Response_Validate_FalseOnIncorrectInReplyTo()
        {
            throw new NotImplementedException();
        }

        private static readonly X509Certificate2 testCert = new X509Certificate2("Kentor.AuthServices.Tests.pfx");
        private string SignXml(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var signedXml = new SignedXml(xmlDoc);

            signedXml.SigningKey = (RSACryptoServiceProvider)testCert.PrivateKey;

            var reference = new Reference();
            reference.Uri = "";
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(signedXml.GetXml(), true));

            return xmlDoc.OuterXml;
        }
    }
}

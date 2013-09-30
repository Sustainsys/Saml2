using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using NSubstitute;
using System.Collections.Specialized;
using System.Text;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class AcsCommandTests
    {
        [TestMethod]
        public void AcsCommand_Run_ErrorOnNoSamlResponseFound()
        {
            var cr = new AcsCommand().Run(Substitute.For<HttpRequestBase>());

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.NoSamlResponseFound
            };

            cr.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnNotBase64InFormResponse()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", "#¤!2" } });

            var cr = new AcsCommand().Run(r);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.BadFormatSamlResponse
            };

            cr.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AcsCommand_Run_ErrorOnIncorrectXml()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("<foo />"));
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", encoded } });

            var cr = new AcsCommand().Run(r);

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = CommandResultErrorCode.BadFormatSamlResponse
            };

            cr.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void AcsCommand_Run_SuccessfulResult()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");

            var response =
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
                xmlns:saml2=""urn:oasis:names:tc:SAML:2.0:assertion""
            ID = ""AcsCommand_Run_SuccessfulResult"" Version=""2.0"" IssueInstant=""2013-01-01T00:00:00Z"">
                <saml2:Issuer>
                    https://idp.example.com
                </saml2:Issuer>
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Success"" />
                </saml2p:Status>
                <saml2:Assertion
                Version=""2.0"" ID=""Saml2Response_GetClaims_CreateIdentity_Assertion1""
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

            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", formValue } });

            var id = new ClaimsIdentity("Federation");
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, "SomeUser", null, "https://idp.example.com"));

            var expected = new CommandResult()
            {
                Principal = new ClaimsPrincipal(id),
                HttpStatusCode = HttpStatusCode.SeeOther,
                Location = new Uri("http://localhost/LoggedIn")
            };

            new AcsCommand().Run(r).ShouldBeEquivalentTo(expected,
                opt => opt.IgnoringCyclicReferences());
        }
    }
}

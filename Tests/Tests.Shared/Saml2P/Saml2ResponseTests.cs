using Kentor.AuthServices.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Kentor.AuthServices.Saml2P;
using FluentAssertions;
using System.Linq;
using System.Security.Claims;

namespace Tests.Shared.Saml2P
{
    [TestClass]
    public class Saml2ResponseTests
    {
        /// This is a specific test for the vulnerabilities found by Duo in February 2018
        /// https://duo.com/blog/duo-finds-saml-vulnerabilities-affecting-multiple-implementations
        [TestMethod]
        public void Saml2Response_GetClaims_CorrectSignedResponseMessage_CommentInNameId()
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
                        <saml2:NameID>Some<!--Comment-->User</saml2:NameID>
                        <saml2:SubjectConfirmation Method=""urn:oasis:names:tc:SAML:2.0:cm:bearer"" />
                    </saml2:Subject>
                    <saml2:Conditions NotOnOrAfter=""2100-01-01T00:00:00Z"" />
                    <saml2:AttributeStatement>
                        <saml2:Attribute Name=""CommentTest"">
                            <saml2:AttributeValue>Some<!--Comment-->Value</saml2:AttributeValue>
                        </saml2:Attribute>
                    </saml2:AttributeStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            var claims = Saml2Response.Read(signedResponse).GetClaims(StubFactory.CreateOptions());

            claims.Single().FindFirst(ClaimTypes.NameIdentifier).Value.Should().Be("SomeUser");
            claims.Single().FindFirst("CommentTest").Value.Should().Be("SomeValue");
        }

        /// This is a specific test for the vulnerabilities found by Duo in February 2018
        /// https://duo.com/blog/duo-finds-saml-vulnerabilities-affecting-multiple-implementations
        [TestMethod]
        public void Saml2Response_GetClaims_CorrectSignedResponseMessage_CommentInAttributeValue()
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
                    <saml2:AttributeStatement>
                        <saml2:Attribute Name=""CommentTest"">
                            <saml2:AttributeValue>Some<!--Comment-->Value</saml2:AttributeValue>
                        </saml2:Attribute>
                    </saml2:AttributeStatement>
                </saml2:Assertion>
            </saml2p:Response>";

            var signedResponse = SignedXmlHelper.SignXml(response);

            var claims = Saml2Response.Read(signedResponse).GetClaims(StubFactory.CreateOptions());

            claims.Single().FindFirst("CommentTest").Value.Should().Be("SomeValue");
        }
    }
}

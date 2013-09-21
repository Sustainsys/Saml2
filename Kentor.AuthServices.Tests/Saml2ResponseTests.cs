using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2ResponseTests
    {
        [TestMethod]
        public void Saml2Response_Read_Id()
        {
            string response = 
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""id123"" Version=""2.0"" IssueInstant=""2013-01-01T0:0:0Z"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:RequestDenied"" />
                </saml2p:Status>
            </saml2p:Response>";

            Saml2Response.Read(response).Id.Should().Be("id123");
        }
    }
}

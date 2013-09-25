using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using FluentAssertions;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Text;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2PostBindingTests
    {
        private HttpRequestBase CreateRequest(string encodedResponse)
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", encodedResponse} });
            return r;
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ThrowsOnNotBase64Encoded()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(CreateRequest("foo")))
                .ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ReadsSaml2ResponseId()
        {
            string response = 
            @"<saml2p:Response xmlns:saml2p=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID = ""id1"" Version=""2.0"" IssueInstant=""2013-01-01T01:04:01Z"">
                <saml2p:Status>
                    <saml2p:StatusCode Value=""urn:oasis:names:tc:SAML:2.0:status:Requester"" />
                </saml2p:Status>
            </saml2p:Response>";

            var r = CreateRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes(response)));

            Saml2Binding.Get(Saml2BindingType.HttpPost).Unbind(r).Id.Should().Be("id1");
        }
    }
}

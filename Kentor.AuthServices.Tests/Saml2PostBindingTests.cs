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
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", encodedResponse } });
            return r;
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(null))
                .ShouldThrow<ArgumentNullException>().And.Message.Contains("request");
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

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Bind(null))
                .ShouldThrow<ArgumentNullException>("Value cannot be null.\r\nParameter name: message");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind()
        {
            var xmlData = "<root><content>data</content></root>";
            var request = Substitute.For<ISaml2Message>();
            request.ToXml().Returns(xmlData);
            request.DestinationUri.Returns(new Uri("http://www.example.com/acs"));
            request.MessageName.Returns("SAMLMessageName");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(request);

            var expected = new CommandResult()
            {
                Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN""
""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<body onload=""document.forms[0].submit()"">
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""http://www.example.com/acs"" 
method=""post"">
<div>
<input type=""hidden"" name=""SAMLMessageName"" 
value=""PHJvb3Q+PGNvbnRlbnQ+ZGF0YTwvY29udGVudD48L3Jvb3Q+""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
</body>
</html>"
            };

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.CanUnbind(null))
                .ShouldThrow<ArgumentNullException>().And.Message.Contains("request");
        }
    }
}

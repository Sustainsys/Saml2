using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using FluentAssertions;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.Tests.WebSSO;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class Saml2PostBindingTests
    {
        private HttpRequestData CreateRequest(string encodedResponse, string relayState = null)
        {
            var formData = new List<KeyValuePair<string, string[]>>()
            {
                new KeyValuePair<string, string[]>("SAMLResponse", new string[] {encodedResponse })
            };

            if(!string.IsNullOrEmpty(relayState))
            {
                formData.Add(new KeyValuePair<string, string[]>("RelayState", new string[] { relayState }));
            };

            return new HttpRequestData(
                "POST",
                new Uri("http://example.com"),
                "/ModulePath",
                formData);
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ThrowsOnNotBase64Encoded()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(CreateRequest("foo")))
                .ShouldThrow<FormatException>();
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ReadsSaml2Response()
        {
            string response = "responsestring";

            var r = CreateRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes(response)));

            Saml2Binding.Get(Saml2BindingType.HttpPost).Unbind(r).Data.Should().Be(response);
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ReadsRelayState()
        {
            string response = "responsestring";
            string relayState = "someState";

            var r = CreateRequest(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(response)),
                relayState);

            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Unbind(r).RelayState.Should().Be(relayState);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Bind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "<root><content>data</content></root>",
                DestinationUrl = new Uri("http://www.example.com/acs"),
                MessageName = "SAMLMessageName"
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(message);

            var expected = new CommandResult()
            {
                ContentType = "text/html",
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
<form action=""http://www.example.com/acs"" method=""post"">
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

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_WithRelayState()
        {
            var message = new Saml2MessageImplementation
            {
                DestinationUrl = new Uri("http://www.example.com/acs"),
                XmlData = "<root><content>data</content></root>",
                MessageName = "SAMLMessageName",
                RelayState = "ABC1234"
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(message);

            var expected = new CommandResult()
            {
                ContentType = "text/html",
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
<form action=""http://www.example.com/acs"" method=""post"">
<div>
<input type=""hidden"" name=""RelayState"" value=""ABC1234""/>
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

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.CanUnbind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }
    }
}

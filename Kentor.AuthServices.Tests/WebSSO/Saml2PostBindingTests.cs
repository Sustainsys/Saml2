using System;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text;
using System.Collections.Generic;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class Saml2PostBindingTests
    {
        private HttpRequestData CreateRequest(string encodedResponse)
        {
            return new HttpRequestData(
                "POST",
                new Uri("http://example.com"),
                "/ModulePath",
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLResponse", new string[] {encodedResponse }) 
                });
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

            Saml2Binding.Get(Saml2BindingType.HttpPost).Unbind(r).Should().Be(response);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck_payload()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Bind(null, new Uri("http://host")))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck_destinationUrl()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Bind(new Saml2AuthenticationRequest(), null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("destinationUrl");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind()
        {
            var destinationUri = new Uri("http://www.example.com/acs");
            var message = new ConcreteSaml2Request();
            var subject = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(message, destinationUri);

            var expected = new CommandResult
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
<input type=""hidden"" name=""SAMLRequest"" 
value=""PEZvbyB4bWxuczpzYW1sMnA9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpwcm90b2NvbCIgeG1sbnM6c2FtbDI9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphc3NlcnRpb24iIElEPSJpZDFmZjUzZGNhZmY4ODQ4NWRhNDUyZDlmMTk4NDg2MmQ1IiBWZXJzaW9uPSIyLjAiIElzc3VlSW5zdGFudD0iMjAwNC0xMi0wNVQwOToyMTo1OVoiIC8+""/>
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
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }
    }
}

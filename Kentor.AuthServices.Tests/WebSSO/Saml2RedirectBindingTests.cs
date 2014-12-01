using System;
using System.IO;
using System.IO.Compression;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NSubstitute;
using System.Web;
using Kentor.AuthServices.WebSso;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class Saml2RedirectBindingTests
    {
        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck_message()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(null, new Uri("http://host")))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck_destinationUrl()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(new Saml2AuthenticationRequest(), null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("destinationUrl");
        }

        [TestMethod]
        public void Saml2PostBinding_Unind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Unbind(null).Should().BeNull();
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind()
        {
            var message = new ConcreteSaml2Request();
            const string serializedData = "fcuxCsIwEIDhVwnZ1SQ20hxWEEQo6KQ4uB1NAoU2V3Ip%2bPhmdHL8f%2fiOVyLxmafEwDhPZunkmhMQ8siQcA4MZYDH%2bX4Ds1WwZCo00CR%2fzX%2bCzCGXkZIU%2faWTo9cx2r0fMMa2bVrrsbHGu6hdrYPxVopXyFxBJ6uvinkNfeKCqdSlVLPRZqPsUzkwGqx7S7E7fQEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%3d%3d";
            var destinationUri = new Uri("http://www.example.com/acs");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message, destinationUri);

            var expected = new CommandResult
            {
                Location = new Uri("http://www.example.com/acs?SAMLRequest=" + serializedData),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_SignedBind()
        {
            var message = new ConcreteSaml2Request();
            message.SigningCertificate = XmlDocumentExtensionsTests.TestCert;

            const string serializedData = "fcuxCsIwEIDhVwnZ1SQ20hxWEEQo6KQ4uB1NAoU2V3Ip%2bPhmdHL8f%2fiOVyLxmafEwDhPZunkmhMQ8siQcA4MZYDH%2bX4Ds1WwZCo00CR%2fzX%2bCzCGXkZIU%2faWTo9cx2r0fMMa2bVrrsbHGu6hdrYPxVopXyFxBJ6uvinkNfeKCqdSlVLPRZqPsUzkwGqx7S7E7fQEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%3d%3d";
            const string signature = "Gjl9DaBdo7bRnpQI61%2f9ozGvXDc6%2buXR3GfQ082O75iBHYvHVOVuoj8cUkG74o9aUSlueyaZYIaRrvKbTuIJ6uVkReZan7%2bz0gg3ffLIifKnxvxLdgmMQzdc2CYA%2bz5%2fXos74zU3YRFuVzUom07y%2bGvFHxpbbx92qLGmqSh5I5%2fUwqRDwaDW36kT7MT%2bSRm5i0MWlcS9AXx17Qym7MLfYW5kPBnVN73GqDCH0wkMXZ3s%2bwlBtIft0wdK%2bmBEDpSbs0Pwt%2blfe2hkr9vur1zIrnsZ9dyvAVWT%2bYgFOj2eU9C62l6QJs%2fq3%2bg4ooU0Y%2blD%2bEaMuqJsuffZOgy7E2voYw%3d%3d";

            var destinationUri = new Uri("http://www.example.com/acs");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message, destinationUri);

            var expected = new CommandResult
            {
                Location = new Uri("http://www.example.com/acs?SAMLRequest=" + serializedData + "&SigAlg=http%3a%2f%2fwww.w3.org%2f2000%2f09%2fxmldsig%23rsa-sha1&Signature=" + signature),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            subject.ShouldBeEquivalentTo(expected);
        }

        private HttpRequestData CreateRequest(string encodedResponse)
        {
            return new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" + encodedResponse));
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind()
        {
            // Example from http://en.wikipedia.org/wiki/SAML_2.0#HTTP_Redirect_Binding

            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<samlp:AuthnRequest
  xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
  xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion""
  ID=""aaf23196-1773-2113-474a-fe114412ab72""
  Version=""2.0""
  IssueInstant=""2004-12-05T09:21:59Z""
  AssertionConsumerServiceIndex=""0""
  AttributeConsumingServiceIndex=""0"">
  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>
  <samlp:NameIDPolicy
    AllowCreate=""true""
    Format=""urn:oasis:names:tc:SAML:2.0:nameid-format:transient""/>
</samlp:AuthnRequest>
";

            var serializedData = "fZFfa8IwFMXfBb9DyXvaJtZ1BqsURRC2Mabbw95ivc5Am3TJrXPffmmLY3%2fA15Pzuyf33On8XJXBCaxTRmeEhTEJQBdmr%2fRbRp63K3pL5rPhYOpkVdYib%2fCon%2bC9AYfDQRB4WDvRvWWksVoY6ZQTWlbgBBZik9%2ffCR7GorYGTWFK8pu6DknnwKL%2fWEetlxmR8sBHbHJDWZqOKGdsRJM0kfQAjCUJ43KX8s78ctnIz%2blp5xpYa4dSo1fjOKGM03i8jSeCMzGevHa2%2fBK5MNo1FdgN2JMqPLmHc0b6WTmiVbsGoTf5qv66Zq2t60x0wXZ2RKydiCJXh3CWVV1CWJgqanfl0%2bin8xutxYOvZL18NKUqPlvZR5el%2bVhYkAgZQdsA6fWVsZXE63W2itrTQ2cVaKV2CjSSqL1v9P%2fAXv4C"; // hand patched with lower case version of UrlEncode characters to match the .NET UrlDecode output

            var r = CreateRequest(serializedData);
            Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(r).Should().Be(xmlData);

        }
    }
}

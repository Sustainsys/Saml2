using System;
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
        public void Saml2PostBinding_Bind_Nullcheck_payload()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(null, new Uri("http://host"), null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("payload");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck_destinationUrl()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind("-", null, null))
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
            var destinationUrl = new Uri("http://www.example.com/acs");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(xmlData, destinationUrl, null);

            var expected = new CommandResult()
            {
                Location = new Uri("http://www.example.com/acs?SAMLRequest=" + serializedData),
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

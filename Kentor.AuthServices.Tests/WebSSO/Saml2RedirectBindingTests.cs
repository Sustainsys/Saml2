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
        public void Saml2RedirectBinding_Bind_Nullcheck_payload()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(null, new Uri("http://host"), null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("payload");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_Nullcheck_destinationUrl()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind("-", null, null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("destinationUrl");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_NullcheckRequest()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Unbind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind()
        {
            // Example from http://en.wikipedia.org/wiki/SAML_2.0#HTTP_Redirect_Binding
            var xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<samlp:AuthnRequest\r\n"
                + "  xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"\r\n"
                + "  xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"\r\n"
                + "  ID=\"aaf23196-1773-2113-474a-fe114412ab72\"\r\n"
                + "  Version=\"2.0\"\r\n"
                + "  IssueInstant=\"2004-12-05T09:21:59Z\"\r\n"
                + "  AssertionConsumerServiceIndex=\"0\"\r\n"
                + "  AttributeConsumingServiceIndex=\"0\">\r\n"
                + "  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>\r\n"
                + "  <samlp:NameIDPolicy\r\n"
                + "    AllowCreate=\"true\"\r\n"
                + "    Format=\"urn:oasis:names:tc:SAML:2.0:nameid-format:transient\"/>\r\n"
                + "</samlp:AuthnRequest>\r\n";

            var serializedData = "fZFfa8IwFMXfBb9DyXvaJtZ1BqsURRC2Mabbw95ivc5Am3TJrXPffmmLY3%2FA15Pzuyf33On8XJXBCaxTRmeEhTEJQBdmr%2FRbRp63K3pL5rPhYOpkVdYib%2FCon%2BC9AYfDQRB4WDvRvWWksVoY6ZQTWlbgBBZik9%2FfCR7GorYGTWFK8pu6DknnwKL%2FWEetlxmR8sBHbHJDWZqOKGdsRJM0kfQAjCUJ43KX8s78ctnIz%2Blp5xpYa4dSo1fjOKGM03i8jSeCMzGevHa2%2FBK5MNo1FdgN2JMqPLmHc0b6WTmiVbsGoTf5qv66Zq2t60x0wXZ2RKydiCJXh3CWVV1CWJgqanfl0%2Bin8xutxYOvZL18NKUqPlvZR5el%2BVhYkAgZQdsA6fWVsZXE63W2itrTQ2cVaKV2CjSSqL1v9P%2FAXv4C";
            var destinationUrl = new Uri("http://www.example.com/sso");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(xmlData, destinationUrl, null);

            var expected = new CommandResult()
            {
                Location = new Uri("http://www.example.com/sso?SAMLRequest=" + serializedData),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            subject.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind()
        {
            // Example from http://en.wikipedia.org/wiki/SAML_2.0#HTTP_Redirect_Binding
            var xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<samlp:AuthnRequest\r\n"
                + "  xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"\r\n"
                + "  xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"\r\n"
                + "  ID=\"aaf23196-1773-2113-474a-fe114412ab72\"\r\n"
                + "  Version=\"2.0\"\r\n"
                + "  IssueInstant=\"2004-12-05T09:21:59Z\"\r\n"
                + "  AssertionConsumerServiceIndex=\"0\"\r\n"
                + "  AttributeConsumingServiceIndex=\"0\">\r\n"
                + "  <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>\r\n"
                + "  <samlp:NameIDPolicy\r\n"
                + "    AllowCreate=\"true\"\r\n"
                + "    Format=\"urn:oasis:names:tc:SAML:2.0:nameid-format:transient\"/>\r\n"
                + "</samlp:AuthnRequest>\r\n";

            var serializedData = "fZFfa8IwFMXfBb9DyXvaJtZ1BqsURRC2Mabbw95ivc5Am3TJrXPffmmLY3%2fA15Pzuyf33On8XJXBCaxTRmeEhTEJQBdmr%2fRbRp63K3pL5rPhYOpkVdYib%2fCon%2bC9AYfDQRB4WDvRvWWksVoY6ZQTWlbgBBZik9%2ffCR7GorYGTWFK8pu6DknnwKL%2fWEetlxmR8sBHbHJDWZqOKGdsRJM0kfQAjCUJ43KX8s78ctnIz%2blp5xpYa4dSo1fjOKGM03i8jSeCMzGevHa2%2fBK5MNo1FdgN2JMqPLmHc0b6WTmiVbsGoTf5qv66Zq2t60x0wXZ2RKydiCJXh3CWVV1CWJgqanfl0%2bin8xutxYOvZL18NKUqPlvZR5el%2bVhYkAgZQdsA6fWVsZXE63W2itrTQ2cVaKV2CjSSqL1v9P%2fAXv4C"; // hand patched with lower case version of UrlEncode characters to match the .NET UrlDecode output

            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" + serializedData));
            Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request).Should().Be(xmlData);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_WithQueryIn_destinationUrl()
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

            var serializedData = "fZFfa8IwFMXfBb9DyXvaJtZ1BqsURRC2Mabbw95ivc5Am3TJrXPffmmLY3%2FA15Pzuyf33On8XJXBCaxTRmeEhTEJQBdmr%2FRbRp63K3pL5rPhYOpkVdYib%2FCon%2BC9AYfDQRB4WDvRvWWksVoY6ZQTWlbgBBZik9%2FfCR7GorYGTWFK8pu6DknnwKL%2FWEetlxmR8sBHbHJDWZqOKGdsRJM0kfQAjCUJ43KX8s78ctnIz%2Blp5xpYa4dSo1fjOKGM03i8jSeCMzGevHa2%2FBK5MNo1FdgN2JMqPLmHc0b6WTmiVbsGoTf5qv66Zq2t60x0wXZ2RKydiCJXh3CWVV1CWJgqanfl0%2Bin8xutxYOvZL18NKUqPlvZR5el%2BVhYkAgZQdsA6fWVsZXE63W2itrTQ2cVaKV2CjSSqL1v9P%2FAXv4C";
            var destinationUrl = new Uri("http://www.example.com/acs?aQueryParam=QueryParamValue");

            var subject = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(xmlData, destinationUrl, null);

            var expected = new CommandResult()
            {
                Location = new Uri("http://www.example.com/acs?aQueryParam=QueryParamValue&SAMLRequest=" + serializedData),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            subject.ShouldBeEquivalentTo(expected);
        }

    }
}

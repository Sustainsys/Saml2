using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NSubstitute;
using System.Web;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.WebSSO;
using Kentor.AuthServices.TestHelpers;
using System.Security.Cryptography;
using System.Text;

namespace Kentor.AuthServices.Tests.WebSso
{
    [TestClass]
    public class Saml2RedirectBindingTests
    {
        [TestMethod]
        public void Saml2RedirectBinding_Bind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_NullcheckRequest()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Unbind(null))
                .ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        // Example from http://en.wikipedia.org/wiki/SAML_2.0#HTTP_Redirect_Binding
        private const string ExampleXmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
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

        private const string ExampleSerializedData = "fZFfa8IwFMXfBb9DyXvaJtZ1BqsURRC2Mabbw95ivc5Am3TJrXPffmmLY3%2FA15Pzuyf33On8XJXBCaxTRmeEhTEJQBdmr%2FRbRp63K3pL5rPhYOpkVdYib%2FCon%2BC9AYfDQRB4WDvRvWWksVoY6ZQTWlbgBBZik9%2FfCR7GorYGTWFK8pu6DknnwKL%2FWEetlxmR8sBHbHJDWZqOKGdsRJM0kfQAjCUJ43KX8s78ctnIz%2Blp5xpYa4dSo1fjOKGM03i8jSeCMzGevHa2%2FBK5MNo1FdgN2JMqPLmHc0b6WTmiVbsGoTf5qv66Zq2t60x0wXZ2RKydiCJXh3CWVV1CWJgqanfl0%2Bin8xutxYOvZL18NKUqPlvZR5el%2BVhYkAgZQdsA6fWVsZXE63W2itrTQ2cVaKV2CjSSqL1v9P%2FAXv4C";

        [TestMethod]
        public void Saml2RedirectBinding_Bind()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = ExampleXmlData,
                DestinationUrl = new Uri("http://www.example.com/sso"),
                MessageName = "SAMLRequest"
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);

            var expected = new CommandResult()
            {
                Location = new Uri("http://www.example.com/sso?SAMLRequest=" + ExampleSerializedData),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_WithoutRelayState()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" + ExampleSerializedData));

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request);

            var expected = new
            {
                Data = ExampleXmlData,
                RelayState = (string)null
            };

            result.ShouldBeEquivalentTo(expected);
        }


        [TestMethod]
        public void Saml2RedirectBinding_Unbind_WithRelayState()
        {
            var relayState = "BD823LGD";

            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" 
                + ExampleSerializedData + "&RelayState=" + relayState));

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request);

            var expected = new
            {
                Data = ExampleXmlData,
                RelayState = relayState
            };

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_WithQueryIn_destinationUrl()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = ExampleXmlData,
                DestinationUrl = new Uri("http://www.example.com/acs?aQueryParam=QueryParamValue"),
                MessageName = "SAMLRequest"
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);

            var expected = new CommandResult()
            {
                Location = new Uri("http://www.example.com/acs?aQueryParam=QueryParamValue&SAMLRequest=" 
                    + ExampleSerializedData),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther,
            };

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_With_RelayState()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "Data",
                RelayState = "SomeState that needs escaping #%=3",
                DestinationUrl = new Uri("http://host"),
                MessageName = "SAMLRequest"
            };

            var expected = new CommandResult()
            {
                Location = new Uri("http://host?SAMLRequest=c0ksSQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA%3D%3D"
                + "&RelayState=" + Uri.EscapeDataString(message.RelayState)),
                HttpStatusCode = System.Net.HttpStatusCode.SeeOther
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_AddsSignature()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "Data",
                RelayState = "SomeState that needs escaping #%=3",
                DestinationUrl = new Uri("http://host"),
                MessageName = "SAMLRequest",
                SigningCertificate = SignedXmlHelper.TestCert
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);

            var query = result.Location.Query.TrimStart('?');

            var split = query.Split(new[] { "&Signature=" }, StringSplitOptions.None);
            var signedData = split[0];
            var signature = split[1];

            split = signedData.Split(new[] { "SigAlg=" }, StringSplitOptions.None);
            var sigalg = Uri.UnescapeDataString(split[1]);

            var signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(sigalg);

            var hashAlg = signatureDescription.CreateDigest();
            hashAlg.ComputeHash(Encoding.UTF8.GetBytes(signedData));
            var asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(
                SignedXmlHelper.TestCert.PublicKey.Key);

            asymmetricSignatureDeformatter.VerifySignature(
                hashAlg, Convert.FromBase64String(signature))
                .Should().BeTrue();
        }
    }
}

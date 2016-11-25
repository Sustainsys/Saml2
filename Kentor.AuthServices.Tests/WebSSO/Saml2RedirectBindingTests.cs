using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NSubstitute;
using System.Web;
using Kentor.AuthServices.WebSso;
using Kentor.AuthServices.Saml2P;
using Kentor.AuthServices.Tests.WebSSO;
using Kentor.AuthServices.Tests.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Exceptions;

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
                .Invoking(b => b.Unbind(null, null))
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
        public void Saml2RedirectBinding_Unbind_WithoutRelayStateAndSignature()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" + ExampleSerializedData));

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request, null);

            var expectedXml = XmlHelpers.FromString(ExampleXmlData).DocumentElement;

            result.RelayState.Should().Be(null);
            result.Data.Should().BeEquivalentTo(expectedXml);
            result.TrustLevel.Should().Be(TrustLevel.None);
        }


        [TestMethod]
        public void Saml2RedirectBinding_Unbind_WithRelayState()
        {
            var relayState = "BD823LGD";

            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" 
                + ExampleSerializedData + "&RelayState=" + relayState));

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request, null);

            result.RelayState.Should().Be(relayState);
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
            var actual = CreateAndBindMessageWithSignature();

            var queryParams = HttpUtility.ParseQueryString(actual.Location.Query);
            var query = actual.Location.Query.TrimStart('?');

            var signedData = query.Split(new[] { "&Signature=" }, StringSplitOptions.None)[0];

            var sigalg = queryParams["SigAlg"];
            var signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(sigalg);

            var hashAlg = signatureDescription.CreateDigest();
            hashAlg.ComputeHash(Encoding.UTF8.GetBytes(signedData));
            var asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(
                SignedXmlHelper.TestCert.PublicKey.Key);

            asymmetricSignatureDeformatter.VerifySignature(
                hashAlg, Convert.FromBase64String(queryParams["Signature"]))
                .Should().BeTrue("signature should be valid");
        }

        private static CommandResult CreateAndBindMessageWithSignature(
            string issuer = "https://idp.example.com",
            string messageName = "SAMLRequest",
            bool includeRelayState = true
            )
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "<Data/>",
                RelayState = includeRelayState ? "SomeState that needs escaping #%=3" : null,
                DestinationUrl = new Uri("http://host"),
                MessageName = messageName,
                SigningCertificate = SignedXmlHelper.TestCert
            };

            if(!string.IsNullOrEmpty(issuer))
            {
                message.XmlData = $"<Data><Issuer xmlns=\"{Saml2Namespaces.Saml2Name}\">{issuer}</Issuer></Data>";
            }

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);
            return result;
        }

        [TestMethod]
        public void Saml2RedirectBinding_CanUnbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.CanUnbind(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_HandlesValidSignature_SAMLResponse()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLResponse").Location;

            var request = new HttpRequestData("GET", url);

            var actual = Saml2Binding.Get(request)
                .Unbind(request, StubFactory.CreateOptions());

            actual.TrustLevel.Should().Be(TrustLevel.Signature);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_HandlesValidSignature_SAMLRequest()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLRequest").Location;

            var request = new HttpRequestData("GET", url);

            var actual = Saml2Binding.Get(request)
                .Unbind(request, StubFactory.CreateOptions());

            actual.TrustLevel.Should().Be(TrustLevel.Signature);
        }
        
        [TestMethod]
        public void Saml2RedirectBinding_Unbind_HandlesValidSignature_WithoutRelayState()
        {
            var url = CreateAndBindMessageWithSignature(includeRelayState: false).Location;

            var request = new HttpRequestData("GET", url);

            var actual = Saml2Binding.Get(request)
                .Unbind(request, StubFactory.CreateOptions());

            actual.TrustLevel.Should().Be(TrustLevel.Signature);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_TrustLevelNoneWithMissingSignature()
        {
            var url = CreateAndBindMessageWithSignature(null).Location;

            var request = new HttpRequestData("GET", url);

            var actual = Saml2Binding.Get(request)
                .Unbind(request, StubFactory.CreateOptions());

            actual.TrustLevel.Should().Be(TrustLevel.None);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnSignatureWithUnknownIssuer()
        {
            var url = CreateAndBindMessageWithSignature("http://unknown.idp.example.com").Location;

            var request = new HttpRequestData("GET", url);

            var actual = Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, StubFactory.CreateOptions()))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("Cannot verify signature of message from unknown sender http://unknown.idp.example.com.");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnSignatureWithTamperedData_SAMLRequest()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLRequest").Location.ToString();

            url = url.Replace("RelayState=", "RelayState=X");

            var request = new HttpRequestData("GET", new Uri(url));

            Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, StubFactory.CreateOptions()))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("Message from https://idp.example.com failed signature verification");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnSignatureWithTamperedData_SAMLResponse()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLResponse").Location.ToString();

            url = url.Replace("RelayState=", "RelayState=X");

            var request = new HttpRequestData("GET", new Uri(url));

            Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, StubFactory.CreateOptions()))
                .ShouldThrow<InvalidSignatureException>()
                .WithMessage("Message from https://idp.example.com failed signature verification");
        }

        [TestMethod]
        public void Saml2Redirectbinding_Unbind_TrustLevelNoneWithMissingOptions()
        {
            // The stub idp uses the binding, but doesn't provide an options
            // instance - enable it to do so by ignoring certificates.

            var url = CreateAndBindMessageWithSignature(messageName: "SAMLResponse").Location.ToString();

            var request = new HttpRequestData("GET", new Uri(url));

            var actual = Saml2Binding.Get(request)
                .Unbind(request, null);

            actual.TrustLevel.Should().Be(TrustLevel.None);
        }
    }
}

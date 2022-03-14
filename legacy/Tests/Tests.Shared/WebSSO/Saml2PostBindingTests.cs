using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text;
using System.Collections.Generic;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.WebSso;
using System.Security.Cryptography.Xml;
using NSubstitute;
using Sustainsys.Saml2.TestHelpers;
using System.Xml.Linq;
using Sustainsys.Saml2.Metadata.Tokens;

namespace Sustainsys.Saml2.Tests.WebSso
{
    [TestClass]
    public class Saml2PostBindingTests
    {
        private HttpRequestData CreateRequest(string encodedResponse, string relayState = null)
        {
            var formData = new List<KeyValuePair<string, IEnumerable<string>>>()
            {
                new KeyValuePair<string, IEnumerable<string>>("SAMLResponse", new string[] {encodedResponse })
            };

            if (!string.IsNullOrEmpty(relayState))
            {
                formData.Add(new KeyValuePair<string, IEnumerable<string>>("RelayState", new string[] { relayState }));
            };

            return new HttpRequestData(
                "POST",
                new Uri("http://example.com"),
                "/ModulePath",
                formData,
                null);
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_Nullcheck_Request()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(null, null))
                .Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_WorksEvenIfOptionsIsNull()
        {
            string response = "<responsestring/>";
            string relayState = "someState";

            var request = CreateRequest(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(response)),
                relayState);

            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(request, null))
                .Should().NotThrow();
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ThrowsOnNotBase64Encoded()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Unbind(CreateRequest("foo"), StubFactory.CreateOptions()))
                .Should().Throw<FormatException>();
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ReadsSaml2Response()
        {
            string response = "<responsestring />";

            var r = CreateRequest(Convert.ToBase64String(Encoding.UTF8.GetBytes(response)));

            Saml2Binding.Get(Saml2BindingType.HttpPost).Unbind(r, StubFactory.CreateOptions())
                .Data.OuterXml.Should().Be(response);
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_ReadsRelayState()
        {
            string response = "<responsestring/>";
            string relayState = "someState";

            var r = CreateRequest(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(response)),
                relayState);

            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Unbind(r, StubFactory.CreateOptions()).RelayState.Should().Be(relayState);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.Bind(null))
                .Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_LogsIfLoggerNonNull()
        {
            var logger = Substitute.For<ILoggerAdapter>();

            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Bind(new Saml2MessageImplementation
                {
                    XmlData = "<xml/>"
                },
                logger);

            logger.Received().WriteVerbose("Sending message over Http POST binding\n<xml />");
        }

        [TestMethod]
        public void Saml2PostBinding_Bind()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "<root>\r\n  <content>data</content>\r\n</root>",
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
<head>
<meta http-equiv=""Content-Security-Policy"" content=""script-src 'sha256-H3SVZBYrbqBt3ncrT/nNmOb6nwCjC12cPQzh5jnW4Y0='"">
</head>
<body>
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""http://www.example.com/acs"" method=""post"" name=""sustainsysSamlPostBindingSubmit"">
<div>
<input type=""hidden"" name=""SAMLMessageName""
value=""PHJvb3Q+DQogIDxjb250ZW50PmRhdGE8L2NvbnRlbnQ+DQo8L3Jvb3Q+""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
<script type=""text/javascript"">
document.forms.sustainsysSamlPostBindingSubmit.submit();
</script>
</body>
</html>"
            };

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_WithRelayState()
        {
            var message = new Saml2MessageImplementation
            {
                DestinationUrl = new Uri("http://www.example.com/acs"),
                XmlData = "<root>\r\n  <content>data</content>\r\n</root>",
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
<head>
<meta http-equiv=""Content-Security-Policy"" content=""script-src 'sha256-H3SVZBYrbqBt3ncrT/nNmOb6nwCjC12cPQzh5jnW4Y0='"">
</head>
<body>
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""http://www.example.com/acs"" method=""post"" name=""sustainsysSamlPostBindingSubmit"">
<div>
<input type=""hidden"" name=""RelayState"" value=""ABC1234""/>
<input type=""hidden"" name=""SAMLMessageName""
value=""PHJvb3Q+DQogIDxjb250ZW50PmRhdGE8L2NvbnRlbnQ+DQo8L3Jvb3Q+""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
<script type=""text/javascript"">
document.forms.sustainsysSamlPostBindingSubmit.submit();
</script>
</body>
</html>"
            };

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_SignsXml()
        {
            var message = new Saml2MessageImplementation
            {
                DestinationUrl = new Uri("http://www.example.com/acs"),
                XmlData = "<root ID=\"id\">\r\n  <content>data</content>\r\n</root>",
                MessageName = "SAMLMessageName",
                RelayState = "ABC1234",
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
            };

            var signedXml = SignedXmlHelper.SignXml(message.XmlData, true);
            var expectedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(signedXml));

            var result = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(message);

            var expected = new CommandResult()
            {
                ContentType = "text/html",
                Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN""
""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<head>
<meta http-equiv=""Content-Security-Policy"" content=""script-src 'sha256-H3SVZBYrbqBt3ncrT/nNmOb6nwCjC12cPQzh5jnW4Y0='"">
</head>
<body>
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""http://www.example.com/acs"" method=""post"" name=""sustainsysSamlPostBindingSubmit"">
<div>
<input type=""hidden"" name=""RelayState"" value=""ABC1234""/>
<input type=""hidden"" name=""SAMLMessageName""
value=""" + expectedValue + @"""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
<script type=""text/javascript"">
document.forms.sustainsysSamlPostBindingSubmit.submit();
</script>
</body>
</html>"
            };

            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2PostBinding_Bind_SignsXmlAndPreserversXmlDeclaration()
        {
            var message = new Saml2MessageImplementation
            {
                DestinationUrl = new Uri("http://www.example.com/acs"),
                XmlData = "<root ID=\"id\">\r\n  <content>data</content>\r\n</root>",
                MessageName = "SAMLMessageName",
                RelayState = "ABC1234",
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
            };

            var signedXml = "<?xml version=\"1.0\" encoding=\"blaha\"?>\r\n" 
                + SignedXmlHelper.SignXml(message.XmlData, true);
            var expectedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(signedXml));

            var notificationCalled = false;
            var result = Saml2Binding.Get(Saml2BindingType.HttpPost).Bind(message, null, (m, xd, bt) =>
            {
                xd.Declaration = new XDeclaration("1.0", "blaha", null);
                m.Should().BeSameAs(message);
                bt.Should().Be(Saml2BindingType.HttpPost);
                notificationCalled = true;
            });

            var expected = new CommandResult()
            {
                ContentType = "text/html",
                Content = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN""
""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""en"">
<head>
<meta http-equiv=""Content-Security-Policy"" content=""script-src 'sha256-H3SVZBYrbqBt3ncrT/nNmOb6nwCjC12cPQzh5jnW4Y0='"">
</head>
<body>
<noscript>
<p>
<strong>Note:</strong> Since your browser does not support JavaScript, 
you must press the Continue button once to proceed.
</p>
</noscript>
<form action=""http://www.example.com/acs"" method=""post"" name=""sustainsysSamlPostBindingSubmit"">
<div>
<input type=""hidden"" name=""RelayState"" value=""ABC1234""/>
<input type=""hidden"" name=""SAMLMessageName""
value=""" + expectedValue + @"""/>
</div>
<noscript>
<div>
<input type=""submit"" value=""Continue""/>
</div>
</noscript>
</form>
<script type=""text/javascript"">
document.forms.sustainsysSamlPostBindingSubmit.submit();
</script>
</body>
</html>"
            };

            result.Should().BeEquivalentTo(expected);
            notificationCalled.Should().BeTrue();
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_Nullcheck_Request()
        {
            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .Invoking(b => b.CanUnbind(null))
                .Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_DetectsRequests()
        {
            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes("<data/>"));

            var request = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            Saml2Binding.Get(Saml2BindingType.HttpPost)
                .CanUnbind(request).Should().BeTrue();
        }

        [TestMethod]
        public void Saml2PostBinding_Unbind_Request()
        {
            var requestData = Convert.ToBase64String(Encoding.UTF8.GetBytes("<data/>"));

            var request = new HttpRequestData(
                "POST",
                new Uri("http://something"),
                "/path",
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("SAMLRequest", new[] { requestData })
                },
                null,
                null);

            var actual = Saml2Binding.Get(request).Unbind(request, StubFactory.CreateOptions());

            actual.Data.Should().BeEquivalentTo(XmlHelpers.XmlDocumentFromString("<data/>").DocumentElement);
            actual.RelayState.Should().BeNull();
            actual.TrustLevel.Should().Be(TrustLevel.None);
        }
    }
}

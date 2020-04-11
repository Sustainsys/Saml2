using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NSubstitute;
using System.Web;
using Sustainsys.Saml2.WebSso;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Sustainsys.Saml2.Exceptions;
using System.Security.Cryptography.Xml;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Internal;
using Sustainsys.Saml2.Tokens;
using Sustainsys.Saml2.TestHelpers;
using System.Xml.Linq;
using Sustainsys.Saml2.Metadata.Exceptions;
using Sustainsys.Saml2.Metadata.Tokens;

namespace Sustainsys.Saml2.Tests.WebSso
{
    [TestClass]
    public class Saml2RedirectBindingTests
    {
        [TestMethod]
        public void Saml2RedirectBinding_Bind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Bind(null))
                .Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("message");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_Nullcheck_Request()
        {
            Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Invoking(b => b.Unbind(null, null))
                .Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("request");
        }

        private const string ExampleXmlData = "<samlp:AuthnRequest xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\"aaf23196-1773-2113-474a-fe114412ab72\" Version=\"2.0\" IssueInstant=\"2004-12-05T09:21:59Z\" AssertionConsumerServiceIndex=\"0\" AttributeConsumingServiceIndex=\"0\">\r\n"
            + " <saml:Issuer>https://sp.example.com/SAML2</saml:Issuer>\r\n"
            + " <samlp:NameIDPolicy AllowCreate=\"true\" Format=\"urn:oasis:names:tc:SAML:2.0:nameid-format:transient\" />\r\n"
            + "</samlp:AuthnRequest>";

		private const string ExampleSerializedData = "fZFBSwMxEIXvgv8h5J7uJt26NHQLS4tQUBErHryl26kNbJI1M6v135ttUaqHXmfeN4%2F3ZobGtZ2ue9r7J3jvAYkdXOtRHxcV76PXwaBF7Y0D1NTodX1%2Fp9Uo110MFJrQ8jPkMmEQIZINnrPVsuLG7NRYTm%2BELMuxUFKORVEWRuxAyqKQymxKxdkLRExIxdOFxCH2sPJIxlMa5XkhpBL55DmfaiX1ZPrKWf1jswgeewdxDfHDNgnbwqHi6UpNFO2mJzgprH%2F7L5lfXzE2GyLpo2Wc74k61FmG3QgOxnUtjJrgsiGcmmXnyl%2B00w%2Bpg9XyMbS2%2BWJ124bPRQRDUHGKPXB2G6IzdLm1YWK3YneUaorGowVPnGXJ6eT894Xzbw%3D%3D";

		private static string InflateBase64EncodedData(string input)
		{
			byte[] data = Convert.FromBase64String(input);
			using (MemoryStream ms = new MemoryStream(data, false))
			using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
			using (StreamReader sr = new StreamReader(ds))
			{
				return sr.ReadToEnd();
			}
		}

		private static void CompareCommandResults(CommandResult result, CommandResult expected)
		{
			result.HttpStatusCode.Should().Be(expected.HttpStatusCode);
			result.Cacheability.Should().Be(expected.Cacheability);
			result.Principal.Should().Be(expected.Principal);
			result.SessionNotOnOrAfter.Should().Be(expected.SessionNotOnOrAfter);
			result.Content.Should().Be(expected.Content);
			result.ContentType.Should().Be(expected.ContentType);
			result.RelayData.Should().BeEquivalentTo(expected.RelayData);
			result.TerminateLocalSession.Should().Be(expected.TerminateLocalSession);
			result.SetCookieName.Should().Be(expected.SetCookieName);
			result.RelayState.Should().Be(expected.RelayState);
			result.RequestState.Should().Be(expected.RequestState);
			result.ClearCookieName.Should().Be(expected.ClearCookieName);
			result.HandledResult.Should().Be(expected.HandledResult);

			if (result.Location == null)
			{
				if (expected.Location != null)
				{
					throw new Exception(
						$"Expected member Location to be {expected.Location} but found null");
				}
			}
			else
			{
				if (expected.Location == null)
				{
					throw new Exception(
						$"Expected member Location to be null but found but found {result.Location}");
				}

				var components = UriComponents.Scheme | UriComponents.Host | UriComponents.Port 
					| UriComponents.Path;
				result.Location.GetComponents(components, UriFormat.UriEscaped).Should().Be(
					expected.Location.GetComponents(components, UriFormat.UriEscaped));

				var resultQuery = HttpUtility.ParseQueryString(result.Location.Query);
				var expectedQuery = HttpUtility.ParseQueryString(expected.Location.Query);
				resultQuery.Keys.Should().BeEquivalentTo(expectedQuery.Keys);

				foreach (string key in resultQuery)
				{
					var resultValues = resultQuery.GetValues(key);
					var expectedValues = expectedQuery.GetValues(key);
					resultValues.Length.Should().Be(expectedValues.Length);

					for (int i = 0; i < resultValues.Length; ++i)
					{
						var resultValue = resultValues[0];
						var expectedValue = expectedValues[0];

						if (key == "SAMLRequest")
						{
							resultValue = InflateBase64EncodedData(resultValue);
							expectedValue = InflateBase64EncodedData(expectedValue);
						}
						resultValue.Should().Be(expectedValue);
					}
				}
			}
		}

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
				HttpStatusCode = HttpStatusCode.SeeOther,
			};

			CompareCommandResults(result, expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_WithXmlDeclaration()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "<xml/>",
                DestinationUrl = new Uri("http://www.example.com/sso"),
                MessageName = "SAMLRequest"
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message, null, (m, x, t) =>
            {
                x.Declaration = new XDeclaration("42.17", "utf-73", null);
            });

            var payload = Uri.UnescapeDataString(result.Location.Query.Split('=')[1]);

            var inflated = InflateBase64EncodedData(payload);

            inflated.Should().Be("<?xml version=\"42.17\" encoding=\"utf-73\"?>\r\n<xml />");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_WithoutRelayStateAndSignature()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost?SAMLRequest=" + ExampleSerializedData));

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Unbind(request, null);

            var expectedXml = XmlHelpers.XmlDocumentFromString(ExampleXmlData).DocumentElement;

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

			CompareCommandResults(result, expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_With_RelayState()
        {
            var message = new Saml2MessageImplementation
            {
                XmlData = "<xml />",
                RelayState = "SomeState that needs escaping #%=3",
                DestinationUrl = new Uri("http://host"),
                MessageName = "SAMLRequest"
            };

            var expected = new CommandResult()
            {
                Location = new Uri("http://host?SAMLRequest=s6nIzVHQtwMA"
                + "&RelayState=" + Uri.EscapeDataString(message.RelayState)),
                HttpStatusCode = HttpStatusCode.SeeOther
            };

            var result = Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message);

			CompareCommandResults(result, expected);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_AddsSignature()
        {
            var actual = CreateAndBindMessageWithSignature();

            var queryParams = HttpUtility.ParseQueryString(actual.Location.Query);
            var query = actual.Location.Query.TrimStart('?');

            var signedData = query.Split(new[] { "&Signature=" }, StringSplitOptions.None)[0];

            var sigalg = queryParams["SigAlg"];
            var signatureDescription = (SignatureDescription)CryptographyExtensions
				.CreateAlgorithmFromName(sigalg);

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
                SigningCertificate = SignedXmlHelper.TestCert,
                SigningAlgorithm = SecurityAlgorithms.RsaSha256Signature
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
                .Should().Throw<ArgumentNullException>()
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
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("Cannot verify signature of message from unknown sender http://unknown.idp.example.com.");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnWeakSignatureAlgorithm()
        {
            var url = CreateAndBindMessageWithSignature().Location;

            var request = new HttpRequestData("GET", url);

            var options = StubFactory.CreateOptions();
            options.SPOptions.MinIncomingSigningAlgorithm = SecurityAlgorithms.RsaSha384Signature;

            // Check that the created url indeed is signed with SHA256.
            url.OriginalString.Should().Contain("sha256");

            var actual = Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, options))
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("*weak*");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnSignatureWithTamperedData_SAMLRequest()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLRequest").Location.OriginalString;

            url = url.Replace("RelayState=", "RelayState=X");

            var request = new HttpRequestData("GET", new Uri(url));

            Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, StubFactory.CreateOptions()))
                .Should().Throw<InvalidSignatureException>("contents has been tampered with. Tampered URL: " + url)
                .WithMessage("Message from https://idp.example.com failed signature verification");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Unbind_ThrowsOnSignatureWithTamperedData_SAMLResponse()
        {
            var url = CreateAndBindMessageWithSignature(messageName: "SAMLResponse").Location.OriginalString;

            url = url.Replace("RelayState=", "RelayState=X");

            var request = new HttpRequestData("GET", new Uri(url));

            Saml2Binding.Get(request)
                .Invoking(b => b.Unbind(request, StubFactory.CreateOptions()))
                .Should().Throw<InvalidSignatureException>()
                .WithMessage("Message from https://idp.example.com failed signature verification");
        }

        [TestMethod]
        public void Saml2Redirectbinding_Unbind_TrustLevelNoneWithMissingOptions()
        {
            // The stub idp uses the binding, but doesn't provide an options
            // instance - enable it to do so by ignoring certificates.

            var url = CreateAndBindMessageWithSignature(messageName: "SAMLResponse").Location.OriginalString;

            var request = new HttpRequestData("GET", new Uri(url));

            var actual = Saml2Binding.Get(request)
                .Unbind(request, null);

            actual.TrustLevel.Should().Be(TrustLevel.None);
        }

        [TestMethod]
        public void Saml2RedirectBinding_Bind_WritesLogIfLoggerNotNull()
        {
            var message = new Saml2MessageImplementation()
            {
                DestinationUrl = new Uri("http://destination"),
                XmlData = "<xml />"
            };
            var logger = Substitute.For<ILoggerAdapter>();

            Saml2Binding.Get(Saml2BindingType.HttpRedirect).Bind(message, logger);

            logger.Received().WriteVerbose("Sending message over Http Redirect Binding\n<xml />");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Net;
using System.Web;
using System.Linq;
using NSubstitute;
using System.IO.Compression;
using System.IO;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class SignInCommandTests
    {
        [TestMethod]
        public void SignInCommand_Run_ReturnsAuthnRequestForDefaultIdp()
        {
            var defaultDestination = IdentityProvider.ConfiguredIdentityProviders.First()
                .Value.DestinationUri;

            var subject = new SignInCommand().Run(new HttpRequestData(null, null));

            var expected = new CommandResult()
            {
                HttpStatusCode = HttpStatusCode.SeeOther,
                Cacheability = HttpCacheability.NoCache,
                Location = new Uri(defaultDestination + "?SAMLRequest=XYZ")
            };

            subject.ShouldBeEquivalentTo(expected, options => options.Excluding(cr => cr.Location));
            subject.Location.Host.Should().Be(defaultDestination.Host);

            var queries = HttpUtility.ParseQueryString(subject.Location.Query);

            queries.Should().HaveCount(1);
            queries.Keys[0].Should().Be("SAMLRequest");
            queries[0].Should().NotBeEmpty();
        }

        [TestMethod]
        public void SignInCommand_Run_MapsReturnUrl()
        {
            var defaultDestination = IdentityProvider.ConfiguredIdentityProviders.First()
                .Value.DestinationUri;

            var httpRequest = new HttpRequestData("GET", new Uri("http://localhost/signin?ReturnUrl=/Return.aspx"));

            var subject = new SignInCommand().Run(httpRequest);

            var idp = IdentityProvider.ConfiguredIdentityProviders.First().Value;

            var authnRequest = idp.CreateAuthenticateRequest(null);

            // Dig out requestId from the signincommand
            string requestId;
            var tmp = Convert.FromBase64String(HttpUtility.UrlDecode(subject.Location.Query.Replace("?SAMLRequest=", "")));
            using (var compressed = new MemoryStream(tmp))
            {
                compressed.Seek(0, SeekOrigin.Begin);
                using (var decompressedStream = new DeflateStream(compressed, CompressionMode.Decompress))
                {
                    using (var deCompressed = new MemoryStream())
                    {
                        decompressedStream.CopyTo(deCompressed);
                        var xmlData = System.Text.Encoding.UTF8.GetString(deCompressed.GetBuffer());
                        var requestXml = XDocument.Parse(xmlData);
                        requestId = requestXml.Document.Root.Attribute(XName.Get("ID")).Value;
                    }
                }
            }

            StoredRequestState storedAuthnData;
            PendingAuthnRequests.TryRemove(new System.IdentityModel.Tokens.Saml2Id(requestId), out storedAuthnData);

            storedAuthnData.ReturnUri.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void SignInCommand_Run_With_Idp2_ReturnsAuthnRequestForSecondIdp()
        {
            var secondIdp = IdentityProvider.ConfiguredIdentityProviders.Skip(1).First().Value;
            var secondDestination = secondIdp.DestinationUri;
            var secondIssuer = secondIdp.Issuer;

            var request = new HttpRequestData("GET", new Uri("http://sp.example.com?ìdp=" +
            HttpUtility.UrlEncode(secondIssuer)));
            var subject = new SignInCommand().Run(request);

            subject.Location.Host.Should().Be(secondDestination.Host);
        }

        [TestMethod]
        public void SignInCommand_Run_With_InvalidIdp_ThrowsException()
        {
            var request = new HttpRequestData("GET", new Uri("http://localhost/signin?idp=no-such-idp-in-config"));
            
            Action a = () => new SignInCommand().Run(request);

            a.ShouldThrow<InvalidOperationException>().WithMessage("Unknown idp");
        }
    }
}

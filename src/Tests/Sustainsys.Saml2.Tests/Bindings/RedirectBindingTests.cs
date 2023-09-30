using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Bindings;

public class RedirectBindingTests
{
    private const string Xml = "<xml />";

    [Fact]
    public async Task Bind()
    {
        var xd = new XmlDocument();
        xd.LoadXml(Xml);

        var message = new OutboundSaml2Message
        {
            Name = "SamlRequest",
            Xml = xd.DocumentElement!,
            Destination = "https://example.com/destination",
            RelayState = "someRelayState"
        };

        var subject = new HttpRedirectBinding();

        var httpResponse = Substitute.For<HttpResponse>();

        await subject.BindAsync(httpResponse, message);

        void validateUrl(string url)
        {
            url.Should().StartWith(message.Destination);

            Uri uri = new Uri(url);

            var query = uri.Query.Split("&");

            var expectedParam = $"{message!.Name}=";

            query[0].StartsWith(expectedParam).Should().BeTrue();

            var value = query[0][expectedParam.Length..];

            using var inflated = new MemoryStream(Convert.FromBase64String(Uri.UnescapeDataString(value)));
            using var deflateStream = new DeflateStream(inflated, CompressionMode.Decompress);
            using var reader = new StreamReader(deflateStream);

            reader.ReadToEnd().Should().Be(Xml);

            query[1].Should().Be("RelayState=someRelayState");
        }

        httpResponse.Received().Redirect(Arg.Do<string>(validateUrl));
    }
}

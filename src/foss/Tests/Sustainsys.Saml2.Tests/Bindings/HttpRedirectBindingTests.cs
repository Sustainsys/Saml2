// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System.IO.Compression;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Bindings;

public class HttpRedirectBindingTests
{
    private const string Xml = "<xml />";

    [Fact]
    public async Task Bind()
    {
        var xd = new XmlDocument();
        xd.LoadXml(Xml);

        var message = new Saml2Message
        {
            Name = "SamlRequest",
            Xml = xd.DocumentElement!,
            Destination = "https://example.com/destination",
            RelayState = "someRelayState"
        };

        var subject = new HttpRedirectBinding();

        var httpResponse = Substitute.For<HttpResponse>();

        bool validated = false;

        httpResponse.Redirect(Arg.Do<string>(validateUrl));

        await subject.BindAsync(httpResponse, message);

        void validateUrl(string url)
        {
            url.Should().StartWith(message.Destination);

            Uri uri = new Uri(url);

            var query = uri.Query.TrimStart('?').Split("&");

            var expectedParam = $"{message!.Name}=";

            query[0].Should().StartWith(expectedParam);

            var value = query[0][expectedParam.Length..];

            using var inflated = new MemoryStream(Convert.FromBase64String(Uri.UnescapeDataString(value)));
            using var deflateStream = new DeflateStream(inflated, CompressionMode.Decompress);
            using var reader = new StreamReader(deflateStream);

            reader.ReadToEnd().Should().Be(Xml);

            query[1].Should().Be("RelayState=someRelayState");

            validated = true;
        }

        httpResponse.Received().Redirect(Arg.Any<string>());

        validated.Should().BeTrue();
    }
    private static string GetEncodedXml()
    {
        using var compressed = new MemoryStream();
        using (var deflateStream = new DeflateStream(compressed, CompressionLevel.Optimal))
        {
            using var writer = new StreamWriter(deflateStream);
            writer.Write(Xml);
        }

        return Uri.EscapeDataString(Convert.ToBase64String(compressed.ToArray()));
    }

    [Theory]
    [InlineData(Constants.SamlRequest, null)]
    [InlineData(Constants.SamlResponse, null)]
    [InlineData("Invalid", "SAMLResponse or SAMLRequest parameter not found")]
    [InlineData("SAMLRequest=x&SAMLResponse", "Duplicate message parameters*")]
    [InlineData("SAMLRequest=x&SAMLRequest", "Duplicate message parameters found*")]
    [InlineData("SAMLResponse=x&SAMLRequest", "Duplicate message parameters found: SAMLResponse, SAMLRequest")]
    [InlineData("RelayState=x&SAMLRequest", "Duplicate RelayState parameters found")]
    public async Task UnBind_String(string messageName, string? expectedException)
    {
        var encoded = GetEncodedXml();

        var url = $"https://idp.example.com/sso?{messageName}={encoded}&RelayState=xyz123";

        var subject = new HttpRedirectBinding();

        var xd = new XmlDocument();
        xd.LoadXml(Xml);

        var expected = new Saml2Message
        {
            Destination = "https://idp.example.com/sso",
            Name = messageName,
            RelayState = "xyz123",
            Xml = xd.DocumentElement!
        };

        bool caughtException = false;
        Saml2Message actual = default!;
        try
        {
            actual = await subject.UnBindAsync(url, x => Task.FromResult(new Saml2Entity()));
        }
        catch (Exception ex)
        {
            caughtException = true;
            ex.Message.Should().Match(expectedException);
        }

        if (!caughtException)
        {
            expectedException.Should().BeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
using FluentAssertions;
using NSubstitute;
using Sustainsys.Saml2.Bindings;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Tests.Bindings;

public class RedirectBindingTests
{
    private const string Xml = "<xml />";

    [Fact]
    public void Bind()
    {
        var xd = new XmlDocument();
        xd.LoadXml(Xml);

        var message = new Saml2Message
        {
            Name = "SamlRequest",
            Xml = xd
        };

        var subject = new RedirectBinding();

        var actual = subject.Bind(message);

        actual.Method.Should().Be(HttpMethod.Get);

        actual.Items.Should().HaveCount(1);
        var item = actual.Items[0];

        item.Key.Should().Be(message.Name);
       
        using var inflated = new MemoryStream(Convert.FromBase64String(Uri.UnescapeDataString(item.Value)));
        using var deflateStream = new DeflateStream(inflated, CompressionMode.Decompress);
        using var reader = new StreamReader(deflateStream);

        reader.ReadToEnd().Should().Be(Xml);
    }
}

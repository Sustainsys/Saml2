using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Serialization;
public partial class SamlXmlWriterTests
{
    [Fact]
    public void WriteAuthnRequest_Minimal()
    {
        AuthnRequest authnRequest = new()
        {
            IssueInstant = new DateTime(2023, 09, 14, 15, 23, 47, DateTimeKind.Utc),
        };

        var subject = new SamlXmlWriter();

        var actual = subject.Write(authnRequest);

        var xml =
            $"<samlp:AuthnRequest xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\" " +
            $"ID=\"{authnRequest.Id}\" IssueInstant=\"2023-09-14T15:23:47Z\" Version=\"2.0\"/>";

        var expected = new XmlDocument();
        expected.LoadXml(xml);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WriteAuthnRequest_Everything()
    {
        AuthnRequest authnRequest = new()
        {
            IssueInstant = new DateTime(2023, 09, 14, 15, 23, 47, DateTimeKind.Utc),
            AssertionConsumerServiceUrl = "https://sp.example.com/acs",
            Issuer = new("https://sp.example.com/Metadata")
        };

        var subject = new SamlXmlWriter();

        var actual = subject.Write(authnRequest);

        var xml =
            $"<samlp:AuthnRequest xmlns:samlp=\"{Constants.Namespaces.SamlpUri}\" " +
            $"ID=\"{authnRequest.Id}\" IssueInstant=\"2023-09-14T15:23:47Z\" Version=\"2.0\" " +
            $"AssertionConsumerServiceURL=\"https://sp.example.com/acs\">" +
            $"<saml:Issuer xmlns:saml=\"{Constants.Namespaces.SamlUri}\">https://sp.example.com/Metadata</saml:Issuer>" +
            $"</samlp:AuthnRequest>";

        var expected = new XmlDocument();
        expected.LoadXml(xml);

        actual.Should().BeEquivalentTo(expected);
    }
}
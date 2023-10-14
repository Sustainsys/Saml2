using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Samlp;
public class SamlpSerializerTests
{
    [Fact]
    public void WriteAuthnRequest_Minimal()
    {
        var authnRequest = new AuthnRequest
        {
            IssueInstant = new(2023, 09, 14, 15, 23, 47, TimeSpan.Zero),
        };

        var subject = new SamlpSerializer(new SamlSerializer());

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
        var authnRequest = new AuthnRequest
        {
            IssueInstant = new(2023, 09, 14, 15, 23, 47, TimeSpan.Zero),
            AssertionConsumerServiceUrl = "https://sp.example.com/acs",
            Issuer = new("https://sp.example.com/Metadata")
        };

        var subject = new SamlpSerializer(new SamlSerializer());

        var actual = subject.Write(authnRequest);

        var xml = 
            $"<samlp:AuthnRequest xmlns:samlp=\"{Constants.Namespaces.Samlp}\" " +
            $"ID=\"{authnRequest.Id}\" IssueInstant=\"2023-09-14T15:23:47Z\" Version=\"2.0\" " +
            $"AssertionConsumerServiceURL=\"https://sp.example.com/acs\">" +
            $"<saml:Issuer xmlns:saml=\"{Constants.Namespaces.Saml}\">https://sp.example.com/Metadata</saml:Issuer>" +
            $"</samlp:AuthnRequest>";

        var expected = new XmlDocument();
        expected.LoadXml(xml);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadSamlResponse_MinimalErrorRequester()
    {

    }
}

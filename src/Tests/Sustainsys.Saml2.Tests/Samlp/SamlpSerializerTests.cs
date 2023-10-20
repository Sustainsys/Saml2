using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Samlp;
public class SamlpSerializerTests
{
    [Fact]
    public void WriteAuthnRequest_Minimal()
    {
        var authnRequest = new AuthnRequest
        {
            IssueInstant = new(2023, 09, 14, 15, 23, 47, DateTimeKind.Utc),
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
            IssueInstant = new(2023, 09, 14, 15, 23, 47, DateTimeKind.Utc),
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
        var source = TestData.GetXmlTraverser<SamlpSerializerTests>();

        var subject = new SamlpSerializer(new SamlSerializer());

        var actual = subject.ReadSamlResponse(source);

        var expected = new SamlResponse()
        {
            Id = "x123",
            IssueInstant = new DateTime(2023, 10, 14, 13, 46, 32, DateTimeKind.Utc),
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Requester
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("./@ID", ErrorReason.MissingAttribute)]
    [InlineData("./@Version", ErrorReason.MissingAttribute)]
    [InlineData("./@IssueInstant", ErrorReason.MissingAttribute)]
    [InlineData("./samlp:Status", ErrorReason.MissingElement)]
    [InlineData("./samlp:Status/samlp:StatusCode", ErrorReason.MissingElement)]
    [InlineData("./samlp:Status/samlp:StatusCode/@Value", ErrorReason.MissingAttribute)]
    public void ReadSamlResponse_MissingMandatory(string removeXPath, ErrorReason expectedError)
    {
        var source = TestData.GetXmlTraverser<SamlpSerializerTests>(nameof(ReadSamlResponse_MinimalErrorRequester));

        var deleteNode = source.CurrentNode!.SelectSingleNode(removeXPath, source.CurrentNode.GetNsMgr());
        
        if(deleteNode is XmlAttribute attribute)
        {
            attribute.OwnerElement!.RemoveAttributeNode(attribute);
        }
        else
        {
            deleteNode!.ParentNode!.RemoveChild(deleteNode);
        }
        
        var subject = new SamlpSerializer(new SamlSerializer());

        subject.Invoking(s => s.ReadSamlResponse(source))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(expectedError);
    }

    // Test that a response with all optional content present in the Response can be read. The embedded
    // assertion is minimal. This doesn't mean that we  actual read everything, a lot of rarely used
    // stuff is just ignored.
    [Fact]
    public void ReadSamlResponse_CanReadCompleteResponseWithAssertion()
    {
        var source = TestData.GetXmlTraverser<SamlpSerializerTests>();
        ((XmlElement)source.CurrentNode!).Sign(
            TestData.Certificate, source.CurrentNode![Constants.Elements.Issuer, Constants.Namespaces.Saml]!);
        
        var subject = new SamlpSerializer(new SamlSerializer());

        var actual = subject.ReadSamlResponse(source);

        var expected = new SamlResponse
        {
            Id = "x123",
            InResponseTo = "x789",
            Version = "2.0",
            IssueInstant = new DateTime(2023, 10, 14, 13, 46, 32, DateTimeKind.Utc),
            Destination = "https://sp.example.com/Saml2/Acs",
            Issuer = "https://idp.example.com/Metadata",
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Success
                }
            },
            Extensions = new()
        };

        actual.Should().BeEquivalentTo(expected);
    }
}

using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Serialization;
partial class SamlXmlReaderTests
{
    [Fact]
    public void ReadResponse_MinimalErrorRequester()
    {
        var source = GetXmlTraverser();

        var subject = new SamlXmlReader();

        var actual = subject.ReadResponse(source);

        var expected = new Response()
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
    public void ReadResponse_MissingMandatory(string removeXPath, ErrorReason expectedError)
    {
        var source = GetXmlTraverser(nameof(ReadResponse_MinimalErrorRequester));

        var deleteNode = source.CurrentNode!.SelectSingleNode(removeXPath, source.CurrentNode.GetNsMgr());

        if (deleteNode is XmlAttribute attribute)
        {
            attribute.OwnerElement!.RemoveAttributeNode(attribute);
        }
        else
        {
            deleteNode!.ParentNode!.RemoveChild(deleteNode);
        }

        var subject = new SamlXmlReader();

        subject.Invoking(s => s.ReadResponse(source))
            .Should().Throw<SamlXmlException>()
            .WithErrors(expectedError);
    }

    // Test that a response with all optional content present in the Response can be read. This doesn't
    // mean that we actually read everything, a lot of rarely used stuff is just ignored (for now)
    [Fact]
    public void ReadResponse_CanReadCompleteResponse()
    {
        var source = GetXmlTraverser();
        ((XmlElement)source.CurrentNode!).Sign(
            TestData.Certificate, source.CurrentNode![Constants.Elements.Issuer, Constants.Namespaces.SamlUri]!);

        var subject = new SamlXmlReader();

        var actual = subject.ReadResponse(source);

        var expected = new Response
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
            Extensions = new(),
            Assertions =
            {
                new()
                {
                    Version = "2.42",
                    Id = "_0f9174fb-a286-43cf-93c8-197dfc6c58d2",
                    IssueInstant = new DateTime(2024,02,12,13,00,53,DateTimeKind.Utc),
                    Issuer = "https://idp.example.com/Metadata",
                    Subject = new()
                    {
                        NameId = "x123456",
                        SubjectConfirmation = new()
                        {
                            Method = "urn:oasis:names:tc:SAML:2.0:cm:bearer",
                            SubjectConfirmationData = new()
                            {
                                NotOnOrAfter = new DateTime(2024,02,12,13,02,53,DateTimeKind.Utc),
                                Recipient = "https://sp.example.com/Saml2/Acs"
                            }
                        }
                    },
                    Conditions = new()
                    {
                        AudienceRestrictions =
                        {
                            new()
                            {
                                Audiences = { "https://sp.example.com/Saml2" }
                            }
                        }
                    },
                    AuthnStatement = new()
                    {
                        AuthnInstant = new DateTime(2024,2,12,13,0,53,DateTimeKind.Utc),
                        SessionIndex = "42",
                        AuthnContext = new()
                        {
                            AuthnContextClassRef = "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified"
                        }
                    },
                    Attributes = { { "organisation", "Sustainsys AB"} }
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadResponse_SignedWithoutIssuerFails()
    {
        // Profiles 4.1.4.2: If the <Response> message is signed or if an enclosed assertion is
        // encrypted, then the <Issuer> element MUST be present.

        var source = GetXmlTraverser(nameof(ReadResponse_MinimalErrorRequester));

        var documentElement = (XmlElement)source.CurrentNode!;

        documentElement.Sign(TestData.Certificate);

        // We don't have any helper method to insert the signature first, just move the node.
        var signatureNode = documentElement["Signature", SignedXml.XmlDsigNamespaceUrl]!;
        documentElement.RemoveChild(signatureNode);
        documentElement.InsertBefore(signatureNode, documentElement.FirstChild!);

        var subject = new SamlXmlReader();

        subject.Invoking(s => s.ReadResponse(source))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.MissingElement);
    }

    // TODO: Test and addition of error callback.
}
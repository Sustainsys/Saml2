using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Tests.Xml;
using Sustainsys.Saml2.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2.Metadata;

namespace Sustainsys.Saml2.Tests.Metadata;

public partial class MetadataSerializerTests
{
    [Fact]
    public void ReadEntityDescriptor_Mandatory()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer().ReadEntityDescriptor(xmlTraverser);

        var expected = new TrustedData<EntityDescriptor>(
            TrustLevel.None,
            new EntityDescriptor
            {
                EntityId = "https://stubidp.sustainsys.com/Metadata",
                RoleDescriptors =
                {
                    new RoleDescriptor
                    {
                        ProtocolSupportEnumeration = "urn:whatever"
                    }
                }
            });

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingEntityId()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.MissingAttribute);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesNamespace()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.UnexpectedNamespace);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesLocalName()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.UnexpectedLocalName)
            .WithMessage("*name*EntityDescriptor*");
    }

    [Fact]
    public void ReadEntityDescriptor_OptionalAttributes()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer().ReadEntityDescriptor(xmlTraverser);

        var expected = new TrustedData<EntityDescriptor>(
            TrustLevel.None,
            new EntityDescriptor
            {
                EntityId = "https://stubidp.sustainsys.com/Metadata",
                Id = "_eb83b59a-572a-480b-b36c-e3a3edfd92d0",
                CacheDuraton = TimeSpan.FromMinutes(15),
                ValidUntil = new DateTime(2022, 03, 15, 20, 47, 00, DateTimeKind.Utc),
                RoleDescriptors =
                {
                    new RoleDescriptor
                    {
                        ProtocolSupportEnumeration = "urn:whatever"
                    }
                }
            });

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingChildren()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.MissingElement);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidateSignature()
    {
        var xmlTraverser = GetXmlTraverser();
        var signingKeys = new[]
        {
            new SigningKey
            {
                Certificate = new X509Certificate2("stubidp.sustainsys.com.cer"),
                TrustLevel = TrustLevel.ConfiguredKey
            }
        };

        var actual = new MetadataSerializer
        {
            TrustedSigningKeys = signingKeys,
            AllowedHashAlgorithms = SignedXmlHelperTests.allowedHashes
        }
            .ReadEntityDescriptor(xmlTraverser);

        actual.TrustLevel.Should().Be(TrustLevel.ConfiguredKey);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidateSignature_ReportsError()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer
        {
            TrustedSigningKeys = TestData.SingleSigningKey,
            AllowedHashAlgorithms = SignedXmlHelperTests.allowedHashes
        }
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.SignatureFailure);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidateElementName()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithErrors(ErrorReason.UnexpectedLocalName, ErrorReason.UnexpectedNamespace)
            .WithMessage("Unexpected*metadata*");
    }

    [Fact]
        public void ReadEntityDescriptor_ReadsExtensions()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }

    [Fact]
    public void ReadEntityDescriptor_ReadsNonSupportedRoleDescriptors()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }

    [Fact]
    public void ReadEntityDescriptor_ADFS()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }
}

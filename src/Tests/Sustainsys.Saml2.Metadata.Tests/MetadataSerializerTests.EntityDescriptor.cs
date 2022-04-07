using FluentAssertions;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Tests.Xml;
using Sustainsys.Saml2.Metadata.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests;

public partial class MetadataSerializerTests
{
    [Fact]
    public void ReadEntityDescriptor_Mandatory()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer(null, null).ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata"
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingEntityId()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer(null, null).Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .Where(ex => ex.Errors.Single().Reason == ErrorReason.MissingAttribute);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesNamespace()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer(null, null).Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithMessage("*namespace*");
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesLocalName()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer(null, null).Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithMessage("*name*EntityDescriptor*");
    }

    [Fact]
    public void ReadEntityDescriptor_OptionalAttributes()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new MetadataSerializer(null, null).ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata",
            Id = "_eb83b59a-572a-480b-b36c-e3a3edfd92d0",
            CacheDuraton = TimeSpan.FromMinutes(15),
            ValidUntil = new DateTime(2022, 03, 15, 20, 47, 00, DateTimeKind.Utc)
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingChildren()
    {
        var xmlTraverser = GetXmlTraverser();

        new MetadataSerializer(null, null).Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .Where(ex => ex.Errors.Single().Reason == ErrorReason.MissingElement);
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

        var actual = new MetadataSerializer(signingKeys, SignedXmlHelperTests.allowedHashes)
            .ReadEntityDescriptor(xmlTraverser);

        actual.TrustLevel.Should().Be(TrustLevel.ConfiguredKey);
    }

    [Fact(Skip = "Not implemented yet")]
    public void ReadEntityDescriptor_ValidateElementName()
    {
        // Bail out in a good way if the element name is not matching. Probably
        // need to add a return value to EnsureName indicating if it was successful
        // to be able to skip over the element if it's not the right one.
    }
}

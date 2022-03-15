using FluentAssertions;
using Sustainsys.Saml2.Metadata.Elements;
using Sustainsys.Saml2.Metadata.Xml;
using Sustainsys.Saml2.Tests.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Xml;
using Xunit;

namespace Sustainsys.Saml2.Metadata.Tests;

public class MetadataSerializerTests
{
    private XmlTraverser GetXmlTraverser([CallerMemberName] string? fileName = null)
        => TestData.GetXmlTraverser<MetadataSerializerTests>(fileName);

    [Fact]
    public void ReadEntityDescriptor_EntityId()
    {
        var xmlTraverser = GetXmlTraverser();

        var subject = new MetadataSerializer();

        var actual = subject.ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata"
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesNamespace()
    {
        var xmlTraverser = GetXmlTraverser();

        var subject = new MetadataSerializer();

        subject.Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithMessage("*namespace*");
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesLocalName()
    {
        var xmlTraverser = GetXmlTraverser();

        var subject = new MetadataSerializer();

        subject.Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<Saml2XmlException>()
            .WithMessage("*name*EntityDescriptor*");
    }

    [Fact]
    public void ReadEntityDescriptor_OptionalAttributes()
    {
        var xmlTraverser = GetXmlTraverser();

        var subject = new MetadataSerializer();

        var actual = subject.ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata",
            Id = "_eb83b59a-572a-480b-b36c-e3a3edfd92d0",
            CacheDuraton = TimeSpan.FromMinutes(15),
            ValidUntil = new DateTime(2022, 03, 15, 20, 47, 00, DateTimeKind.Utc)
        };

        actual.Should().BeEquivalentTo(expected);
    }
}

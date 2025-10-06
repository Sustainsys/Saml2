// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Tests.Xml;
using Sustainsys.Saml2.Xml;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Tests.Serialization;

public partial class SamlXmlReaderTests
{
    [Fact]
    public void ReadEntityDescriptor_Mandatory()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new SamlXmlReader().ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata",
            RoleDescriptors =
            {
                new RoleDescriptor
                {
                    ProtocolSupportEnumeration = "urn:whatever"
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingEntityId()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.MissingAttribute);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesNamespace()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.UnexpectedNamespace);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidatesLocalName()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.UnexpectedLocalName)
            .WithMessage("*name*EntityDescriptor*");
    }

    [Fact]
    public void ReadEntityDescriptor_OptionalAttributes()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new SamlXmlReader().ReadEntityDescriptor(xmlTraverser);

        var expected = new EntityDescriptor
        {
            EntityId = "https://stubidp.sustainsys.com/Metadata",
            Id = "_eb83b59a-572a-480b-b36c-e3a3edfd92d0",
            CacheDuraton = TimeSpan.FromMinutes(15),
            ValidUntil = new(2022, 03, 15, 20, 47, 00),
            RoleDescriptors =
            {
                new RoleDescriptor
                {
                    ProtocolSupportEnumeration = "urn:whatever"
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadEntityDescriptor_MissingChildren()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader().Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
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
#if NET9_0_OR_GREATER
                Certificate = X509CertificateLoader.LoadCertificateFromFile("stubidp.sustainsys.com.cer"),
#else
                Certificate = new X509Certificate2("stubidp.sustainsys.com.cer"),
#endif
                TrustLevel = TrustLevel.ConfiguredKey
            }
        };

        var actual = new SamlXmlReader
        {
            TrustedSigningKeys = signingKeys,
            AllowedAlgorithms = SignedXmlHelperTests.allowedHashes
        }
        .ReadEntityDescriptor(xmlTraverser);

        actual.TrustLevel.Should().Be(TrustLevel.ConfiguredKey);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidateSignature_ReportsError()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new SamlXmlReader
        {
            TrustedSigningKeys = TestData.SingleSigningKey,
            AllowedAlgorithms = SignedXmlHelperTests.allowedHashes
        }
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.SignatureFailure);
    }

    [Fact]
    public void ReadEntityDescriptor_ValidateElementName()
    {
        var xmlTraverser = GetXmlTraverser();

        var actual = new SamlXmlReader()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.UnexpectedLocalName, ErrorReason.UnexpectedNamespace)
            .WithMessage("Unexpected*metadata*");
    }

    [Fact]
    public void ReadEntityDescriptor_ReadsExtensions()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }

    [Fact]
    public void ReadEntityDescriptor_ReadsNonSupportedRoleDescriptors()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }

    [Fact]
    public void ReadEntityDescriptor_ADFS()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().NotThrow();
    }

    [Fact]
    public void ReadEntityDescriptor_WrongNamespaceRoleDescriptor()
    {
        var xmlTraverser = GetXmlTraverser();

        new SamlXmlReader()
            .Invoking(s => s.ReadEntityDescriptor(xmlTraverser))
            .Should().Throw<SamlXmlException>()
            .WithErrors(ErrorReason.UnexpectedNamespace);
    }
}
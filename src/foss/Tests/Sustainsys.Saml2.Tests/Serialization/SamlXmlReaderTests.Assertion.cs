// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System.Xml;

namespace Sustainsys.Saml2.Tests.Serialization;

public partial class SamlXmlReaderTests
{
    [Fact]
    public void ReadAssertion_Mandatory()
    {
        var source = GetXmlTraverser();

        var subject = new SamlXmlReader();

        var actual = subject.ReadAssertion(source);

        var expected = new Assertion
        {
            Id = "a9329",
            Issuer = "https://idp.example.com/Saml2",
            Version = "2.42", // For the purpose of this test to ensure value is read.
            IssueInstant = new(2024, 02, 03, 18, 24, 14),
            Subject = new()
            {
                NameId = new()
                {
                    Value = "x987654"
                }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("./@ID", ErrorReason.MissingAttribute)]
    [InlineData("./@Version", ErrorReason.MissingAttribute)]
    [InlineData("./@IssueInstant", ErrorReason.MissingAttribute)]
    [InlineData("./saml:Issuer", ErrorReason.UnexpectedLocalName)]
    [InlineData("./saml:Subject", ErrorReason.UnexpectedLocalName)]
    [InlineData("./saml:AttributeStatement/saml:Attribute", ErrorReason.MissingElement)]
    [InlineData("./saml:Subject/saml:SubjectConfirmation/@Method", ErrorReason.MissingAttribute)]
    public void ReadAssertion_MissingMandatory(string removeXPath, ErrorReason expectedError)
    {
        var source = GetXmlTraverser();

        DeleteNode(removeXPath, source);

        var subject = new SamlXmlReader();

        subject.Invoking(s => s.ReadAssertion(source))
            .Should().Throw<SamlXmlException>()
            .WithErrors(expectedError);
    }

    private static void DeleteNode(string removeXPath, XmlTraverser source)
    {
        var deleteNodes = source.CurrentNode!.SelectNodes(removeXPath, source.CurrentNode.GetNsMgr())!;

        if (deleteNodes.Count == 0)
        {
            throw new InvalidOperationException("Didn't find any node to delete.");
        }

        foreach (XmlNode deleteNode in deleteNodes)
        {
            if (deleteNode is XmlAttribute attribute)
            {
                attribute.OwnerElement!.RemoveAttributeNode(attribute);
            }
            else
            {
                deleteNode!.ParentNode!.RemoveChild(deleteNode);
            }
        }
    }

    [Fact]
    public void ReadAssertion_ErrorCallback()
    {
        var source = GetXmlTraverser(nameof(ReadAssertion_Mandatory));

        DeleteNode("./@IssueInstant", source);

        var subject = new SamlXmlReader();

        var errorInspectorCalled = false;

        void errorInspector(ReadErrorInspectorContext<Assertion> context)
        {
            context.Data.Id.Should().Be("a9329");

            var xmlSourceElement = context.XmlSource as XmlElement;
            xmlSourceElement.Should().NotBeNull();
            xmlSourceElement!.GetAttribute("ID").Should().Be("a9329");

            context.Errors.Count.Should().Be(1);
            var error = context.Errors.Single();
            error.Node.Should().BeSameAs(context.XmlSource);
            error.LocalName.Should().Be("IssueInstant");
            error.Reason.Should().Be(ErrorReason.MissingAttribute);
            error.Ignore.Should().BeFalse();

            error.Ignore = true;

            errorInspectorCalled = true;
        }

        subject.ReadAssertion(source, errorInspector);

        errorInspectorCalled.Should().BeTrue();
    }

    [Fact]
    public void ReadAssertion_CanReadOptional()
    {
        // TODO: Go through spec and check that there are tests for minimal/maximal
        // sub-contents. E.g. a minimal SubjectConfirmationData

        var source = GetXmlTraverser();

        ((XmlElement)source.CurrentNode!).Sign(
            TestData.Certificate, source.CurrentNode![Constants.Elements.Issuer, Constants.Namespaces.SamlUri]!);

        var subject = new SamlXmlReader();

        var actual = subject.ReadAssertion(source, null);

        var expected = new Assertion()
        {
            Version = "2.42",
            Id = "a9329",
            IssueInstant = new(2024, 2, 3, 18, 24, 14),
            Issuer = "https://idp.example.com/Saml2",
            Subject = new()
            {
                NameId = "x987654",
                SubjectConfirmation = new()
                {
                    Method = Constants.SubjectConfirmationMethods.Bearer,
                    SubjectConfirmationData = new()
                    {
                        NotBefore = new(2024, 2, 10, 17, 45, 14),
                        NotOnOrAfter = new(2024, 2, 10, 17, 50, 14),
                        Recipient = "https://sp.example.com/Saml2/Acs",
                        InResponseTo = "b123456",
                        Address = "192.168.42.17"
                    }
                }
            },
            Conditions = new()
            {
                NotBefore = new(2024, 2, 10, 17, 45, 14),
                AudienceRestrictions =
                {
                    new()
                    {
                        Audiences =
                        {
                            "https://sp.example.com/Saml2",
                            "https://other.example.com/Saml2"
                        }
                    }
                },
                OneTimeUse = true
            },
            AuthnStatement = new()
            {
                AuthnInstant = new(2024, 2, 10, 15, 27, 34),
                SessionIndex = "42",
                SessionNotOnOrAfter = new(2024, 2, 10, 19, 45, 14),
                AuthnContext = new()
                {
                    AuthnContextClassRef = Constants.AuthnContextClasses.PasswordProtectedTransport
                }
            },
            Attributes =
            {
                { "role", "coder", "OSS Maintainer" },
                { "organisation", "Sustainsys AB" },
                { "role", "bug-slayer" },
                { "NullAttribute1", (string?)null },
                { "NullAttribute2", (string?)null },
                { "NotNullAttribute", "" }
            }
        };

        actual.Should().BeEquivalentTo(expected);
    }
}
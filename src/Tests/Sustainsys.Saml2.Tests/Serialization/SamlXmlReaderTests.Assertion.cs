using FluentAssertions.Equivalency.Steps;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Serialization;
using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
            IssueInstance = new DateTime(2024, 02, 03, 18, 24, 14, DateTimeKind.Utc),
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
    [InlineData("./saml:Subject", ErrorReason.MissingElement)]
    public void ReadAssertion_MissingMandatory(string removeXPath, ErrorReason expectedError)
    {
        var source = GetXmlTraverser(nameof(ReadAssertion_Mandatory));

        DeleteNode(removeXPath, source);

        var subject = new SamlXmlReader();

        subject.Invoking(s => s.ReadAssertion(source))
            .Should().Throw<SamlXmlException>()
            .WithErrors(expectedError);
    }

    private static void DeleteNode(string removeXPath, XmlTraverser source)
    {
        var deleteNode = source.CurrentNode!.SelectSingleNode(removeXPath, source.CurrentNode.GetNsMgr());

        if (deleteNode is XmlAttribute attribute)
        {
            attribute.OwnerElement!.RemoveAttributeNode(attribute);
        }
        else
        {
            deleteNode!.ParentNode!.RemoveChild(deleteNode);
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
}

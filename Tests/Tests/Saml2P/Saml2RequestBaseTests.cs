using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml2P;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    class ConcreteSaml2Request : Saml2RequestBase
    {
        public XElement ToXElement()
        {
            var x = new XElement("Foo");
            x.Add(ToXNodes());
            return x;
        }

        public override string ToXml()
        {
            return ToXElement().ToString();
        }

        protected override string LocalName
        {
            get { return "ConcreteRequest"; }
        }

        public void TestReadBasePropertiesWithNull()
        {
            ReadBaseProperties(null);
        }
    }

    [TestClass]
    public class Saml2RequestBaseTests
    {
        [TestMethod]
        public void Saml2RequestBase_Id_IsUnique()
        {
            var r1 = new ConcreteSaml2Request();
            var r2 = new ConcreteSaml2Request();

            r1.Id.Should().NotBe(r2.Id);
        }

        [TestMethod]
        public void Saml2RequestBase_Id_IsValidXsId()
        {
            var id = new ConcreteSaml2Request().Id.Value;

            Regex.IsMatch(id, "[^:0-9][^:]*").Should().BeTrue();
        }

        [TestMethod]
        public void Saml2RequestBase_Version()
        {
            new ConcreteSaml2Request().Version.Should().Be("2.0");
        }

        [TestMethod]
        public void Saml2RequestBase_IssueInstant_Format()
        {
            var issueInstant = new ConcreteSaml2Request().IssueInstant;

            Regex.IsMatch(issueInstant,
                "20[0-9]{2}-((0[0-9])|(1[0-2]))-(([0-2][0-9])|(3[0-1]))T(([01][0-9])|(2[0-3]))(:[0-5][0-9]){2}Z")
                .Should().BeTrue();
        }

        [TestMethod]
        public void Saml2RequestBase_IssueInstant_IsNow()
        {
            var issueInstant = new ConcreteSaml2Request().IssueInstant;

            var parsed = DateTime.Parse(issueInstant).ToUniversalTime();

            parsed.Should().BeCloseTo(DateTime.UtcNow, 1200);
            parsed.Kind.Should().Be(DateTimeKind.Utc);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Id()
        {
            var r = new ConcreteSaml2Request();

            r.ToXElement().Attribute("ID").Should().NotBeNull()
                .And.Subject.Value.Should().Be(r.Id.Value);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_IssueInstant()
        {
            var r = new ConcreteSaml2Request();
            r.ToXElement().Attribute("IssueInstant").Should().NotBeNull()
                .And.Subject.Value.Should().Be(r.IssueInstant);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Version()
        {
            var r = new ConcreteSaml2Request();
            r.ToXElement().Attribute("Version").Should().NotBeNull()
                .And.Subject.Value.Should().Be(r.Version);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Saml2PNamespacePrefix()
        {
            var r = new ConcreteSaml2Request();
            r.ToXElement().GetPrefixOfNamespace(Saml2Namespaces.Saml2P).Should().Be("saml2p");
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Destination()
        {
            var uri = "http://idp.example.com/";
            var r = new ConcreteSaml2Request() { DestinationUrl = new Uri(uri) };

            r.ToXElement().Attribute("Destination").Should().NotBeNull()
                .And.Subject.Value.Should().Be(uri);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Issuer()
        {
            var uri = "http://sp.example.com/";
            var r = new ConcreteSaml2Request() { Issuer = new EntityId(uri) };

            r.ToXElement().Element(Saml2Namespaces.Saml2+ "Issuer").Value.Should().Be(uri);
        }

        [TestMethod]
        public void Saml2RequestBase_ToXNodes_Saml2NamespacePrefix()
        {
            var r = new ConcreteSaml2Request();
            r.ToXElement().GetPrefixOfNamespace(Saml2Namespaces.Saml2Name).Should().Be("saml2");
        }

        [TestMethod]
        public void Saml2RequestBase_MessageName()
        {
            var subject = new ConcreteSaml2Request();
            subject.MessageName.Should().Be("SAMLRequest");
        }

        [TestMethod]
        public void Saml2RequestBase_ReadBasePropertiesWithNullArgument_ShouldThrow()
        {
            var subject = new ConcreteSaml2Request();
            Action a = () => subject.TestReadBasePropertiesWithNull();
            a.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("xml");
        }
    }
}

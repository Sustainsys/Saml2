using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Metadata;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests.Saml2P
{
    [TestClass]
    public class Saml2IdpEntryTests
    {
        [TestMethod]
        public void Saml2IdpEntry_NullCheckProviderId()
        {
            var subject = new Saml2IdpEntry(new EntityId("urn:foo"));

            subject.Invoking(s => s.ProviderId = null)
                .ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Saml2IdpEntry_ToXElement_OnlyProviderId()
        {
            var subject = new Saml2IdpEntry(new EntityId("urn:foo"))
            {
                Location = null,
                Name = null
            };

            var actual = subject.ToXElement();

            var expected = new XElement(Saml2Namespaces.Saml2P + "IDPEntry",
                new XAttribute("ProviderID", "urn:foo"));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}

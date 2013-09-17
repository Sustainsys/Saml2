using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Xml.Linq;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2AuthenticationRequestTests
    {
        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_RootNode()
        {
            var x = new Saml2AuthenticationRequest().ToXElement();

            x.Should().NotBeNull().And.Subject.Name.Should().Be(
                Saml2Namespaces.Saml2P + "AuthnRequest");
        }

        [TestMethod]
        public void Saml2AuthenticationRequest_ToXElement_AddsRequestBaseFields()
        {
            // Just checking for the id field and assuming that means that the
            // base fields are added. The details of the fields are tested
            // by Saml2RequestBaseTests.

            var x = new Saml2AuthenticationRequest().ToXElement();

            x.Should().NotBeNull().And.Subject.Attribute("ID").Should().NotBeNull();
        }
    }
}

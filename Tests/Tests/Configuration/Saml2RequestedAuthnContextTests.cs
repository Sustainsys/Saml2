using FluentAssertions;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class Saml2RequestedAuthnContextTests
    {
        [TestMethod]
        public void Saml2RequestedAuthnContext_Ctor_Nullcheck()
        {
            Action a = () => new Saml2RequestedAuthnContext(null);

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("requestedAuthnContextElement");
        }

        [TestMethod]
        public void Saml2RequestedAuthnContext_Ctor_HandlesFullUri()
        {
            var config = new RequestedAuthnContextElement();
            config.AllowChange = true;
            var classRef = "http://id.sambi.se/loa2";
            config.AuthnContextClassRef = classRef;

            var subject = new Saml2RequestedAuthnContext(config);
            subject.ClassRef.Should().Be(classRef);
        }

        [TestMethod]
        public void Saml2RequestedAuthnContext_Ctor_HandlesEmpty()
        {
            var config = new RequestedAuthnContextElement();

            var subject = new Saml2RequestedAuthnContext(config);

            subject.ClassRef.Should().BeNull();
        }

        [TestMethod]
        public void Saml2ReqestedAuthnContext_Ctor()
        {
            var classRef = "http://id.sambi.se/loa2";
            var subject = new Saml2RequestedAuthnContext(new Uri(classRef), AuthnContextComparisonType.Maximum);

            subject.ClassRef.OriginalString.Should().Be(classRef);
            subject.Comparison.Should().Be(AuthnContextComparisonType.Maximum);
        }
    }
}

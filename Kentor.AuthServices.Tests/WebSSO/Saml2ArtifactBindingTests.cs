using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;

namespace Kentor.AuthServices.Tests.WebSSO
{
    [TestClass]
    public class Saml2ArtifactBindingTests
    {
        [TestMethod]
        public void Saml2ArtifactBinding_CanUnbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.Artifact)
                .Invoking(b => b.CanUnbind(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2ArtifactBinding_Unbind_Nullcheck()
        {
            Saml2Binding.Get(Saml2BindingType.Artifact)
                .Invoking(b => b.Unbind(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("request");
        }

        [TestMethod]
        public void Saml2ArtifactBinding_Unbind_FromGet()
        {
            var r = new HttpRequestData(
                "GET",
                new Uri("http://example.com/path/acs?SAMLart=Foo&RelayState=Bar"));

            var result = Saml2Binding.Get(Saml2BindingType.Artifact).Unbind(r);

            var expected = new UnbindResult(null, "Bar");

            result.ShouldBeEquivalentTo(expected);
        }
    }
}

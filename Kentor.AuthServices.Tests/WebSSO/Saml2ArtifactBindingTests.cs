using Kentor.AuthServices.WebSso;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;

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

        [TestMethod]
        public void Saml2ArtifactBinding_Unbind_FromPost()
        {
            var r = new HttpRequestData(
                "POST",
                new Uri("http://example.com"),
                "/ModulePath",
                new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLart", new[] { "artifact" }),
                    new KeyValuePair<string, string[]>("RelayState", new[] {"MyState"})
                });

            var result = Saml2Binding.Get(Saml2BindingType.Artifact).Unbind(r);

            var expected = new UnbindResult(null, "MyState");

            result.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void Saml2ArtifactBinding_Unbind_ThrowsOnUnknownHttpMethod()
        {
            var r = new HttpRequestData("PUT", new Uri("http://host"));

            Saml2Binding.Get(Saml2BindingType.Artifact)
                .Invoking(b => b.Unbind(r))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("Artifact binding can only use GET or POST http method, but found PUT");
        }
    }
}

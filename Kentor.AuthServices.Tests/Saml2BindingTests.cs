using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Collections.Specialized;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2BindingTests
    {
        [TestMethod]
        public void Saml2Binding_Get_ReturnsSaml2Postbinding()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", "Some Data" } });

            Saml2Binding.Get(r).Should().BeOfType<Saml2PostBinding>();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainGet()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("GET");

            Saml2PostBinding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainPost()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection());

            Saml2PostBinding.Get(r).Should().BeNull();
        }

        class ConcreteSaml2Binding : Saml2Binding
        { }

        [TestMethod]
        public void Saml2Binding_Bind_IsNotImplemented()
        {
            Action a = () => new ConcreteSaml2Binding().Bind(null);

            a.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void Saml2Binding_Unbind_IsNotImplemented()
        {
            Action a = () => new ConcreteSaml2Binding().Unbind<Saml2Response>(null);

            a.ShouldThrow<NotImplementedException>();
        }
    }
}

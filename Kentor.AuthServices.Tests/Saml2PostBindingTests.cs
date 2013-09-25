using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using FluentAssertions;
using System.Collections.Specialized;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2PostBindingTests
    {
        [TestMethod]
        public void Saml2PostBinding_CanUnbind_FalseOnNotPost()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("GET");

            new Saml2PostBinding().CanUnbind(r).Should().BeFalse();
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_FalseOnNoSamlRespose()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection());

            new Saml2PostBinding().CanUnbind(r).Should().BeFalse();
        }

        [TestMethod]
        public void Saml2PostBinding_CanUnbind_True()
        {
            var r = Substitute.For<HttpRequestBase>();
            r.HttpMethod.Returns("POST");
            r.Form.Returns(new NameValueCollection() { { "SAMLResponse", "someData" } });

            new Saml2PostBinding().CanUnbind(r).Should().BeTrue();
        }
    }
}

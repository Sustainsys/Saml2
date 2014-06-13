using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Collections.Specialized;
using FluentAssertions;
using System.Collections.Generic;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2BindingTests
    {
        [TestMethod]
        public void Saml2Binding_Get_ReturnsSaml2Postbinding()
        {
            var r = new HttpRequestData("POST", null, new KeyValuePair<string, string[]>[]
                {
                    new KeyValuePair<string, string[]>("SAMLResponse", new string[] { "Some Data" })
                });

            Saml2Binding.Get(r).Should().BeOfType<Saml2PostBinding>();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainGet()
        {
            var r = new HttpRequestData("GET", null);
            
            Saml2PostBinding.Get(r).Should().BeNull();
        }

        [TestMethod]
        public void Saml2Binding_Get_NullOnPlainPost()
        {
            var r = new HttpRequestData("GET", null);

            Saml2PostBinding.Get(r).Should().BeNull();
        }

        class ConcreteSaml2Binding : Saml2Binding
        { }

        [TestMethod]
        public void Saml2Binding_Bind_IsNotImplemented()
        {
            Action a = () => new ConcreteSaml2Binding().Bind(null, null, null);

            a.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void Saml2Binding_Unbind_IsNotImplemented()
        {
            Action a = () => new ConcreteSaml2Binding().Unbind(null);

            a.ShouldThrow<NotImplementedException>();
        }
    }
}

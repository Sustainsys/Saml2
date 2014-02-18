using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2RedirectBindingTests
    {
        [TestMethod]
        public void Saml2RedirectBinding_Nullcheck()
        {
            Action a = () => Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("message");
        }
    }
}

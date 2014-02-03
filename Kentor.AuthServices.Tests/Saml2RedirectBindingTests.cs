using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    using System.Globalization;
    using System.Threading;

    [TestClass]
    public class Saml2RedirectBindingTests
    {
        [TestInitialize]
        public void MyTestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void Saml2RedirectBinding_Nullcheck()
        {
            Action a = () => Saml2Binding.Get(Saml2BindingType.HttpRedirect)
                .Bind(null);

            a.ShouldThrow<ArgumentNullException>().WithMessage(
                "Value cannot be null.\r\nParameter name: message");
        }
    }
}

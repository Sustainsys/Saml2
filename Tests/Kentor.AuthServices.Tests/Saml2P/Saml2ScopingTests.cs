using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kentor.AuthServices.Tests.Saml2P
{
    [TestClass]
    public class Saml2ScopingTests
    {
        [TestMethod]
        public void Saml2Scoping_Ctor_ThrowsOnNegativeProxyCount()
        {
            Action a = () => new Saml2Scoping().ProxyCount = -1;

            a.ShouldThrow<ArgumentException>()
                .WithMessage("*negative*");
        }
    }
}

using FluentAssertions;
using Sustainsys.Saml2.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sustainsys.Saml2.Tests.Saml2P
{
    [TestClass]
    public class Saml2ScopingTests
    {
        [TestMethod]
        public void Saml2Scoping_Ctor_ThrowsOnNegativeProxyCount()
        {
            Action a = () => new Saml2Scoping().ProxyCount = -1;

            a.Should().Throw<ArgumentException>()
                .WithMessage("*negative*");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2PSecurityTokenHandlerTests
    {
        [TestMethod]
        public void Saml2PSecurityTokenHandler_Ctor_Nullcheck()
        {
            Action a = () => new Saml2PSecurityTokenHandler(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("spEntityId");
        }
    }
}

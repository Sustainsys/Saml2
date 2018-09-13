using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class Saml2NameIdExtensionsTests
    {
        [TestMethod]
        public void Saml2NameIdExtensions_ToXElement_Nullcheck()
        {
            Action a = () => ((Saml2NameIdentifier)null).ToXElement();

            a.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("nameIdentifier");
        }
    }
}

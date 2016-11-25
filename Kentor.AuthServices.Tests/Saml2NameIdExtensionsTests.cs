using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Tokens;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2NameIdExtensionsTests
    {
        [TestMethod]
        public void Saml2NameIdExtensions_ToXElement_Nullcheck()
        {
            Action a = () => ((Saml2NameIdentifier)null).ToXElement();

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("nameIdentifier");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using FluentAssertions;
using System.Collections.Generic;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2StatementExtensionsTests
    {
        [TestMethod]
        public void Saml2StatementExtensions_ToXElement_NullCheck()
        {
            Saml2Statement assertion = null;

            Action a = () => assertion.ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("statement");
        }

        [TestMethod]
        public void Saml2StatementExtensions_ToXElement_TypeCheck()
        {
            Saml2Statement assertion = new StubSaml2Statement();

            Action a = () => assertion.ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("statement");
        }

        private class StubSaml2Statement : Saml2Statement
        {
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class Saml2SubjectExtensionsTests
    {
        [TestMethod]
        public void Saml2SubjectExtensions_ToXElement_CheckNull()
        {
            Saml2Subject subject = null;

            Action a = () => subject.ToXElement();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("subject");
        }
    }
}

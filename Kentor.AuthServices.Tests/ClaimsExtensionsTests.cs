using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Claims;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ClaimsExtensionsTests
    {
        [TestMethod]
        public void ClaimsExtensions_ToSaml2NameIdentifier_NullCheck()
        {
            Action a = () => ((Claim)null).ToSaml2NameIdentifier();

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("nameIdClaim");
        }
    }
}

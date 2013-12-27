using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ClaimsIdentityExtensionsTests
    {
        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_Subject()
        {
            var subject = "JohnDoe";

            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, subject)
            });

            var a = ci.ToSaml2Assertion("http://idp.example.com");

            a.Subject.NameId.Value.Should().Be(subject);
        }
    }
}

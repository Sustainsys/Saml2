using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    using System.Globalization;
    using System.Threading;

    [TestClass]
    public class ClaimsIdentityExtensionsTests
    {
        [TestInitialize]
        public void MyTestInitialize()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        } 

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_ThrowsOnNullIdentity()
        {
            ClaimsIdentity identity = null;

            Action a = () => identity.ToSaml2Assertion("foo");

            a.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: identity");
        }

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

        [TestMethod]
        public void ClaimsIdentityExtensions_ToSaml2Assertion_Includes_DefaultCondition()
        {
            var ci = new ClaimsIdentity(new Claim[] { 
                new Claim(ClaimTypes.NameIdentifier, "JohnDoe")
            });

            var a = ci.ToSaml2Assertion("http://idp.example.com");

            // Default validity time is hearby defined to two minutes.
            a.Conditions.NotOnOrAfter.Value.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(2));
        }
    }
}

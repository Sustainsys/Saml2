using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Sustainsys.Saml2.Internal;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void DateTimeExtensionsTests_ToSaml2Date()
        {
            var subject = new DateTime(2014, 03, 02, 22, 42, 54, DateTimeKind.Utc)
            .ToLocalTime().ToSaml2DateTimeString();

            subject.Should().Be("2014-03-02T22:42:54Z");
        }

        [TestMethod]
        public void DateTimeExtensionsTests_ToSaml2DateStripsSecondFractions()
        {
            var subject = new DateTime(2014, 07, 14, 16, 23, 47, 153, DateTimeKind.Utc)
            .ToSaml2DateTimeString();

            subject.Should().Be("2014-07-14T16:23:47Z");
        }
    }
}

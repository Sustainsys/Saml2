using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void DateTimeExtensionsTestToSaml2Date()
        {
            var subject = new DateTime(2014, 03, 02, 22, 42, 54, DateTimeKind.Utc)
            .ToLocalTime().ToSaml2DateTimeString();

            subject.Should().Be("2014-03-02T22:42:54Z");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Tests.Internal
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

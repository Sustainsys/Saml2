using FluentAssertions;
using Kentor.AuthServices.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kentor.AuthServices.Tests.Configuration
{
    [TestClass]
    public class CompatibilityTests
    {
        [TestMethod]
        public void Compatibility_Ctor_Nullcheck()
        {
            Action a = () => new Compatibility(null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("configElement");
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class ReturnRequestedIssuerNameRegistryTests
    {
        [TestMethod]
        public void ReturnRequestedIssuerNameRegistry_GetIssuerNameRequested()
        {
            var name = "SomeName";

            new ReturnRequestedIssuerNameRegistry().GetIssuerName(
                null, name).Should().Be(name);
        }

        [TestMethod]
        public void ReturnRequestedIssuerNameRegistry_GetIssuerName()
        {
            Action a = () => new ReturnRequestedIssuerNameRegistry().GetIssuerName(null);

            a.ShouldThrow<NotImplementedException>();
        }
    }
}

using System;
using System.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class PendingAuthnRequestsTests
    {
        [TestMethod]
        public void PendingAuthnRequests_AddRemove()
        {
            var id = new Saml2Id();
            var requestIdp = "testidp";
            PendingAuthnRequests.Add(id, requestIdp);
            string responseIdp;
            PendingAuthnRequests.TryRemove(id, out responseIdp).Should().BeTrue();
            responseIdp.Should().Be(requestIdp);
        }

        [TestMethod]
        public void PendingAuthnRequests_Add_ThrowsOnExisting()
        {
            var id = new Saml2Id();
            var requestIdp = "testidp";
            PendingAuthnRequests.Add(id, requestIdp);
            Action a = () => PendingAuthnRequests.Add(id, requestIdp);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnIdNeverIssued()
        {
            var id = new Saml2Id();
            string responseIdp;
            PendingAuthnRequests.TryRemove(id, out responseIdp).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnRemovedTwice()
        {
            var id = new Saml2Id();
            var requestIdp = "testIdp";
            string responseIdp;
            PendingAuthnRequests.Add(id, requestIdp);
            PendingAuthnRequests.TryRemove(id, out responseIdp).Should().BeTrue();
            PendingAuthnRequests.TryRemove(id, out responseIdp).Should().BeFalse();
        }
    }
}

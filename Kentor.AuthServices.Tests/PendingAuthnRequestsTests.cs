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
            PendingAuthnRequests.Add(id);
            PendingAuthnRequests.TryRemove(id).Should().BeTrue();
        }

        [TestMethod]
        public void PendingAuthnRequests_Add_ThrowsOnExisting()
        {
            var id = new Saml2Id();
            PendingAuthnRequests.Add(id);
            Action a = () => PendingAuthnRequests.Add(id);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnIdNeverIssued()
        {
            var id = new Saml2Id();
            PendingAuthnRequests.TryRemove(id).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnRemovedTwice()
        {
            var id = new Saml2Id();
            PendingAuthnRequests.Add(id);
            PendingAuthnRequests.TryRemove(id).Should().BeTrue();
            PendingAuthnRequests.TryRemove(id).Should().BeFalse();
        }
    }
}

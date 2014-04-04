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
            PendingAuthnRequests.Remove(id);
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
        public void PendingAuthnRequests_Remove_ThrowsOnIdNeverIssued()
        {
            var id = new Saml2Id();
            Action a = () => PendingAuthnRequests.Remove(id);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_ThrowsOnRemovedTwice()
        {
            var id = new Saml2Id();
            PendingAuthnRequests.Add(id);
            PendingAuthnRequests.Remove(id);
            Action a = () => PendingAuthnRequests.Remove(id);
            a.ShouldThrow<InvalidOperationException>();

        }
    }
}

using System;
using System.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class PendingAuthnRequestsTests
    {
        [TestMethod]
        public void PendingAuthnRequests_AddRemove()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            var pendingAuthContainer = new PendingAuthInMemoryStorage();
            pendingAuthContainer.Add(id, requestData);
            StoredRequestState responseData;
            pendingAuthContainer.TryRemove(id, out responseData).Should().BeTrue();
            responseData.Should().Be(requestData);
            responseData.Idp.Id.Should().Be("testidp");
            responseData.ReturnUrl.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void PendingAuthnRequests_Add_ThrowsOnExisting()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            var pendingAuthContainer = new PendingAuthInMemoryStorage();
            pendingAuthContainer.Add(id, requestData);
            Action a = () => pendingAuthContainer.Add(id, requestData);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnIdNeverIssued()
        {
            var id = new Saml2Id();
            StoredRequestState responseData;
            new PendingAuthInMemoryStorage().TryRemove(id, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnRemovedTwice()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            StoredRequestState responseData;
            var pendingAuthContainer = new PendingAuthInMemoryStorage();
            pendingAuthContainer.Add(id, requestData);
            pendingAuthContainer.TryRemove(id, out responseData).Should().BeTrue();
            pendingAuthContainer.TryRemove(id, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequest_TryRemove_NullGivesNull()
        {
            Saml2Id id = null;
            StoredRequestState state;

            new PendingAuthInMemoryStorage().TryRemove(id, out state).Should().BeFalse();
            state.Should().BeNull();
        }
    }
}

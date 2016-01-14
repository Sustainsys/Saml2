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
            var relayState = SecureKeyGenerator.CreateRelayState();
            var saml2Id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"), saml2Id);
            PendingAuthnRequests.Add(relayState, requestData);
            StoredRequestState responseData;
            PendingAuthnRequests.TryRemove(relayState, out responseData).Should().BeTrue();
            responseData.Should().Be(requestData);
            responseData.Idp.Id.Should().Be("testidp");
            responseData.ReturnUrl.Should().Be("http://localhost/Return.aspx");
            responseData.MessageId.Should().Be(saml2Id);
        }

        [TestMethod]
        public void PendingAuthnRequests_Add_ThrowsOnExisting()
        {
            var relayState = SecureKeyGenerator.CreateRelayState();
            var requestData = new StoredRequestState(
                new EntityId("testidp"),
                new Uri("http://localhost/Return.aspx"),
                new Saml2Id());
            PendingAuthnRequests.Add(relayState, requestData);
            Action a = () => PendingAuthnRequests.Add(relayState, requestData);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnIdNeverIssued()
        {
            var relayState = SecureKeyGenerator.CreateRelayState();
            StoredRequestState responseData;
            PendingAuthnRequests.TryRemove(relayState, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnRemovedTwice()
        {
            var relayState = SecureKeyGenerator.CreateRelayState();
            var requestData = new StoredRequestState(
                new EntityId("testidp"),
                new Uri("http://localhost/Return.aspx"),
                new Saml2Id());

            StoredRequestState responseData;
            PendingAuthnRequests.Add(relayState, requestData);
            PendingAuthnRequests.TryRemove(relayState, out responseData).Should().BeTrue();
            PendingAuthnRequests.TryRemove(relayState, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequest_TryRemove_NullGivesNull()
        {
            string id = null;
            StoredRequestState state;

            PendingAuthnRequests.TryRemove(id, out state).Should().BeFalse();
            state.Should().BeNull();
        }
    }
}

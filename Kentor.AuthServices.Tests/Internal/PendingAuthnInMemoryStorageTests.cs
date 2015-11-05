using System;
using System.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.IdentityModel.Metadata;
using Kentor.AuthServices.Internal;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class PendingAuthnInMemoryStorageTests
    {
        [TestMethod]
        public void PendingAuthnInMemoryStorage_AddRemove()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            var container = new PendingAuthnInMemoryStorage();
            container.Add(id, requestData);
            StoredRequestState responseData;
            container.TryRemove(id, out responseData).Should().BeTrue();
            responseData.Should().Be(requestData);
            responseData.Idp.Id.Should().Be("testidp");
            responseData.ReturnUrl.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void PendingAuthnInMemoryStorage_Add_ThrowsOnExisting()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            var container = new PendingAuthnInMemoryStorage();
            container.Add(id, requestData);
            Action a = () => container.Add(id, requestData);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnInMemoryStorage_Remove_FalseOnIdNeverIssued()
        {
            var id = new Saml2Id();
            StoredRequestState responseData;
            var container = new PendingAuthnInMemoryStorage();
            container.TryRemove(id, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnInMemoryStorage_Remove_FalseOnRemovedTwice()
        {
            var id = new Saml2Id();
            var requestData = new StoredRequestState(new EntityId("testidp"), new Uri("http://localhost/Return.aspx"));
            StoredRequestState responseData;
            var container = new PendingAuthnInMemoryStorage();
            container.Add(id, requestData);
            container.TryRemove(id, out responseData).Should().BeTrue();
            container.TryRemove(id, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequest_TryRemove_NullGivesNull()
        {
            Saml2Id id = null;
            StoredRequestState state;
            var container = new PendingAuthnInMemoryStorage();
            container.TryRemove(id, out state).Should().BeFalse();
            state.Should().BeNull();
        }
    }
}

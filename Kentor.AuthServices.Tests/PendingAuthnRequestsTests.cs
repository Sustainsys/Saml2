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
            var requestData = new PendingAuthnRequestData("testidp", new Uri("http://localhost/Return.aspx"));
            PendingAuthnRequests.Add(id, requestData);
            PendingAuthnRequestData responseData;
            PendingAuthnRequests.TryRemove(id, out responseData).Should().BeTrue();
            responseData.Should().Be(requestData);
            responseData.Idp.Should().Be("testidp");
            responseData.ReturnUri.Should().Be("http://localhost/Return.aspx");
        }

        [TestMethod]
        public void PendingAuthnRequests_Add_ThrowsOnExisting()
        {
            var id = new Saml2Id();
            var requestData = new PendingAuthnRequestData("testidp", new Uri("http://localhost/Return.aspx"));
            PendingAuthnRequests.Add(id, requestData);
            Action a = () => PendingAuthnRequests.Add(id, requestData);
            a.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnIdNeverIssued()
        {
            var id = new Saml2Id();
            PendingAuthnRequestData responseData;
            PendingAuthnRequests.TryRemove(id, out responseData).Should().BeFalse();
        }

        [TestMethod]
        public void PendingAuthnRequests_Remove_FalseOnRemovedTwice()
        {
            var id = new Saml2Id();
            var requestData = new PendingAuthnRequestData("testidp", new Uri("http://localhost/Return.aspx"));
            PendingAuthnRequestData responseData;
            PendingAuthnRequests.Add(id, requestData);
            PendingAuthnRequests.TryRemove(id, out responseData).Should().BeTrue();
            PendingAuthnRequests.TryRemove(id, out responseData).Should().BeFalse();
        }
    }
}

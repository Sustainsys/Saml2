using FluentAssertions;
using Kentor.AuthServices.Saml2P;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IdentityModel.Metadata;

namespace Kentor.AuthServices.Tests.Saml2P
{
    [TestClass]
    public class Saml2IdpEntryTests
    {
        [TestMethod]
        public void Saml2IdpEntry_NullCheckProviderId()
        {
            var subject = new Saml2IdpEntry(new EntityId("urn:foo"));

            subject.Invoking(s => s.ProviderId = null)
                .ShouldThrow<ArgumentNullException>();
        }
    }
}

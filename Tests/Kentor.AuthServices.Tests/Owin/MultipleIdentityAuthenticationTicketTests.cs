using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Microsoft.Owin.Security;
using FluentAssertions;
using System.Linq;
using Kentor.AuthServices.Owin;

namespace Kentor.AuthServices.Tests.Owin
{
    [TestClass]
    public class MultipleIdentityAuthenticationTicketTests
    {
        [TestMethod]
        public void MultipleIdentityAuthenticationTicket_SavesAllIdentities()
        {
            var identities = new ClaimsIdentity[] { new ClaimsIdentity("auth1"), new ClaimsIdentity("auth2") };
            var properties = new AuthenticationProperties();

            var subject = new MultipleIdentityAuthenticationTicket(identities, properties);

            subject.Identity.Should().Be(identities.First());
            subject.Properties.Should().Be(properties);
            subject.Identities.Should().BeEquivalentTo(identities);
        }

        [TestMethod]
        public void MultipleIdentityAuthenticationTicket_EmptyIdentitySet()
        {
            var identities = Enumerable.Empty<ClaimsIdentity>();
            var properties = new AuthenticationProperties();

            var subject = new MultipleIdentityAuthenticationTicket(identities, properties);

            subject.Identities.Should().BeEmpty();
            subject.Identity.Should().BeNull();
        }
    }
}

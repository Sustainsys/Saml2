using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class IdentityProviderTests
    {
        [TestMethod]
        public void IdentityProvider_CreateAuthenticateRequest_Destination()
        {
            string idpUri = "http://idp.example.com/";
            
            var ip = new IdentityProvider()
            {
                DestinationUri = new Uri(idpUri)
            };

            var r = ip.CreateAuthenticateRequest();

            r.ToXElement().Attribute("Destination").Should().NotBeNull()
                .And.Subject.Value.Should().Be(idpUri);
        }
    }
}
